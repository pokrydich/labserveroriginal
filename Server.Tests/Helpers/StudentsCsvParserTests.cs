namespace LabServer.Server.Tests.Helpers;

using CsvHelper;
using LabServer.Server.Helpers;

public class StudentsCsvParserTests
{
    [Fact]
    public void GuessDelimiter_ReturnsExpectedCharacters()
    {
        var delimiterEmailFirst = StudentsCsvParser.GuessDelimiter("user1@mail.com;Some User Name\nuser2@mail.com;Some User2 Name");
        Assert.Equal(";", delimiterEmailFirst);

        var delimiterEmailLast = StudentsCsvParser.GuessDelimiter("Some User Name;user1@mail.com\nSome User2 Name;user2@mail.com");
        Assert.Equal(";", delimiterEmailLast);
    }

    [Fact]
    public void GuessDelimiter_ReturnsNull_WhenInputHasSingleEmail()
    {
        var delimiter = StudentsCsvParser.GuessDelimiter("single@email.test");
        Assert.Null(delimiter);
    }

    [Theory]
    [InlineData("u1@mail.com,User User1\nu2@mail.com,User User2\nu3@mail.com,User User3")]
    [InlineData("User User1,u1@mail.com\nUser User2,u2@mail.com\nUser User3,u3@mail.com")]
    public void Parse_ProducesRecordsRegardlessOfColumnOrder(string rawCsv)
    {
        var parsed = StudentsCsvParser.Parse(rawCsv);
        Assert.Equal(3, parsed.Count);
        Assert.Collection(parsed,
            s => Assert.Equal(("u1@mail.com", "User User1"), (s.Email, s.Name)),
            s => Assert.Equal(("u2@mail.com", "User User2"), (s.Email, s.Name)),
            s => Assert.Equal(("u3@mail.com", "User User3"), (s.Email, s.Name)));
    }

    [Fact]
    public void Parse_Throws_WhenEmailColumnMissing()
    {
        var raw = "Value1,Value2\nAnother,Row";
        Assert.Throws<MissingFieldException>(() => StudentsCsvParser.Parse(raw));
    }
}
