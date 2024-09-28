using chirpcsvlib;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using ukrepeaterlib;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace radiodata_ui.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChirpCsvController : ControllerBase
    {
        // GET api/chirpCsv/IO91lk
        [HttpGet("{locator}")]
        public async Task<FileResult> Get(string locator)
        {
            var client = new EtccApiClient();

            var data = await client.GetAll();

            var vhfAndUhfRepeaters = data
                .Where(r => r.Band == "2M" || r.Band == "70CM")
                .Where(r => r.Type == "AV")
                .Where(r => r.ModeCodes.Contains(EtccModeFlag.Analogue))
                .OrderBy(r => r.DistanceFrom(locator) ?? double.MaxValue)
                .ToList();

            int i = 0;
            var chirpRows = vhfAndUhfRepeaters
                .Select(r => r.ToChirpCsvRow(i++, commentSuffix: $"{r.DistanceFrom(locator):0}km"))
                .Where(r => r != null)
                .Select(r => r!);

            var csv = ChirpCsvFileUtils.ToCsv(chirpRows);

            return File(Encoding.UTF8.GetBytes(csv), "text/csv", $"chirp-{locator}.csv");
        }
    }
}
