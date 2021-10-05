using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace rapide_shortener_service.Services.Utility
{
    public class URLGenerator
    {
        public static string GenerateShortUrl()
        {
            string urlSafe = string.Empty;
            Enumerable.Range(48, 75)
              .Where(i => i < 58 || i > 64 && i < 91 || i > 96)
              .OrderBy(o => new Random().Next())
              .ToList()
              .ForEach(i => urlSafe += Convert.ToChar(i));
            urlSafe = String.Join("", urlSafe.ToCharArray().OrderBy(x => Guid.NewGuid()).Select(c => c.ToString()).ToArray());
            int offset = new Random().Next(0, urlSafe.Length - 2);
            int length = new Random().Next(2, urlSafe.Length - offset);
            if (length > 10) length = length - 6;
            if (length > 20) length = length - 14;
            if (length > 7) length = length - 5;
            string token = urlSafe.Substring(offset, length);
            return "https://share.lunaar.net/" + token;

        }
    }
}