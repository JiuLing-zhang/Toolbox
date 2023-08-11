using JiuLing.CommonLibs.ExtensionMethods;
using Microsoft.AspNetCore.SignalR;
using Toolbox.Api.Models;
using Toolbox.Api.Models.FileTransfer;

namespace Toolbox.Api.Hubs;
public class FileTransferHub : Hub
{
    private static readonly object Locker = new();
    private static readonly List<ConnectionDetail> Connections = new();
    public FileTransferHub()
    {

    }

    [HubMethodName("CreateRoom")]
    public async Task<string> CreateRoomAsync(int roomId)
    {
        var sender = new ClientInfo(Context.ConnectionId);
        lock (Locker)
        {
            //删除过期的房间
            var time = DateTime.Now;
            Connections.RemoveAll(x => x.ExpirationTime.Subtract(time).TotalSeconds < 0);

            if (Connections.Any(x => x.RoomId == roomId))
            {
                return (new ApiResponse(1, "房间号已存在")).ToJson();
            }
            Connections.Add(new ConnectionDetail(roomId, sender, null));
        }
        return (new ApiResponse(0, "创建成功")).ToJson();
    }

    [HubMethodName("JoinRoom")]
    public async Task<string> JoinRoomAsync(int roomId)
    {
        string candidate;
        string senderOffer;
        var receiver = new ClientInfo(Context.ConnectionId);
        lock (Locker)
        {
            var connection = Connections.FirstOrDefault(x => x.RoomId == roomId);
            if (connection == null)
            {
                return (new ApiResponse(1, "房间号不存在")).ToJson();
            }
            if (connection.Sender.Candidate == null)
            {
                return (new ApiResponse(2, "房间未创建完成(Candidate)")).ToJson();
            }
            candidate = connection.Sender.Candidate;
            if (connection.Sender.SDPOffer == null)
            {
                return (new ApiResponse(3, "房间未创建完成")).ToJson();
            }
            senderOffer = connection.Sender.SDPOffer;

            if (connection.Receiver != null)
            {
                return (new ApiResponse(4, "房间已被其它用户抢占")).ToJson();
            }

            Connections.First(x => x.RoomId == roomId).Receiver = receiver;
        }
        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveOffer", senderOffer);
        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveIceCandidate", candidate);

        return (new ApiResponse(0, "进入成功")).ToJson();
    }

    [HubMethodName("SendIceCandidate")]
    public async Task<string> SendIceCandidateAsync(int roomId, int roomType, string candidate)
    {
        string anotherId = "";
        lock (Locker)
        {
            var connection = Connections.FirstOrDefault(x => x.RoomId == roomId);
            if (connection == null)
            {
                return (new ApiResponse(1, "房间号不存在")).ToJson();
            }

            if (roomType == 1)
            {
                //发送者
                if (connection.Sender.Id != Context.ConnectionId)
                {
                    return (new ApiResponse(2, "链接标识校验失败")).ToJson();
                }
                if (connection.Sender.Candidate != null)
                {
                    return (new ApiResponse(3, "链接已被创建")).ToJson();
                }
                connection.Sender.Candidate = candidate;
            }
            else if (roomType == 0)
            {
                //接收者
                if (connection.Receiver == null)
                {
                    return (new ApiResponse(4, "请先加入房间")).ToJson();
                }
                if (connection.Receiver.Id != Context.ConnectionId)
                {
                    return (new ApiResponse(5, "链接标识校验失败")).ToJson();
                }
                if (connection.Receiver.Candidate != null)
                {
                    return (new ApiResponse(6, "链接已被创建")).ToJson();
                }
                connection.Receiver.Candidate = candidate;
                anotherId = connection.Sender.Id;
            }
        }

        if (anotherId.IsNotEmpty())
        {
            await Clients.Client(anotherId).SendAsync("ReceiveIceCandidate", candidate);
        }
        return (new ApiResponse(0, "成功")).ToJson();
    }

    [HubMethodName("SendOffer")]
    public async Task<string> SendOfferAsync(int roomId, string offer)
    {
        lock (Locker)
        {
            var connection = Connections.FirstOrDefault(x => x.RoomId == roomId);
            if (connection == null)
            {
                return (new ApiResponse(1, "房间号不存在")).ToJson();
            }
            if (connection.Sender.Id != Context.ConnectionId)
            {
                return (new ApiResponse(2, "链接标识校验失败")).ToJson();
            }
            if (connection.Sender.SDPOffer != null)
            {
                return (new ApiResponse(3, "链接已被创建")).ToJson();
            }
            connection.Sender.SDPOffer = offer;
        }
        return (new ApiResponse(0, "成功")).ToJson();
    }

    [HubMethodName("SendAnswer")]
    public async Task<string> SendAnswerAsync(int roomId, string answer)
    {
        string senderId = "";
        lock (Locker)
        {
            var connection = Connections.FirstOrDefault(x => x.RoomId == roomId);
            if (connection == null)
            {
                return (new ApiResponse(1, "房间号不存在")).ToJson();
            }

            if (connection.Receiver == null)
            {
                return (new ApiResponse(2, "请先加入房间")).ToJson();
            }
            if (connection.Receiver.Id != Context.ConnectionId)
            {
                return (new ApiResponse(3, "链接标识校验失败")).ToJson();
            }
            if (connection.Receiver.SDPOffer != null)
            {
                return (new ApiResponse(4, "链接已被创建")).ToJson();
            }
            connection.Receiver.SDPOffer = answer;
            senderId = connection.Sender.Id;
        }
        await Clients.Client(senderId).SendAsync("ReceiveAnswer", answer);
        return (new ApiResponse(0, "成功")).ToJson();
    }

}