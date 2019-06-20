using System;
using System.Text.RegularExpressions;

namespace testapi.Services
{
    public class TextTagValidatorImpl : TextTagValidator
    {
        public void ValidateTags(testapi.Models.Input input){
            string text = input.Text;
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
    } 
}