using System;
using testapi.Models;
using System.Text.RegularExpressions;

namespace testapi.Services
{
    public class OutputCreatorImpl : OutputCreator
    {

        private const double GST_RATE = 0.15;

        private const string DEFAULT_COST_CENTRE = "UNKNOWN";

        public Output create(Input input)
        {
            string text = input.Text;
            Tuple<string, double> extractedValues = extractFromText(text); ;

            return new Output()
            {
                CostCentre = extractedValues.Item1,
                Gst = extractedValues.Item2 * GST_RATE,
                TotalExcludingGst = extractedValues.Item2 * (1 - GST_RATE)
            };
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
            return DEFAULT_COST_CENTRE;
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