using System.ComponentModel.DataAnnotations;

namespace Toolbox.Api.Models.Request;
public class ChatGPTRequest
{
    [Required(ErrorMessage = "内容不能为空")]
    public string Prompt { get; set; } = null!;
}