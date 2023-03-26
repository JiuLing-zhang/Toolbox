using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Toolbox.Api.Entities;
[Table("ConfigBase")]
public class ConfigBase
{
    [Key]
    public string ConfigKey { get; set; } = null!;

    public string ConfigDetail { get; set; } = null!;
}