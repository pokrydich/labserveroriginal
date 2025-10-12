namespace LabServer.Server.Tests.Helpers;

using LabServer.Server.Helpers;

public class RandomUtilsTests
{
    private const string PasswordAlphabet = "abcdefghijklmnopqrstuvwxyz01234567890@!#$%^&_";
    private const string TokenAlphabet = "abcdefghijklmnopqrstuvwxyz01234567890";

    [Fact]
    public void GetPassword_ReturnsExpectedLengthAndAlphabet()
    {
        var password = RandomUtils.GetPassword(16);
        Assert.Equal(16, password.Length);
        Assert.All(password, c => Assert.Contains(c, PasswordAlphabet));
    }

    [Fact]
    public void GetToken_ReturnsExpectedLengthAndAlphabet()
    {
        var token = RandomUtils.GetToken(32);
        Assert.Equal(32, token.Length);
        Assert.All(token, c => Assert.Contains(c, TokenAlphabet));
    }

    [Fact]
    public void RandomGenerators_HandleZeroLength()
    {
        Assert.Equal(string.Empty, RandomUtils.GetPassword(0));
        Assert.Equal(string.Empty, RandomUtils.GetToken(0));
    }
}
