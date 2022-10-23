
namespace PublishImage.Helpers
{
    public class SessionHelper
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random Random = new();

        /// <summary>
        /// Get one time session
        /// </summary>
        /// <param name="length">Length of session</param>
        public static string GetSession(int length)
        {

            return new string(Enumerable.Repeat(Chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
