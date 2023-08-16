using JiuLing.CommonLibs.ExtensionMethods;
using Microsoft.AspNetCore.SignalR;
using System;
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
    public Task<string> CreateRoomAsync(int roomId)
    {
        lock (Locker)
        {
            //删除过期的房间
            var time = DateTime.Now;
            Connections.RemoveAll(x => x.ExpirationTime.Subtract(time).TotalSeconds < 0);

            if (Connections.Any(x => x.RoomId == roomId))
            {
                return Task.FromResult((new ApiResponse(1, "房间号已存在")).ToJson());
            }
            var sender = new SenderInfo(Context.ConnectionId);
            Connections.Add(new ConnectionDetail(roomId, sender, null));
        }
        return Task.FromResult((new ApiResponse(0, "创建成功")).ToJson());
    }

    [HubMethodName("JoinRoom")]
    public async Task<string> JoinRoomAsync(int roomId)
    {
        string senderId = "";
        lock (Locker)
        {
            var connection = Connections.FirstOrDefault(x => x.RoomId == roomId);
            if (connection == null)
            {
                return (new ApiResponse(1, "房间号不存在")).ToJson();
            }
            if (connection.Receiver != null)
            {
                return (new ApiResponse(2, "房间已被其它用户抢占")).ToJson();
            }
            senderId = connection.Sender.Id;
            var receiver = new ReceiverInfo(Context.ConnectionId);
            Connections.First(x => x.RoomId == roomId).Receiver = receiver;
        }
        await Clients.Client(senderId).SendAsync("ReceiverJoin");
        return (new ApiResponse(0, "进入成功")).ToJson();
    }

    [HubMethodName("SendSenderIceCandidate")]
    public async Task<string> SendSenderIceCandidateAsync(int roomId, string candidate)
    {
        string receiverId;
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
            if (connection.Sender.Candidate != null)
            {
                return (new ApiResponse(3, "链接已被创建")).ToJson();
            }
            if (connection.Receiver == null)
            {
                return (new ApiResponse(4, "接收方未初始化完成")).ToJson();
            }
            receiverId = connection.Receiver.Id;
            connection.Sender.Candidate = candidate;
        }
        await Clients.Client(receiverId).SendAsync("ReceiveSenderIceCandidate", candidate);
        return (new ApiResponse(0, "成功")).ToJson();
    }

    [HubMethodName("SendReceiverIceCandidate")]
    public async Task<string> SendReceiverIceCandidateAsync(int roomId, string candidate)
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
            if (connection.Receiver.Candidate != null)
            {
                return (new ApiResponse(4, "链接已被创建")).ToJson();
            }
            connection.Receiver.Candidate = candidate;
            senderId = connection.Sender.Id;
        }
        await Clients.Client(senderId).SendAsync("ReceiveReceiverIceCandidate", candidate);
        return (new ApiResponse(0, "成功")).ToJson();
    }

    [HubMethodName("SendOffer")]
    public async Task<string> SendOfferAsync(int roomId, string offer)
    {
        string receiverId;
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
            if (connection.Sender.Offer != null)
            {
                return (new ApiResponse(3, "不允许重复建立链路")).ToJson();
            }
            if (connection.Receiver == null)
            {
                return (new ApiResponse(4, "接收方未初始化完成")).ToJson();
            }
            receiverId = connection.Receiver.Id;
            connection.Sender.Offer = offer;
        }
        await Clients.Client(receiverId).SendAsync("ReceiveOffer", offer);
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
            if (connection.Receiver.Answer != null)
            {
                return (new ApiResponse(4, "链接已被创建")).ToJson();
            }
            connection.Receiver.Answer = answer;
            senderId = connection.Sender.Id;
        }
        await Clients.Client(senderId).SendAsync("ReceiveAnswer", answer);
        return (new ApiResponse(0, "成功")).ToJson();
    }

    [HubMethodName("SwitchConnectionType")]
    public async Task<string> SwitchConnectionTypeAsync(int roomId)
    {
        string receiverId;
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
            if (connection.Receiver == null)
            {
                return (new ApiResponse(3, "接收方未初始化完成")).ToJson();
            }
            receiverId = connection.Receiver.Id;
        }
        await Clients.Client(receiverId).SendAsync("ReceiveSwitchConnectionType");
        return (new ApiResponse(0, "成功")).ToJson();
    }

    [HubMethodName("SendFileInfo")]
    public async Task SendFileInfoAsync(string fileInfo)
    {
        string receiverId = Connections.FirstOrDefault(x => x.Sender.Id == Context.ConnectionId).Receiver?.Id ?? "";
        if (receiverId.IsNotEmpty())
        {
            await Clients.Client(receiverId).SendAsync("ReceiveFileInfo", fileInfo);
        }
    }

    [HubMethodName("SendFile")]
    public async Task SendFileAsync(byte[] buffer)
    {
        string receiverId = Connections.FirstOrDefault(x => x.Sender.Id == Context.ConnectionId).Receiver?.Id ?? "";
        if (receiverId.IsNotEmpty())
        {
            await Clients.Client(receiverId).SendAsync("ReceiveFile", buffer);
        }
    }

    [HubMethodName("SendFileSent")]
    public async Task SendFileSentAsync()
    {
        string receiverId = Connections.FirstOrDefault(x => x.Sender.Id == Context.ConnectionId).Receiver?.Id ?? "";
        if (receiverId.IsNotEmpty())
        {
            await Clients.Client(receiverId).SendAsync("ReceiveFileSent");
        }
    }
     
}