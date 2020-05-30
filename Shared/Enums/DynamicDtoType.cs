using System.Runtime.Serialization;

namespace Silgred.Shared.Enums
{
    [DataContract]
    public enum DynamicDtoType
    {
        [EnumMember(Value = "FrameInfo")] FrameInfo = 0
    }
}