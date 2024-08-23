using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GrailBot.Model;

public class MessageResponder(string prefix, Func<string, string?> function)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Prefix { get; set; } = prefix;
    public Func<string, string?> Function { get; set; } = function;
}
