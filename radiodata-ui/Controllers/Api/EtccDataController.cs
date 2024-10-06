using chirpcsvlib;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ukrepeaterlib;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace radiodata_ui.Controllers.Api;

[Route("api")]
[ApiController]
public class EtccDataController(EtccDataService etccDataService) : ControllerBase
{
    // GET api/chirpCsv/IO91lk?km=50&personal=true
    [HttpGet("chirpcsv/{locator}")]
    public async Task<FileResult> Get(string locator, int km = 50, bool personal = false)
    {
        int i = 1;

        var chirpRows = (await etccDataService.GetVhfAndUhfAnalogueTargets(locator, personal, km))
            .Select(r => r.ToChirpCsvRow(commentSuffix: $"{r.DistanceFrom(locator):0}km"))
            .Where(r => r != null)
            .Select(r => r!)
            .Select(r => r with { Location = i++ });

        var csv = ChirpCsvFileUtils.ToCsv(chirpRows);

        return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"chirp-{locator}.csv");
    }

    [HttpGet("json/{locator}")]
    public async Task<IActionResult> GetJson(string locator, int km = 50, bool personal = false)
    {
        var data = (await etccDataService.GetVhfAndUhfAnalogueTargets(locator, personal, km))
            .Where(r => r != null)
            .Select(r => new {
                town = r.Town.ToTitleCase(),
                distance = $"{r.DistanceFrom(locator):0}km",
                locator = r.Locator.CapitaliseLocator(),
                call = r.Repeater,
                input = (r.Rx == 0 ? r.Tx : r.Rx / 1000000.0).ToString("0.000"),
                output = (r.Tx == 0 ? r.Rx : r.Tx / 1000000.0).ToString("0.000"),
                ctcss = r.Ctcss.ToString("0.0"),
            });

        return Ok(data);
    }
}
