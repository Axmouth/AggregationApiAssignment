using Newtonsoft.Json.Serialization;

namespace AggregationApiAssignment;

public class Util
{
    public static Octokit.Language LanguageFromString(string language)
    {
        return language switch
        {
            "C#" => Octokit.Language.CSharp,
            "JavaScript" => Octokit.Language.JavaScript,
            "Python" => Octokit.Language.Python,
            "Java" => Octokit.Language.Java,
            "C++" => Octokit.Language.CPlusPlus,
            "Go" => Octokit.Language.Go,
            "Rust" => Octokit.Language.Rust,
            "TypeScript" => Octokit.Language.TypeScript,
            "Ruby" => Octokit.Language.Ruby,
            "PHP" => Octokit.Language.Php,
            "Swift" => Octokit.Language.Swift,
            "Kotlin" => Octokit.Language.Kotlin,
            "Objective-C" => Octokit.Language.ObjectiveC,
            "Scala" => Octokit.Language.Scala,
            "Shell" => Octokit.Language.Shell,
            "PowerShell" => Octokit.Language.PowerShell,
            "R" => Octokit.Language.R,
            "Perl" => Octokit.Language.Perl,
            "Haskell" => Octokit.Language.Haskell,
            "Lua" => Octokit.Language.Lua,
            "Clojure" => Octokit.Language.Clojure,
            "Groovy" => Octokit.Language.Groovy,
            "Assembly" => Octokit.Language.Assembly,
            "Elixir" => Octokit.Language.Elixir,
            "Dart" => Octokit.Language.Dart,
            "Vim-script" => Octokit.Language.VimL,
            "Emacs-Lisp" => Octokit.Language.EmacsLisp,
            "TeX" => Octokit.Language.TeX,
            "VimL" => Octokit.Language.VimL,
            "Erlang" => Octokit.Language.Erlang,
            _ => Octokit.Language.Unknown
        };
    }

    // List of languages
    public static List<string> Languages = new List<string>
    {
        "C#",
        "JavaScript",
        "Python",
        "Java",
        "C++",
        "Go",
        "Rust",
        "TypeScript",
        "Ruby",
        "PHP",
        "Swift",
        "Kotlin",
        "Objective-C",
        "Scala",
        "Shell",
        "PowerShell",
        "R",
        "Perl",
        "Haskell",
        "Lua",
        "Clojure",
        "Groovy",
        "Assembly",
        "Elixir",
        "Dart",
        "Vim-script",
        "Emacs-Lisp",
        "TeX",
        "VimL",
        "Erlang"
    };
}

public class CaseInsensitiveContractResolver : DefaultContractResolver
{
    protected override string ResolvePropertyName(string propertyName)
    {
        return base.ResolvePropertyName(propertyName.ToLowerInvariant()); // Convert property name to lowercase for case-insensitive comparison
    }
}
