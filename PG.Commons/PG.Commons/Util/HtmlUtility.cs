using System;

namespace PG.Commons.Util
{
    public static class HtmlUtility
    {
        public static bool IsValidUri(string uri)
        {
            if (StringUtility.IsNullEmptyOrWhiteSpace(uri))
            {
                return false;
            }
            return Uri.TryCreate(uri, UriKind.Absolute, result: out Uri uriResult)
                   && (uriResult?.Scheme == Uri.UriSchemeHttp || uriResult?.Scheme == Uri.UriSchemeHttps);
        }
    }
}
