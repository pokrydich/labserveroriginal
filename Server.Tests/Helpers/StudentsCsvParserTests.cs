namespace LabServer.Server.Helpers;

public class StudentsCsvParserTests
{
    [Fact]
    public void TestDelimiterGuess()
    {
        var delimiterEmailFirst = StudentsCsvParser.GuessDelimiter("user1@mail.com;Some User Name\nuser2@mail.com;Some User2 Name");
        Assert.Equal(";", delimiterEmailFirst);

        var delimiterEmailLast = StudentsCsvParser.GuessDelimiter("Some User Name;user1@mail.com\nSome User2 Name;nuser2@mail.com");
        Assert.Equal(";", delimiterEmailLast);
    }

    [Theory]
    [InlineData("u1@mail.com,User User1\nu2@mail.com,User User2\nu3@mail.com,User User3")]
    [InlineData("User User1,u1@mail.com\nUser User2,u2@mail.com\nUser User3,u3@mail.com")]
    public void TestCsvParsingFirstColumnEmail(System.String rawCsv)
    {
        var parsed = StudentsCsvParser.Parse(rawCsv);
        Assert.Single(parsed, s => s.Email == "u1@mail.com" && s.Name == "User User1");
        Assert.Single(parsed, s => s.Email == "u2@mail.com" && s.Name == "User User2");
        Assert.Single(parsed, s => s.Email == "u3@mail.com" && s.Name == "User User3");
    }
}