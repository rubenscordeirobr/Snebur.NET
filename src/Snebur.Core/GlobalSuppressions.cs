using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", 
    Justification = "The 'Error' record is only used internally within this project and will not be exposed to other cultures.", 
    Scope = "member", 
    Target = "~P:Snebur.Core.IResultValue.Error")]

[assembly: SuppressMessage("Security", 
    "CA5351:Do Not Use Broken Cryptographic Algorithms", 
    Justification = "MD5 is used solely for file integrity verification when files are uploaded.", 
    Scope = "member",
    Target = "~M:Snebur.Core.Helpers.HashHelper.GenerateMd5GuidHash(System.Byte[])~System.Guid")]

[assembly: SuppressMessage("Naming",
    "CA1716:Identifiers should not match keywords", 
    Justification = "The 'Error' record is only used internally within this project and will not be exposed to other cultures.", Scope = "type", Target = "~T:Snebur.Core.Error")]

[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", 
    Justification = "This property is metadata only. ",
    Scope = "member",
    Target = "~P:Snebur.Core.Infos.PhoneNumberFormatInfo.AlternateNationalFormats")]

[assembly: SuppressMessage("Design", "CA1024:Use properties where appropriate", 
    Justification = "Property must not throw exception. Why this is method", 
    Scope = "member", 
    Target = "~M:Snebur.Core.Result`1.GetRequiredValue~`0")]

[assembly: SuppressMessage("Security", "CA5394:Do not use insecure randomness", 
    Justification = "<Pending>", 
    Scope = "member", Target = "~M:Snebur.Core.Utils.RandomUtils.GenerateRandomNumber(System.Int32)~System.String")]

[assembly: SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", 
    Justification = "",
    Scope = "member", 
    Target = "~F:Snebur.Core.Utils.ReflectionUtils.AllInstanceBindingFlags")]
