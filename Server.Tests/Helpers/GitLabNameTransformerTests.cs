namespace LabServer.Server.Tests.Helpers;

using LabServer.Server.Helpers;

public class GitLabNameTransformerTests
{
    [Fact]
    public void Transliterate_ConvertsCyrillicCharacters()
    {
        var result = GitLabNameTransformer.Transliterate("Тестовая Строка 123");
        Assert.Equal("testovaia_stroka_123", result);
    }

    [Fact]
    public void UserNameFromName_UsesAtMostThreeComponents()
    {
        var username = GitLabNameTransformer.UseranmeFromName("Иванов Иван Иванович Петрович");
        Assert.Equal("ivanov_i_i", username);
    }

    [Fact]
    public void UserNameFromName_IgnoresUnknownSymbols()
    {
        var username = GitLabNameTransformer.UseranmeFromName("Иванов * Иван !");
        Assert.Equal("ivanov__i", username);
    }
}
