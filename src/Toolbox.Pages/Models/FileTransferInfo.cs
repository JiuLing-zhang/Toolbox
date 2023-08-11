namespace Toolbox.Pages.Models;
internal class FileTransferInfo
{
    public string FileName { get; set; } = null!;
    public string SHA1 { get; set; } = null!;
    public int FileSize { get; set; }
}