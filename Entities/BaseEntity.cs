namespace VertexCRM.Entities;

public class BaseEntity
{
    public int Id { get; set; }

    public string? ChangedBy { get; set; }

    public DateTime? CreatedOn { get; set; }
}
