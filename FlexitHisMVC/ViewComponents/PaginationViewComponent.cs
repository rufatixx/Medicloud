using Microsoft.AspNetCore.Mvc;

namespace Medicloud.ViewComponents
{
	public class PaginationViewComponent:ViewComponent
	{
		public IViewComponentResult Invoke(int currentPage, int totalPages, string searchQuery)
		{
			int halfRange = 3;
			int startPage = currentPage - halfRange;
			int endPage = currentPage + halfRange;

			if (startPage <= 0)
			{
				endPage -= (startPage - 1);
				startPage = 1;
			}
			if (endPage > totalPages)
			{
				startPage -= (endPage - totalPages);
				endPage = totalPages;
			}
			if (startPage <= 0) startPage = 1;

			int loopStart = startPage;
			int loopEnd = Math.Min(startPage + 6, totalPages);

			var paginationModel = new PaginationModel
			{
				CurrentPage = currentPage,
				TotalPages = totalPages,
				StartPage = loopStart,
				EndPage = loopEnd,
				SearchQuery = searchQuery
			};

			return View(paginationModel);
		}

	}
	public class PaginationModel
	{
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int StartPage { get; set; }
		public int EndPage { get; set; }
		public string SearchQuery { get; set; }
	}
}
