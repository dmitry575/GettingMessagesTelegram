namespace GettingMessagesTelegram.Helpers;

public class WordHelper
{
    /// <summary>
    /// Split in sentences
    /// </summary>
    public const string Delimitary = ".!?()-:;,\n";

    /// <summary>
    /// Split in title
    /// </summary>
    public const string DelimitaryTitle = ".!?()-:;,\n ";

    /// <summary>
    /// Get split text
    /// </summary>
    public static string GetSplitByWord(string content, int maxLength, string delimitr = Delimitary)
    {
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
        int end = maxLength - 1;
        for (; end > 0; end--)
        {
            if (Delimitary.IndexOf(content[end]) > -1)
            {
                // if before space
                if (end != maxLength-1 && content[end + 1] == ' ' || content[end] == '\n')
                {
                    return content.Substring(0, end);
                }
            }
        }


        return content.Substring(0, maxLength - 1);
    }

}