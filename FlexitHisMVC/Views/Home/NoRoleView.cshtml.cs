using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FlexitHisRazor.Pages;

public class NoRoleViewModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public NoRoleViewModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}

