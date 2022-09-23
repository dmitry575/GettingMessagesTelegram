using System.Text.RegularExpressions;

namespace GettingMessagesTelegram.Helpers;

public class UrlHelper
{
    public static string GetTmeUrl(string text)
    {
        Match match = Regex.Match(text, @"https\://t\.me/[a-zA-Z0-9\-_\.]+(/\S*)?$");
        if (match.Success)
        {
            return match.Value;
        }

        return string.Empty;
    }
}
