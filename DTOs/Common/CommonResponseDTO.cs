namespace VertexCRM.DTOs.Common;

public class CommonResponseDTO
{
    public string Status { get; set; }
    public string Message { get; set; }
    public object? Data { get; set; }

    public CommonResponseDTO(string status, string message, object? data = null)
    {
        Status = status;
        Message = message;
        Data = data;
    }
}
