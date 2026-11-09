using System;
using System.Globalization;
using System.Text;

namespace Requestrr.WebApi.RequestrrBot.DownloadClients
{
    public static class RequesterTagHelper
    {
        public static string CreateTag(string userId, string username)
        {
            var normalizedUsername = Normalize(username);
            var normalizedUserId = Normalize(userId);

            if (string.IsNullOrEmpty(normalizedUsername))
            {
                normalizedUsername = "discord-user";
            }

            if (string.IsNullOrEmpty(normalizedUserId))
            {
                normalizedUserId = "unknown";
            }

            var idSuffix = normalizedUserId.Length > 6
                ? normalizedUserId.Substring(normalizedUserId.Length - 6)
                : normalizedUserId;

            var tag = $"requestrr-{normalizedUsername}-{idSuffix}";
            return tag.Length <= 64 ? tag : $"requestrr-{normalizedUsername.Substring(0, Math.Max(1, 64 - idSuffix.Length - 16))}-{idSuffix}";
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var decomposed = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            var pendingHyphen = false;

            foreach (var character in decomposed)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(character);
                if (category == UnicodeCategory.NonSpacingMark)
                {
                    continue;
                }

                if ((character >= 'a' && character <= 'z') || (character >= 'A' && character <= 'Z') || (character >= '0' && character <= '9'))
                {
                    if (pendingHyphen && builder.Length > 0)
                    {
                        builder.Append('-');
                    }

                    builder.Append(char.ToLowerInvariant(character));
                    pendingHyphen = false;
                }
                else if (builder.Length > 0)
                {
                    pendingHyphen = true;
                }
            }

            return builder.ToString().Trim('-');
        }
    }
}
