using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.Category;
using Medicloud.BLL.Services.File;
using Medicloud.BLL.Services.FileUpload;
using Medicloud.BLL.Services.Organization;
using Medicloud.BLL.Services.OrganizationPhoto;
using Medicloud.BLL.Services.Portfolio;
using Medicloud.BLL.Services.Staff;
using Medicloud.WebUI.Areas.Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Medicloud.WebUI.Areas.Business.Controllers
{
	[Authorize]
	[Area("Business")]
	public class ProfileController : Controller
	{
		private readonly IOrganizationService _organizationService;
		private readonly IFileUploadService _fileUploadService;
		private readonly IOrganizationPhotoService _organizationPhotoService;
		private readonly IPortfolioService _portfolioService;
		private readonly IFileService _fileService;
		private readonly ICategoryService _categoryService;
		private readonly IStaffService _staffService;
		public ProfileController(IOrganizationService organizationService, IFileUploadService uploadService, IOrganizationPhotoService organizationPhotoService, IPortfolioService portfolioService, IFileService fileService, ICategoryService categoryService, IStaffService staffService)
		{
			_organizationService = organizationService;
			_fileUploadService = uploadService;
			_organizationPhotoService = organizationPhotoService;
			_portfolioService = portfolioService;
			_fileService = fileService;
			_categoryService = categoryService;
			_staffService = staffService;
		}

		public async Task<IActionResult> Index()
		{
			int activeOrganizationId = HttpContext.Session.GetInt32("activeOrgId") ?? 0;
			if(activeOrganizationId == 0)
			{
				int userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

				var organizations = await _organizationService.GetUserOrganizations(userId);
				if (organizations != null && organizations.Count > 0)
				{
					var active = organizations.Last();
					HttpContext.Session.SetInt32("activeOrgId", active.Id);

				}

			}
			if (activeOrganizationId == 0)
			{
				return RedirectToAction("Index", "Registration", new { area = "business" });

			}
			var organization = await _organizationService.GetByIdAsync(activeOrganizationId);
			var portfolios = await _portfolioService.GetPortfolioByOrganizationIdAsync(activeOrganizationId);
			byte[] file = null;
			byte[] coverFile = null;
			var vm = new BusinessProfileVM
			{
				Id = activeOrganizationId,
				Name = organization.name,
				portfolios = portfolios,
				LogoId = organization.logoId,
				CoverId = organization.coverId,
			};


			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateCoverPhoto([FromForm] IFormFile coverPhoto, int existingCoverId, int organizationId)
		{


			if (existingCoverId > 0)
			{
				var existFile = await _fileService.GetById(existingCoverId);
				if (existFile != null)
				{
					bool deleteFile = _fileUploadService.DeleteFile(existFile.filePath);
					await _organizationService.UpdateCover(null, organizationId);
				}

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
				string fileName = $"organization_{Guid.NewGuid().ToString()}";
				filePath = $"OrganizationImages/{fileName}{fileExtension}";
				bool uploaded = await _fileUploadService.UploadFileAsync(file, filePath, true);
				if (uploaded)
				{
					await _organizationService.UpdateCover(new()
					{
						fileName = fileName,
						filePath = filePath,
					}, organizationId);
				}

			}

			return RedirectToAction("ProfileImages");
		}

		[HttpPost]
		public async Task<IActionResult> UpdateCover(int existFileId, int organizationId)
		{
			var file = await _fileService.GetById(existFileId);

			if (file != null)
			{
				var fileData = await _fileUploadService.DownloadFileAsync(file.filePath);
				if (fileData != null)
				{
					string fileExtension = Path.GetExtension(file.fileName)?.ToLower();
					string fileName = $"organization_{Guid.NewGuid().ToString()}";
					string filePath = $"OrganizationImages/{fileName}{fileExtension}";
					bool uploaded = await _fileUploadService.UploadFileAsync(fileData, filePath, true);
					if (uploaded)
					{
						await _organizationService.UpdateCover(new()
						{
							fileName = fileName,
							filePath = filePath,
						}, organizationId);
					}

				}


			}


			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> UploadWorkPhoto([FromForm] IFormFile workPhoto, int organizationId)
		{
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
				bool uploaded = await _fileUploadService.UploadFileAsync(file, filePath, true);
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
			int activeOrganizationId = HttpContext.Session.GetInt32("activeOrgId") ?? 0;
			string beforeUrl = Request.Headers["Referer"].ToString();
			string currentUrl = Request.Path.ToString();
			Console.WriteLine(currentUrl);
			Console.WriteLine(beforeUrl);
			if (!beforeUrl.ToLower().Contains(currentUrl.ToLower()))
			{
				ViewBag.BeforeUrl = beforeUrl;
			}

			var organization = await _organizationService.GetByIdAsync(activeOrganizationId);

			var workPhotos = await _organizationPhotoService.GetByOrganizationId(activeOrganizationId);
			var vm = new BusinessProfileVM
			{
				Id = activeOrganizationId,
				Name = organization.name,
				WorkImages = workPhotos,
				LogoId = organization.logoId,
				CoverId = organization.coverId,
			};

			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateLogoPhoto([FromForm] IFormFile logoPhoto, int existingLogoId, int organizationId)
		{


			if (existingLogoId > 0)
			{
				var existFile = await _fileService.GetById(existingLogoId);
				if (existFile != null)
				{
					bool deleteFile = _fileUploadService.DeleteFile(existFile.filePath);

				}
				await _organizationService.UpdateLogo(null, organizationId);

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

				string fileName = $"organization_{Guid.NewGuid().ToString()}";
				filePath = $"OrganizationImages/{fileName}{fileExtension}";
				bool uploaded = await _fileUploadService.UploadFileAsync(file, filePath, true);
				if (uploaded)
					await _organizationService.UpdateLogo(new()
					{
						filePath = filePath,
						fileName = fileName,
					}, organizationId);

			}

			return RedirectToAction("ProfileImages");

		}


		public async Task<IActionResult> Info()
		{
			int activeOrganizationId = HttpContext.Session.GetInt32("activeOrgId") ?? 0;


			var organization = await _organizationService.GetByIdAsync(activeOrganizationId);
			var ownerStaff = await _staffService.GetOwnerStaffByOrganizationId(activeOrganizationId);
			var portfolios = await _portfolioService.GetPortfolioByOrganizationIdAsync(activeOrganizationId);
			byte[] file = null;
			byte[] coverFile = null;
			var vm = new UpdateOrganizationDTO
			{
				Id = activeOrganizationId,
				Name = organization.name,
				StaffPhoneNumber = ownerStaff.phoneNumber,
				StaffEmail=ownerStaff.email,
				OnlineShopLink=organization.onlineShopLink,
				Description = organization.description,
				FbLink=organization.fbLink,
				ILink=organization.insLink,
				WebLink=organization.website,

			};

			return View(vm);
		}



		[HttpPost]
		public async Task<IActionResult> UpdateInfo([FromForm] UpdateOrganizationDTO dto)
		{
			bool updated=await _organizationService.UpdateAsync(dto);
			return RedirectToAction("Info");

		}

	}
}

