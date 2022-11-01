namespace GettingMessagesTelegram.Helpers;

public class WordHelper
{
    /// <summary>
    /// Split in sentences
    /// </summary>
    private const string Delimitary = ".!?()-:;,";

    /// <summary>
    /// Get split text
    /// </summary>
    public static string GetSplitByWord(string content, int maxLength)
    {
        List<string> result = new List<string>();

        if (string.IsNullOrWhiteSpace(content))
        {
            return content;
        }
        // check is edge of text
        if (content.Length < maxLength)
        {
            return content;
        }

        // send end of text
        int end = maxLength;
        for (; end > 0; end--)
        {
            if (Delimitary.IndexOf(content[end]) > -1)
            {
                // if before space
                if (end != (maxLength) && content[end + 1] == ' ')
                {
                    return content.Substring(0, end);
                }
            }
        }


        return content.Substring(0, maxLength - 1);
    }

}