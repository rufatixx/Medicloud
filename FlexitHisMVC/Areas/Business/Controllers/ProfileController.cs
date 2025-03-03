using Medicloud.BLL.Services.FileUpload;
using Medicloud.BLL.Services.Organization;
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
		public ProfileController(IOrganizationService organizationService, IFileUploadService uploadService)
		{
			_organizationService = organizationService;
			_fileUploadService = uploadService;
		}

		public async Task<IActionResult> Index()
		{
			var organization = await _organizationService.GetByIdAsync(41);
			byte[] file = null;
			byte[] coverFile = null;
			if (!string.IsNullOrWhiteSpace(organization.imagePath))
			{
				file = _fileUploadService.DownloadFile(organization.imagePath);
			}
			if (!string.IsNullOrWhiteSpace(organization.coverPath))
			{
				coverFile = _fileUploadService.DownloadFile(organization.coverPath);
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
				Id=41,
				Name = organization.name,
				LogoSrc=logoSrc,
				CoverSrc=coverSrc,
			};
			
			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateCoverPhoto([FromForm] IFormFile coverPhoto, string existingPhotoPath,int organizationId)
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

			return RedirectToAction("Index");
		}

		public async Task<IActionResult> ProfileImages()
		{
			var organization = await _organizationService.GetByIdAsync(41);
			byte[] file = null;
			byte[] coverFile = null;
			if (!string.IsNullOrWhiteSpace(organization.imagePath))
			{
				file = _fileUploadService.DownloadFile(organization.imagePath);
			}
			if (!string.IsNullOrWhiteSpace(organization.coverPath))
			{
				coverFile = _fileUploadService.DownloadFile(organization.coverPath);
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

			return RedirectToAction("Index");
		}
	}
}

