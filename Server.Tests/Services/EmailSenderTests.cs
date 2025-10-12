namespace LabServer.Server.Tests.Services;

using System.Net.Mail;
using LabServer.Server.Service;

public class EmailSenderTests
{
    private static IConfiguration BuildConfiguration(IDictionary<string, string?> values)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(values!)
            .Build();

    [Fact]
    public void Constructor_UsesConfiguredDefaults()
    {
        var config = BuildConfiguration(new Dictionary<string, string?>
        {
            ["SMTP:email"] = "from@test.dev",
            ["SMTP:domain"] = "smtp.test.dev",
            ["SMTP:username"] = "user",
            ["SMTP:password"] = "pwd",
            ["SMTP:ssl"] = "false",
            ["SMTP:port"] = "2525"
        });

        var sender = new EmailSender(config);
        var fromField = typeof(EmailSender).GetField("_fromAddress", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.Equal("from@test.dev", fromField?.GetValue(sender));
    }

    [Fact]
    public void Send_ReturnsFalse_WhenSmtpFails()
    {
        var config = BuildConfiguration(new Dictionary<string, string?>
        {
            ["SMTP:email"] = "from@test.dev",
            ["SMTP:domain"] = "127.0.0.1", // assume no SMTP on loopback
            ["SMTP:username"] = "user",
            ["SMTP:password"] = "pwd",
            ["SMTP:ssl"] = "false",
            ["SMTP:port"] = "2526"
        });

        var sender = new EmailSender(config);
        var result = sender.Send("to@test.dev", "Subject", "Body");

        Assert.False(result);
    }
}
