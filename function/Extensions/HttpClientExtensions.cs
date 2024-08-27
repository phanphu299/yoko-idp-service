namespace System.Net.Http
{
    public static class HttpClientExtensions
    {
        public static void AddClientHeader(this HttpClient httpClient, string headerName, string value)
        {
            if (string.IsNullOrEmpty(headerName))
                throw new Exception("Header is null");
            if (string.IsNullOrEmpty(value))
                throw new Exception("Value is null");
            if (httpClient.DefaultRequestHeaders.Contains(headerName))
            {
                httpClient.DefaultRequestHeaders.Remove(headerName);
            }
            httpClient.DefaultRequestHeaders.Add(headerName, value);
        }
    }
}