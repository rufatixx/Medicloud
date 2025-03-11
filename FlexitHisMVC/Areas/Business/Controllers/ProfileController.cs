using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.Category;
using Medicloud.BLL.Services.File;
using Medicloud.BLL.Services.FileUpload;
using Medicloud.BLL.Services.Organization;
using Medicloud.BLL.Services.OrganizationPhoto;
using Medicloud.BLL.Services.Portfolio;
using Medicloud.Models;
using Medicloud.WebUI.Areas.Business.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Medicloud.WebUI.Areas.Business.Controllers
{
	[Area("Business")]
	public class ProfileController : Controller
	{
		private readonly IOrganizationService _organizationService;
		private readonly IFileUploadService _fileUploadService;
		private readonly IOrganizationPhotoService _organizationPhotoService;
		private readonly IPortfolioService _portfolioService;
		private readonly IFileService _fileService;
		private readonly ICategoryService _categoryService;
		public ProfileController(IOrganizationService organizationService, IFileUploadService uploadService, IOrganizationPhotoService organizationPhotoService, IPortfolioService portfolioService, IFileService fileService, ICategoryService categoryService)
		{
			_organizationService = organizationService;
			_fileUploadService = uploadService;
			_organizationPhotoService = organizationPhotoService;
			_portfolioService = portfolioService;
			_fileService = fileService;
			_categoryService = categoryService;
		}

		public async Task<IActionResult> Index()
		{
			var organization = await _organizationService.GetByIdAsync(41);
			var portfolios = await _portfolioService.GetPortfolioByOrganizationIdAsync(41);
			byte[] file = null;
			byte[] coverFile = null;
			if (!string.IsNullOrWhiteSpace(organization.imagePath))
			{
				file = await  _fileUploadService.DownloadFile(organization.imagePath);
			}
			if (!string.IsNullOrWhiteSpace(organization.coverPath))
			{
				coverFile = await _fileUploadService.DownloadFile(organization.coverPath);
			}
			string logoSrc = "";
			string coverSrc = "";
			if (file != null && file.Length > 0)
			{
				var base64String = Convert.ToBase64String(file);
				//if (base64String == userImage)
				//{
				//	userImage = null;
				//}
				string fileExtension = Path.GetExtension(organization.imagePath)?.ToLower();
				logoSrc = $"data:image/{fileExtension};base64,{base64String}";
			}
			if (coverFile != null && coverFile.Length > 0)
			{
				var base64String = Convert.ToBase64String(coverFile);
				string fileExtension = Path.GetExtension(organization.coverPath)?.ToLower();
				coverSrc = $"data:image/{fileExtension};base64,{base64String}";
			}
			var vm = new BusinessProfileVM
			{
				Id = 41,
				Name = organization.name,
				LogoSrc = logoSrc,
				CoverSrc = coverSrc,
				portfolios = portfolios,
			};

			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateCoverPhoto([FromForm] IFormFile coverPhoto, string existingPhotoPath, int organizationId)
		{

			if (!string.IsNullOrEmpty(existingPhotoPath))
			{
				bool deleteFile = _fileUploadService.DeleteFile(existingPhotoPath);

			}
			string filePath = null;
			byte[] file;
			string fileExtension;
			if (coverPhoto != null && coverPhoto.Length != 0)
			{
				using (var memoryStream = new MemoryStream())
				{
					await coverPhoto.CopyToAsync(memoryStream);

					file = memoryStream.ToArray();
				}
				fileExtension = Path.GetExtension(coverPhoto.FileName)?.ToLower();

				if (string.IsNullOrEmpty(fileExtension))
				{
					return BadRequest("Invalid file extension.");
				}

				filePath = $"OrganizationImages/organization_{Guid.NewGuid().ToString()}{fileExtension}";
				bool uploaded = _fileUploadService.UploadFile(file, filePath);
				await _organizationService.UpdateAsync(new()
				{
					coverPath = filePath,
					id = organizationId,
				});
			}

			return RedirectToAction("ProfileImages");
		}

		[HttpPost]
		public async Task<IActionResult> UploadWorkPhoto([FromForm] IFormFile workPhoto, int organizationId)
		{
			Console.WriteLine(organizationId);
			Console.WriteLine("Work");
			Console.WriteLine(workPhoto?.Name);

			string filePath = null;
			byte[] file;
			string fileExtension;
			if (workPhoto != null && workPhoto.Length != 0)
			{
				using (var memoryStream = new MemoryStream())
				{
					await workPhoto.CopyToAsync(memoryStream);

					file = memoryStream.ToArray();
				}
				fileExtension = Path.GetExtension(workPhoto.FileName)?.ToLower();

				if (string.IsNullOrEmpty(fileExtension))
				{
					return BadRequest("Invalid file extension.");
				}
				var fileName = Guid.NewGuid().ToString();
				filePath = $"OrganizationImages/organization_{fileName}{fileExtension}";
				bool uploaded = _fileUploadService.UploadFile(file, filePath);
				if (uploaded)
				{
					await _organizationPhotoService.AddAsync(organizationId, new()
					{
						filePath = filePath,
						fileName = fileName,
					});
				}
			}

			return RedirectToAction("ProfileImages");

		}

		public async Task<IActionResult> ProfileImages()
		{
			var organization = await _organizationService.GetByIdAsync(41);
			byte[] file = null;
			byte[] coverFile = null;
			if (!string.IsNullOrWhiteSpace(organization.imagePath))
			{
				file =await _fileUploadService.DownloadFile(organization.imagePath);
			}
			if (!string.IsNullOrWhiteSpace(organization.coverPath))
			{
				coverFile =await _fileUploadService.DownloadFile(organization.coverPath);
			}
			string logoSrc = "";
			string coverSrc = "";
			if (file != null && file.Length > 0)
			{
				var base64String = Convert.ToBase64String(file);
				//if (base64String == userImage)
				//{
				//	userImage = null;
				//}
				string fileExtension = Path.GetExtension(organization.imagePath)?.ToLower();
				logoSrc = $"data:image/{fileExtension};base64,{base64String}";
			}

			if (coverFile != null && coverFile.Length > 0)
			{
				var base64String = Convert.ToBase64String(coverFile);
				string fileExtension = Path.GetExtension(organization.coverPath)?.ToLower();
				coverSrc = $"data:image/{fileExtension};base64,{base64String}";
			}
			var workPhotos = await _organizationPhotoService.GetByOrganizationId(41);
			if (workPhotos != null)
			{
				foreach (var item in workPhotos)
				{
					var itemFile =await _fileUploadService.DownloadFile(item.filePath);
					if (itemFile != null && itemFile.Length > 0)
					{
						var base64String = Convert.ToBase64String(itemFile);
						string fileExtension = Path.GetExtension(item.filePath)?.ToLower();
						item.Src = $"data:image/{fileExtension};base64,{base64String}";
					}

				}
			}
			var vm = new BusinessProfileVM
			{
				Id = 41,
				Name = organization.name,
				LogoSrc = logoSrc,
				CoverSrc = coverSrc,
				WorkImages = workPhotos,
				CoverPath = organization.coverPath,
				LogoPath = organization.imagePath
			};

			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateLogoPhoto([FromForm] IFormFile logoPhoto, string existingPhotoPath, int organizationId)
		{


			if (!string.IsNullOrEmpty(existingPhotoPath))
			{
				bool deleteFile = _fileUploadService.DeleteFile(existingPhotoPath);

			}
			if (organizationId == 0)
			{
				Console.WriteLine("OrganizationID 0 geldi");
				return RedirectToAction("Index");

			}

			string filePath = null;
			byte[] file;
			string fileExtension;
			if (logoPhoto != null && logoPhoto.Length != 0)
			{
				using (var memoryStream = new MemoryStream())
				{
					await logoPhoto.CopyToAsync(memoryStream);

					file = memoryStream.ToArray();
				}
				fileExtension = Path.GetExtension(logoPhoto.FileName)?.ToLower();

				if (string.IsNullOrEmpty(fileExtension))
				{
					return BadRequest("Invalid file extension.");
				}

				filePath = $"OrganizationImages/organization_{Guid.NewGuid().ToString()}{fileExtension}";
				bool uploaded = _fileUploadService.UploadFile(file, filePath);
				await _organizationService.UpdateAsync(new()
				{
					imagePath = filePath,
					id = organizationId,
				});
			}

			return RedirectToAction("ProfileImages");

		}


		

	}
}

