using System.Text.Json;

namespace YnabMcpServer;

/// <summary>
/// Exception thrown when an error occurs while making a request to the YNAB API
/// </summary>
public class YnabApiException : Exception
{
    public int StatusCode { get; private set; }
    public string Response { get; private set; } = string.Empty;
    public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; private set; } = new Dictionary<string, IEnumerable<string>>();

    public YnabApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception innerException)
        : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + ((response == null) ? "(null)" : response.Substring(0, response.Length >= 512 ? 512 : response.Length)), innerException)
    {
        StatusCode = statusCode;
        Response = response;
        Headers = headers;
    }

    public override string ToString()
    {
        return string.Format("HTTP Response: \n\n{0}\n\n{1}", Response, base.ToString());
    }
}

/// <summary>
/// Exception thrown when an error occurs while deserializing the response from the YNAB API
/// </summary>
public class YnabApiException<TResult> : YnabApiException
{
    public TResult Result { get; private set; }

    public YnabApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, TResult result, Exception innerException)
        : base(message, statusCode, response, headers, innerException)
    {
        Result = result;
    }
}
