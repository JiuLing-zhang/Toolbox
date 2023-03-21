using System.ComponentModel.DataAnnotations;
using Toolbox.Api.Enums;

namespace Toolbox.Api.Models.Request;
public class ChatGPTRequest
{
    [Required(ErrorMessage = "内容不能为空")]
    public string Prompt { get; set; } = null!;

    public ChatTypeEnum ChatType { get; set; }
}