let dotNetHelper;
let stunServer;
let localConnection, remoteConnection;
let localDataChannel, remoteDataChannel;

async function initialization(dotNetHelperValue, stunServerValue) {
    dotNetHelper = dotNetHelperValue;
    stunServer = stunServerValue;
}

// 在客户端 A 中执行
async function createSenderConnection() {
    const config = { iceServers: [{ urls: stunServer }] };
    localConnection = new RTCPeerConnection(config);
    // 创建数据通道
    let dataChannelOptions = {
        ordered: true, //保证到达顺序
    };
    localDataChannel = localConnection.createDataChannel("dataChannel", dataChannelOptions);
    localDataChannel.onopen = dataChannelStateChange;
    localDataChannel.onclose = dataChannelStateChange;

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

function dataChannelStateChange() {
    if (localDataChannel.readyState === 'open') {
        dotNetHelper.invokeMethodAsync('SenderConnected');
    }
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

    const config = { iceServers: [{ urls: stunServer }] };
    remoteConnection = new RTCPeerConnection(config);

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
    dotNetHelper.invokeMethodAsync('ReceiverConnected');
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
            let sha1 = GetFileSHA1(new Uint8Array(receivedByteArray));
            dotNetHelper.invokeMethodAsync('FileReceivedWithWebRTC', sha1);
        }

    } else {
        // 将接收到的数据追加到字节数组中
        receivedByteArray = [...receivedByteArray, ...new Uint8Array(receivedData)];
        dotNetHelper.invokeMethodAsync('FileReceivingWithWebRTC', receivedByteArray.length);
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
const SEND_INTERVAL = 20; // 每个数据块发送间隔（毫秒）
function sendFileDataChunks(byteArray) {
    // 发送文件数据块
    const chunk = byteArray.slice(0, CHUNK_SIZE);
    localDataChannel.send(chunk);
    // 删除已发送的数据块
    byteArray = byteArray.slice(CHUNK_SIZE);
    dotNetHelper.invokeMethodAsync('FileSending', byteArray.length);

    if (byteArray.length > 0) {
        setTimeout(() => {
            sendFileDataChunks(byteArray);
        }, SEND_INTERVAL);
    } else {
        // 文件数据已发送完成
        localDataChannel.send(fileSent);
        dotNetHelper.invokeMethodAsync('FileSent');
    }
}

function saveByteArrayToFile(fileName) {
    saveToFileWithBufferAndName(fileName, new Uint8Array(receivedByteArray))
}

function saveToFileWithBufferAndName(fileName, buffer) {
    const blob = new Blob([buffer], { type: 'application/octet-stream' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = fileName;
    link.click();
    URL.revokeObjectURL(link.href);
}