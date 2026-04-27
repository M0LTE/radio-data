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
    [HttpGet]

    // GET api/icomcsv/IO91lk?km=50&personal=true
    [HttpGet("icomcsv/{locator}")]
    public async Task<FileResult> GetIcomIncludingDV(string locator, int km = 50, bool personal = false)
    {
        var data = (await etccDataService.GetDstarTargets(locator, personal, km))
            .Concat(await etccDataService.GetVhfAndUhfAnalogueTargets(locator, personal, km)).ToArray();

        Dictionary<(char mode, string bandName), (int groupNumber, string groupName)> groups = new();

        var rows = data
            .Select(r => r.ToIcomCsvRow());

        int group = 1;
        foreach (var r in rows)
        {
            var band = AmateurBandLib.AmateurBand.FromHz((long)(r!.Frequency * 1000000)).Name;
            if (!groups.ContainsKey((r.Mode[0], band)))
            {
                groups.Add((r.Mode[0], band), (group++, $"{band} {(r.Mode[0] == 'D' ? "D-STAR" : "FM")}"));
            }
        }

        var rows2 = rows
            .Where(r => r != null)
            .Select(r => r!)
            .Select(r => r with
            {
                //GroupNo = r.Mode == "DV" ? 1 : 2,
                //GroupName = r.Mode == "DV" ? "D-STAR" : "Analogue",
                GroupNo = groups[(r.Mode[0], AmateurBandLib.AmateurBand.FromHz((long)(r.Frequency * 1000000)).Name)].groupNumber,
                GroupName = groups[(r.Mode[0], AmateurBandLib.AmateurBand.FromHz((long)(r.Frequency * 1000000)).Name)].groupName,
            })
            .OrderBy(r => r.GroupNo).ThenBy(r => r.RepeaterCallsign);

        var csv = RadioCsvFileUtils.ToCsv(rows2);

        return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"icom-{locator}.csv");
    }

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

        var csv = RadioCsvFileUtils.ToCsv(chirpRows);

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
                input = (r.Rx == 0 ? r.Tx : r.Rx).ToMHzString(),
                output = (r.Tx == 0 ? r.Rx : r.Tx).ToMHzString(),
                ctcss = r.Ctcss.ToString("0.0"),
            });

        return Ok(data);
    }
}
