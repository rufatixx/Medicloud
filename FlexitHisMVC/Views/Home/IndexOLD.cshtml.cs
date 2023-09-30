using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlexitHisRazor.Pages;

public class IndexOLDModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexOLDModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}

