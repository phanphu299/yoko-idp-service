using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System
{
    public static class StringExtension
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        // public static NameValueCollection ParseQueryString(this string queryString)
        // {
        //     queryString = DecodeUrlString(queryString.Trim('?'));
        //     var array = queryString.Split("?", options: StringSplitOptions.RemoveEmptyEntries);
        //     if (array.Count() > 1)
        //     {
        //         queryString = array[array.Length - 1];
        //     }
        //     else
        //     {
        //         queryString = array[0];
        //     }
        //     return System.Web.HttpUtility.ParseQueryString(queryString);
        // }

        // public static string GetRootPath(this string path)
        // {
        //     var url = path;
        //     if (path.StartsWith("http"))
        //     {
        //         var uri = new System.Uri(path);
        //         var seg = string.Join("", uri.Segments).TrimEnd('/');
        //         url = path.Split(seg)[0];
        //     }
        //     return url;
        // }

        public static string DecodeUrlString(string url)
        {
            //https://stackoverflow.com/questions/1405048/how-do-i-decode-a-url-parameter-using-c
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }

        public static string HashAsSha256String(this string plainText)
        {
            using var sha = SHA256.Create();

            var bytes = Encoding.UTF8.GetBytes(plainText);
            var hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}
