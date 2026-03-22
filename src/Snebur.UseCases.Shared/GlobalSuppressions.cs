using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Info Code Smell",
    "S1133:Deprecated code should be removed", 
    Justification = "The obsolete attribute is used to avoid obsolete validation from FluentValidation ",
    Scope = "module")]
