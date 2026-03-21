 
[assembly: SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed",
    Justification = "Private setters and members are used by EF Core via reflection.",
    Scope = "module")]