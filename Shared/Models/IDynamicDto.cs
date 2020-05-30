using Silgred.Shared.Enums;

namespace Silgred.Shared.Models
{
    internal interface IDynamicDto
    {
        DynamicDtoType DtoType { get; }
    }
}