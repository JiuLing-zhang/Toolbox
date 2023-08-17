using Toolbox.Pages.Enums;

namespace Toolbox.Pages.Models;
internal class FileTransferInfo : FileMetadata
{
    public byte[] FileContext { get; set; }
    public FileTransferStateEnum State { get; set; }
    public double Progress { get; set; }
}

internal class FileMetadata
{
    public string FileName { get; set; } = null!;
    public string SHA1 { get; set; } = null!;
    public int FileSize { get; set; }
}