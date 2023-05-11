using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrailBot.Model;

public class WordCount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Message { get; set; }
    public int Amount { get; set; }

    public WordCount(string message)
    {
        Message = message;
    }
}
