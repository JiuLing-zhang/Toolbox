using System.ComponentModel.DataAnnotations;

namespace Toolbox.Api.Models.Request;
public class SignRequest
{
    [Required(ErrorMessage = "时间戳不能为空")]
    public long Timestamp { get; set; }

    [Required(ErrorMessage = "签名不能为空")]
    public string Sign { get; set; } = null!;

}