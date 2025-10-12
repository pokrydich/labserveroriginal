namespace GitLab.Models.Note;

using System.Text.Json.Serialization;

public class CreateNoteRequest
{
    [JsonPropertyName("body")]
    public System.String Body { get; set; }
}