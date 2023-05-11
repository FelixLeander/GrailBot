namespace GrailBot.Model;

public class ContentRegister
{
    public int Id { get; set; }
    public required string Command { get; set; }
    public required string Content { get; set; }
}
