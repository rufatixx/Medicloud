using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlexitHisRazor.Pages;

public class IndexCopyModel : PageModel
{
    private readonly ILogger<IndexCopyModel> _logger;

    public IndexCopyModel(ILogger<IndexCopyModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}

