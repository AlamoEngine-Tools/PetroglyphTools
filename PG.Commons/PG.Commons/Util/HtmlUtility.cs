// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics;

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
            Debug.Assert(uri != null, nameof(uri) + " != null");
            string s = uri.Trim();
            if (!uri.Equals(s, StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            return Uri.TryCreate(uri, UriKind.Absolute, result: out Uri uriResult)
                   && (uriResult?.Scheme == Uri.UriSchemeHttp || uriResult?.Scheme == Uri.UriSchemeHttps);
        }
    }
}
