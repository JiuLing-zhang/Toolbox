namespace Toolbox.Api.Models.FileTransfer;
internal class ConnectionDetail
{
    public int RoomId { get; set; }
    public SenderInfo Sender { get; set; }
    public ReceiverInfo? Receiver { get; set; }
    public DateTime ExpirationTime { get; set; }
    public ConnectionDetail(int roomId, SenderInfo sender, ReceiverInfo? receiver)
    {
        RoomId = roomId;
        Sender = sender;
        Receiver = receiver;
        //半小时强制过期
        ExpirationTime = DateTime.Now.AddMinutes(30);
    }
}

internal class SenderInfo
{
    public string Id { get; set; }
    public string? Offer { get; set; }
    public string? Candidate { get; set; }
    public SenderInfo(string id)
    {
        Id = id;
    }
}

internal class ReceiverInfo
{
    public string Id { get; set; }
    public string? Answer { get; set; }
    public string? Candidate { get; set; }
    public ReceiverInfo(string id)
    {
        Id = id;
    }
}