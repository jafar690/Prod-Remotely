using System.Runtime.Serialization;
using Silgred.Shared.Enums;

namespace Silgred.Shared.Models
{
    [DataContract]
    public class FrameInfo : IDynamicDto
    {
        [DataMember(Name = "EndOfFrame")] public bool EndOfFrame { get; set; }

        [DataMember(Name = "Height")] public int Height { get; set; }

        [DataMember(Name = "ImageBytes")] public byte[] ImageBytes { get; set; }

        [DataMember(Name = "Left")] public int Left { get; set; }

        [DataMember(Name = "Top")] public int Top { get; set; }

        [DataMember(Name = "Width")] public int Width { get; set; }

        [DataMember(Name = "DtoType")] public DynamicDtoType DtoType { get; set; }
    }
}