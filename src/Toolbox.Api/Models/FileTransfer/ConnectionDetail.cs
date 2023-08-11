namespace Toolbox.Api.Models.FileTransfer;
public class ConnectionDetail
{
    public int RoomId { get; set; }
    public ClientInfo Sender { get; set; }
    public ClientInfo? Receiver { get; set; }
    public DateTime ExpirationTime { get; set; }
    public ConnectionDetail(int roomId, ClientInfo sender, ClientInfo? receiver)
    {
        RoomId = roomId;
        Sender = sender;
        Receiver = receiver;
        //半小时强制过期
        ExpirationTime = DateTime.Now.AddMinutes(30);
    }
}

public class ClientInfo
{
    public string Id { get; set; }
    public string? SDPOffer { get; set; }
    public string? Candidate { get; set; }

    public ClientInfo(string id)
    {
        Id = id;
    }
}