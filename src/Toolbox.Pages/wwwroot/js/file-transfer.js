let dotNetHelper;
let localConnection, remoteConnection;
let localDataChannel, remoteDataChannel;

async function initialization(dotNetHelperValue) {
    dotNetHelper = dotNetHelperValue;
}

// 在客户端 A 中执行
async function createSenderConnection() {
    localConnection = new RTCPeerConnection();
    // 创建数据通道
    localDataChannel = localConnection.createDataChannel("dataChannel");
    // 监听 ICE candidate 事件
    localConnection.onicecandidate = event => {
        if (event.candidate) {
            // 发送本地 ICE candidate 到信令服务器
            dotNetHelper.invokeMethodAsync('SendIceCandidateToServer', JSON.stringify(event.candidate));
        }
    }

    // 创建 SDP Offer
    const offer = await localConnection.createOffer();
    await localConnection.setLocalDescription(offer);

    // 发送 SDP Offer 到信令服务器 
    dotNetHelper.invokeMethodAsync('SendOfferToServer', JSON.stringify(offer));
}

// 客户端 A 使用接收到的 SDP answer 设置远程描述
async function receiveAnswer(answer) {

    const answerObj = JSON.parse(answer);
    await localConnection.setRemoteDescription(
        {
            type: answerObj.type,
            sdp: answerObj.sdp
        });
}

// 在客户端 B 中执行
async function createReceiverConnection(offer) {

    remoteConnection = new RTCPeerConnection();

    // 监听 ICE candidate 事件
    remoteConnection.onicecandidate = event => {
        if (event.candidate) {
            // 发送本地 ICE candidate 到信令服务器
            dotNetHelper.invokeMethodAsync('SendIceCandidateToServer', JSON.stringify(event.candidate));
        }
    }
    remoteConnection.ondatachannel = event => {

        // 设置数据通道的事件处理程序
        event.channel.onopen = handleDataChannelOpen;
        event.channel.onmessage = receiveFileData;
    };
    // 设置远程 SDP
    const offerObj = JSON.parse(offer);
    await remoteConnection.setRemoteDescription(
        {
            type: offerObj.type,
            sdp: offerObj.sdp
        });

    // 创建 SDP Answer
    const answer = await remoteConnection.createAnswer();
    await remoteConnection.setLocalDescription(answer);

    // 发送 SDP Answer 到信令服务器
    dotNetHelper.invokeMethodAsync('SendAnswerToServer', JSON.stringify(answer));
}

// 在客户端 A 和 B 中都执行
function receiveIceCandidate(candidate) {
    // 在双方连接中添加远程 ICE candidate
    const candidateObj = JSON.parse(candidate);
    const iceCandidate = new RTCIceCandidate(
        {
            candidate: candidateObj.candidate,
            sdpMid: candidateObj.sdpMid,
            sdpMLineIndex: candidateObj.sdpMLineIndex
        });
    if (localConnection) {
        localConnection.addIceCandidate(iceCandidate);
    } else if (remoteConnection) {
        remoteConnection.addIceCandidate(iceCandidate);
    }
}

function handleDataChannelOpen() {
    console.log("Data channel opened.");
}

let readyToSendKey = "ReadyToSend";
let fileSent = "FileSent";
let receivedByteArray = [];
function receiveFileData(event) {

    const receivedData = event.data;
    if (typeof receivedData === 'string') {

        if (receivedData.indexOf(readyToSendKey) == 0) {
            let fileInfo = receivedData.substring(readyToSendKey.length);
            //收到元数据
            dotNetHelper.invokeMethodAsync('FileInfoReceived', fileInfo);
        } else if (receivedData == fileSent) {
            // 在 receivedByteArray 中就是完整文件的字节数组
            // 可以将 receivedByteArray 传递给 Blazor 或进行其他处理
            let sha1 = GetFileSHA1(new Uint8Array(receivedByteArray));
            dotNetHelper.invokeMethodAsync('FileReceived', sha1);
        }

    } else {
        // 将接收到的数据追加到字节数组中
        receivedByteArray = [...receivedByteArray, ...new Uint8Array(receivedData)];
        dotNetHelper.invokeMethodAsync('FileReceiving', receivedByteArray.length);
    }
}

function sendFileInfo(fileInfo) {
    //发送文件元数据
    localDataChannel.send(readyToSendKey + fileInfo);
}

function sendFile(fileArray) {
    sendFileDataChunks(fileArray);
}

const CHUNK_SIZE = 16384; // 每个数据块的大小
const SEND_INTERVAL = 50; // 每个数据块发送间隔（毫秒）

// 发送文件数据块
let sending = false;

function sendFileDataChunks(byteArray) {
    if (!sending) {
        sending = true;
        // 发送文件数据块
        const chunk = byteArray.slice(0, CHUNK_SIZE);
        localDataChannel.send(chunk);

        // 删除已发送的数据块
        byteArray = byteArray.slice(CHUNK_SIZE);


        if (byteArray.length > 0) {
            setTimeout(() => {
                sending = false;
                sendFileDataChunks(byteArray);
            }, SEND_INTERVAL);
        } else {
            // 文件数据已发送完成
            localDataChannel.send(fileSent);
        }
    }
}

function saveByteArrayToFile(fileName) {
    const blob = new Blob([new Uint8Array(receivedByteArray)], { type: 'application/octet-stream' });

    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(link.href);
}