using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Portfolio.Enums;

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProjectTags
{
    [EnumMember(Value = "Finished")]
    Finished = 1 << 0,
    [EnumMember(Value = "In Development")]
    InDevelopment = 1 << 1,
    [EnumMember(Value = "Prototype")]
    Prototype = 1 << 2,
    
    [EnumMember(Value = "Unity Engine")]
    Unity = 1 << 3,
    [EnumMember(Value = "Unreal Engine")]
    Unreal = 1 << 4,
    [EnumMember(Value = ".Net")]
    DotNet = 1 << 5,
    
    [EnumMember(Value = "CMake")]
    CMake = 1 << 6,
    [EnumMember(Value = "Make Files")]
    MakeFiles = 1 << 7,
    
    [EnumMember(Value = "C")]
    C = 1 << 8,
    [EnumMember(Value = "C++")]
    Cxx = 1 << 9,
    [EnumMember(Value = "C#")]
    CSharp = 1 << 10,
    
    [EnumMember(Value = "Library")]
    Lib = 1 << 11,
    
    [EnumMember(Value = "Unity Project")]
    UnityProject = Unity | CSharp,
    [EnumMember(Value = "Finished Unity Project")]
    UnityFinished = Finished | UnityProject,
    [EnumMember(Value = "Develop Unity Project")]
    UnityInDev = InDevelopment | UnityProject,
    [EnumMember(Value = "Prototype Unity Project")]
    UnityPrototype = Prototype | UnityProject,
    
    [EnumMember(Value = "Unreal C++ Project")]
    UnrealCxxProject = Unreal | Cxx,
    
    [EnumMember(Value = "C Project")]
    CProject = CMake | C,
    [EnumMember(Value = "Finished C Project")]
    CFinished = Finished | CProject,
    [EnumMember(Value = "Develop C Project")]
    CInDev = InDevelopment | CProject,
    [EnumMember(Value = "Prototype C Project")]
    CPrototype = Prototype | CProject,
    
    [EnumMember(Value = "C++ Project")]
    CxxProject = CMake | Cxx,
    [EnumMember(Value = "Finished C++ Project")]
    CxxFinished = Finished | CxxProject,
    [EnumMember(Value = "Develop C++ Project")]
    CxxInDev = InDevelopment | CxxProject,
    [EnumMember(Value = "Prototype C++ Project")]
    CxxPrototype = Prototype | CxxProject,
    
    [EnumMember(Value = "C# Project")]
    CSharpProject = DotNet | CSharp,
    [EnumMember(Value = "Finished C# Project")]
    CSharpFinished = Finished | CSharpProject,
    [EnumMember(Value = "Develop C# Project")]
    CSharpInDev = InDevelopment | CSharpProject,
    [EnumMember(Value = "Prototype C# Project")]
    CSharpPrototype = Prototype | CSharpProject,
}