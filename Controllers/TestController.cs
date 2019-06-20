using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using testapi.Models;

namespace testapi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TotalController : ControllerBase
    {   

        private const double GST_RATE = 0.15;

        [HttpPost]
        public ActionResult<Output> Post([FromBody] Input value)
        {
            String text = value.Text;
            Tuple<string, double> extractedValues;
            try
            {
                validateText(text);

                extractedValues = extractFromText(text);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

            var output =  new Output() 
            {
                CostCentre = extractedValues.Item1,
                Gst = extractedValues.Item2 * GST_RATE,
                TotalExcludingGst = extractedValues.Item2 * (1 - GST_RATE)
            };

            return new ActionResult<Output>(output);
        }

        private void validateText(string text)
        {
            string tagPattern = @"<[^/].+?>";
            foreach (Match match in Regex.Matches(text, tagPattern))
            {
                string openTag = match.Value;
                string closeTag = "</" + openTag.Substring(1);
                if (!text.Substring(match.Index + openTag.Length).Contains(closeTag))
                {
                    throw new ArgumentException(String.Format("Missing closing tag: {0}", closeTag));
                }
            }
        }

        private Tuple<string, double> extractFromText(string text)
        {
            double total = extractTotal(text);
            string costCentre = extractCostCentre(text);

            return new Tuple<string, double>(costCentre, total);
        }

        private string extractCostCentre(string text)
        {
            string pattern = @"<cost_centre>(.+)</cost_centre>";
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string costCentre = match.Groups[1].Captures[0].Value;
                if (!String.IsNullOrWhiteSpace(costCentre)) 
                {
                    return costCentre;
                }
            }
            return "UNKNOWN";
        }

        private double extractTotal(string text)
        {
            string pattern = @"<total>(.+)</total>";
            var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string totalTxt = match.Groups[1].Captures[0].Value;
                double total;
                if (Double.TryParse(totalTxt, out total))
                {
                    return total;
                }
                else 
                {
                    throw new ArgumentException("Unable to parse total");
                }
            }
            throw new ArgumentException("Missing total");
        }

    }
}