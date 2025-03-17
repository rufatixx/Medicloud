
using Medicloud.BLL.DTO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.Comment;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Medicloud.BLL.Services.Comment
{
	public class CommentService:ICommentService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICommentRepository _commentRepository;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CommentService(IUnitOfWork unitOfWork, ICommentRepository commentRepository, IHttpContextAccessor httpContextAccessor)
		{
			_unitOfWork = unitOfWork;
			_commentRepository = commentRepository;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<int> AddPortfolioCommentAsync(AddPortfoliioCommentDTO dto)
		{
			using var con = _unitOfWork.BeginTransaction();
			int commentId= await _commentRepository.AddAsync(new()
			{
				description = dto.Description,
				cDate = DateTime.Now,
				userId = dto.UserId,
				parentId = dto.ParentId,
			});
			if (dto.ParentId == 0)
			{
				int relId = await _commentRepository.AddPortfolioCommentAsync(new()
				{
					FirstModelId = dto.PortfolioId,
					SecondModelId = commentId,
				});
			}

			_unitOfWork.SaveChanges();
			return commentId;
		}

		public async Task<List<CommentDTO>> GetPortfolioCommentAsync(int id)
		{
			using var con=_unitOfWork.BeginConnection();
			var data=await _commentRepository.GetPortfolioCommentAsync(id);
			int userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var result=new List<CommentDTO>();
			if (data != null)
			{
				foreach (var comment in data)
				{
					var dto = new CommentDTO()
					{
						CDate = comment.cDate,
						Description = comment.description,
						Id = comment.id,
						UserId = comment.userId,
						UserName = comment.Username,
						IsAuthor = comment.userId == userId,
					};
					var repliesData = await _commentRepository.GetCommentReplies(comment?.id??0);
					if (repliesData != null)
					{
						dto.Replies = new();
						foreach (var item in repliesData)
						{
							dto.Replies.Add(new()
							{
								CDate = item.cDate,
								Description = item.description,
								Id = item.id,
								UserId = item.userId,
								UserName = item.Username,
								IsAuthor = item.userId == userId,
							});
						}
					}
					result.Add(dto);

				}
			}
			return result;
		}
	}
}
