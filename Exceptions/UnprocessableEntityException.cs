namespace VertexCRM.Exceptions;

/// <summary>
/// Thrown when the request is well-formed but fails semantic/business validation.
/// Maps to HTTP 422 Unprocessable Entity.
/// </summary>
public class UnprocessableEntityException : Exception
{
    public UnprocessableEntityException(string message) : base(message) { }
}
