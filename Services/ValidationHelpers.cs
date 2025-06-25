using System;
using System.Text.RegularExpressions;
using System.Web;

namespace SecureAuthApp.Services
{
    public static class ValidationHelpers
    {
        public static bool IsInputValid(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Decode potential URL-encoded input
            string decoded = HttpUtility.UrlDecode(input);

            // Reject if input contains HTML tags
            if (Regex.IsMatch(decoded, "<.*?>", RegexOptions.IgnoreCase))
                return false;

            // Reject common SQL/script keywords
            if (Regex.IsMatch(decoded, @"(?i)(select|insert|update|delete|drop|exec|script|alert|--|;)", RegexOptions.IgnoreCase))
                return false;

            // Accept only allowed characters: letters, digits, space, @, #, $
            if (!Regex.IsMatch(decoded, @"^[a-zA-Z0-9 @#\$]+$"))
                return false;

            return true;
        }

        public static string SanitizeForXss(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string decoded = HttpUtility.UrlDecode(input);

            // Remove <script> tags and contents
            decoded = Regex.Replace(decoded, @"<script.*?>.*?</script>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // Remove inline event handlers
            decoded = Regex.Replace(decoded, @"on\w+\s*=\s*[""'].*?[""']", string.Empty, RegexOptions.IgnoreCase);

            // Remove all HTML tags
            decoded = Regex.Replace(decoded, "<.*?>", string.Empty);

            // Strip javascript: pseudo-protocol
            decoded = Regex.Replace(decoded, @"javascript\s*:", string.Empty, RegexOptions.IgnoreCase);

            return decoded.Trim();
        }
    }
}