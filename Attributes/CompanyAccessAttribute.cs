using Microsoft.AspNetCore.Mvc;
using VertexCRM.Filters;

namespace VertexCRM.Attributes;

public sealed class CompanyAccessAttribute : TypeFilterAttribute
{
    public CompanyAccessAttribute()
        : base(typeof(CompanyAccessFilter))
    {
    }
}