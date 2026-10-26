using System.Net;

namespace VertexCRM.Exceptions;

/// <summary>
/// Thrown for any known, intentional application error whose message is safe
/// to expose directly to API consumers.
/// The caller specifies the HTTP status code alongside the user-facing message,
/// so the global exception handler needs no switch — it just reads StatusCode
/// and Message from this class and returns them. Every other exception type
/// falls through to a generic 500 response so internal details never leak.
/// </summary>
public class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public AppException(HttpStatusCode statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
