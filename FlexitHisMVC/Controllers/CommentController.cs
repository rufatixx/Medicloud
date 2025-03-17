using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Medicloud.WebUI.Controllers
{
	[Authorize]
	public class CommentController : Controller
	{
		private readonly ICommentService _commentService;

		public CommentController(ICommentService commentService)
		{
			_commentService = commentService;
		}
		[HttpPost]
		public async Task<IActionResult> AddComment([FromBody]AddPortfoliioCommentDTO dto)
		{
			Console.WriteLine(dto.Description);
			int userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
			dto.UserId = userId;
			int result=await _commentService.AddPortfolioCommentAsync(dto);
			return Ok(result);
		}
		
		public async Task<IActionResult> GetPortfolioComments(int id)
		{
			var result = await _commentService.GetPortfolioCommentAsync(id);

			return Ok(result);
		}
	}
}
