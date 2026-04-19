using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Design",
    "CA1003:Use generic event handler instances",
    Justification = "In Blazor, the event handler is not generic, so we use a non-generic delegate.",
    Scope = "module")]

