using VertexCRM.Constants;

namespace VertexCRM.Entities;

public sealed class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public RoleScope Scope { get; set; }

    // Navigation properties
    public ICollection<User> Users { get; set; }
        = [];
}