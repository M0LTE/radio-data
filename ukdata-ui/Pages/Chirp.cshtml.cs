using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace radiodata_ui.Pages;

public class ChirpModel : PageModel
{
    private readonly ILogger<ChirpModel> _logger;

    public ChirpModel(ILogger<ChirpModel> logger)
    {
        _logger = logger;
    }

}
