using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GrailBot.Model;

public class MessageResponder
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Prefix { get; set; }
    public Func<string, string?> Function { get; set; }

    public MessageResponder(string prefix, Func<string, string?> function)
    {
        Prefix = prefix;
        Function = function;
    }
}
