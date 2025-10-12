namespace LabServer.Server.Service;

public interface IEmailSender
{
    System.Boolean Send(System.String targetEmail, System.String subject, System.String message);
}