using System;

namespace Rbit.CommandLineTool.Support
{
    public static class UrlHelper
    {
        public static Uri EnsureCorrectUri(string url)
        {
            if (url.Contains("http://") || url.Contains("https://")) { return new Uri(url); }

            return new Uri($"http://{url}");
        }
    }
}
