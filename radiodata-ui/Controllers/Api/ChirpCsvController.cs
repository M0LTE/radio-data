using chirpcsvlib;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ukrepeaterlib;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace radiodata_ui.Controllers.Api;

[Route("api/[controller]")]
[ApiController]
public class ChirpCsvController(EtccDataService etccDataService) : ControllerBase
{
    // GET api/chirpCsv/IO91lk
    [HttpGet("{locator}")]
    public async Task<FileResult> Get(string locator, int? km)
    {
        int i = 0;

        var chirpRows = (await etccDataService.GetVhfAndUhfAnalogueTargets(locator, km))
            .Select(r => r.ToChirpCsvRow(commentSuffix: $"{r.DistanceFrom(locator):0}km"))
            .Where(r => r != null)
            .Select(r => r!)
            .Select(r => r with { Location = i++ });

        var csv = ChirpCsvFileUtils.ToCsv(chirpRows);

        return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"chirp-{locator}.csv");
    }
}