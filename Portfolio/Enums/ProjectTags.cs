using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Portfolio.Enums;

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProjectTags
{
    [EnumMember(Value = "Finished")]
    Finished = 1 << 1,
    [EnumMember(Value = "In Development")]
    InDevelopment = 1 << 2,
    [EnumMember(Value = "Prototype")]
    Prototype = 1 << 3,
    
    [EnumMember(Value = "Unity Engine")]
    Unity = 1 << 4,
    [EnumMember(Value = "Unreal Engine")]
    Unreal = 1 << 5,
    [EnumMember(Value = ".Net")]
    DotNet = 1 << 6,
    
    [EnumMember(Value = "CMake")]
    CMake = 1 << 7,
    [EnumMember(Value = "Make Files")]
    MakeFiles = 1 << 8,
    
    [EnumMember(Value = "C")]
    C = 1 << 9,
    [EnumMember(Value = "C++")]
    Cxx = 1 << 10,
    [EnumMember(Value = "C#")]
    CSharp = 1 << 11,
    
    UnityProject = Unity | CSharp,
    UnityFinished = Finished | UnityProject,
    UnityInDev = InDevelopment | UnityProject,
    UnityPrototype = Prototype | UnityProject,
    
    CProject = CMake | C,
    CFinished = Finished | CProject,
    CInDev = InDevelopment | CProject,
    CPrototype = Prototype | CProject,
    
    CxxProject = CMake | Cxx,
    CxxFinished = Finished | CxxProject,
    CxxInDev = InDevelopment | CxxProject,
    CxxPrototype = Prototype | CxxProject,
    
    CSharpProject = DotNet | CSharp,
    CSharpFinished = Finished | CSharpProject,
    CSharpInDev = InDevelopment | CSharpProject,
    CSharpPrototype = Prototype | CSharpProject,
}