namespace LabServer.Server.Helpers;

/// <summary>
/// Helper type for generating random secrets
/// </summary>
public static class RandomUtils
{
    private static System.String _passwordAlphabet = "abcdefghijklmnopqrstuvwxyz01234567890@!#$%^&_";
    private static System.String _tokenAlphabet = "abcdefghijklmnopqrstuvwxyz01234567890";
    /// <summary>
    /// Use windows current file timestamp as random seed
    /// </summary>
    private static System.Int32 RandomSeed => (System.Int32)DateTime.Now.ToFileTime();

    /// <summary>
    /// Generate random password
    /// </summary>
    /// <param name="length">password length</param>
    /// <returns>random password</returns>
    public static System.String GetPassword(System.Int32 length)
    {
        var RNG = new Random(RandomSeed);
        return System.String.Join("", Enumerable.Range(0, length).Select(_ => _passwordAlphabet[RNG.Next(0, _passwordAlphabet.Length)]));
    }
    /// <summary>
    /// Generate random access token
    /// </summary>
    /// <param name="length">token length</param>
    /// <returns>random token</returns>
    public static System.String GetToken(System.Int32 length)
    {
        var RNG = new Random(RandomSeed);
        return System.String.Join("", Enumerable.Range(0, length).Select(_ => _tokenAlphabet[RNG.Next(0, _tokenAlphabet.Length)]));
    }
}