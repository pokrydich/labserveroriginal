namespace LabServer.Server.Helpers;

using System.Text;

/// <summary>
/// Helper type for transliterating Cyrilic strings to latin alphabet
/// it is used to convert cyrilic strings into valid gitlab entity names
/// </summary>
public static class GitLabNameTransformer
{
    private static Dictionary<System.Char, System.String> _transliterationMap = new Dictionary<System.Char, System.String>
    {
        { 'а', "a" },
        { 'б', "b" },
        { 'в', "v" },
        { 'г', "g" },
        { 'д', "d" },
        { 'е', "e" },
        { 'ё', "e" },
        { 'ж', "zh" },
        { 'з', "z" },
        { 'и', "i" },
        { 'к', "k" },
        { 'л', "l" },
        { 'м', "m" },
        { 'н', "n" },
        { 'о', "o" },
        { 'п', "p" },
        { 'р', "r" },
        { 'с', "s" },
        { 'т', "t" },
        { 'у', "u" },
        { 'ф', "f" },
        { 'х', "h" },
        { 'ц', "ts" },
        { 'ч', "ch" },
        { 'ш', "sh" },
        { 'щ', "sh" },
        { 'ъ', "" },
        { 'ы', "i" },
        { 'э', "ie" },
        { 'ю', "u" },
        { 'я', "ia" },
        
        { 'a', "a" },
        { 'b', "b" },
        { 'c', "c" },
        { 'd', "d" },
        { 'e', "e" },
        { 'f', "f" },
        { 'g', "g" },
        { 'h', "h" },
        { 'i', "i" },
        { 'j', "j" },
        { 'k', "k" },
        { 'l', "l" },
        { 'm', "m" },
        { 'n', "n" },
        { 'o', "o" },
        { 'p', "p" },
        { 'q', "q" },
        { 'r', "r" },
        { 's', "s" },
        { 't', "t" },
        { 'u', "u" },
        { 'v', "v" },
        { 'w', "w" },
        { 'x', "x" },
        { 'y', "y" },
        { 'z', "z" },

        { '_', "_" },
        { ' ', "_" },
        { '/', "_" },
        { '0', "0" },
        { '1', "1" },
        { '2', "2" },
        { '3', "3" },
        { '4', "4" },
        { '5', "5" },
        { '6', "6" },
        { '7', "7" },
        { '8', "8" },
        { '9', "9" }
    };
    /// <summary>
    /// Transliterates given string (cyrilic -> latin)
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static System.String Transliterate(System.String str)
    {
        StringBuilder sb = new StringBuilder();
        foreach (System.Char c in str.ToLower())
        {
            if (_transliterationMap.ContainsKey(c))
                sb.Append(_transliterationMap[c]);
        }
        return sb.ToString();
    }

    private const System.Int32 cNameComponentsToUse = 3;
    private const System.Int32 cNameCompoenentsLength = 1;

    /// <summary>
    /// Transforms full name (LastName MiddleName FirstName into GitLab name)
    /// E.g.: Ivanov Ivan Ivanovich -> ivanov_i_i
    /// </summary>
    /// <param name="name">full name</param>
    /// <returns>valid GitLab name</returns>
    public static System.String UseranmeFromName(System.String name)
    {
        var split = name.ToLower().Split().Where(s => !System.String.IsNullOrWhiteSpace(s)).ToList();
        var forUsername = split.GetRange(0, Math.Min(split.Count(), cNameComponentsToUse)); // take at max 3 segments of given name to form gitlab usernmae
        StringBuilder sb = new StringBuilder();
        sb.Append(Transliterate(forUsername[0]));
        foreach (var component in forUsername.Skip(1))
        {
            sb.Append("_");
            sb.Append(Transliterate(component.Substring(0, cNameCompoenentsLength)));
        }
        return sb.ToString();
    }
}