## 1. 在线工具箱  
* 基于 `Blazor WebAssembly` 构建  
* 基于 `MAUI Blazor` 构建  
* 基于 [`MudBlazor`](https://github.com/MudBlazor/MudBlazor/) 组件开发  
* 支持 `PWA`  


## 2. 功能模块  

* 软件管理
* `AI` 聊天
* 哈希工具
* 文件分析
* 文件传输

### 2.1. 软件管理  
用来管理发布和下载常用工具或包资源  
<img src="https://github.com/JiuLing-zhang/Toolbox/raw/main/resources/images/app.png" width="80%">

### 2.2. `AI` 聊天  

* `ChatGPT`  
* 支持 `gpt-3.5-turbo`、`text-davinci-003` 两种模型
* 支持流式返回  

<img src="https://github.com/JiuLing-zhang/Toolbox/raw/main/resources/images/chat.png" width="80%">

### 2.3. 哈希工具  
计算文件、文本的 `MD5`、`SHA1`、`SHA256`  
<img src="https://github.com/JiuLing-zhang/Toolbox/raw/main/resources/images/hash-check.png" width="80%">

### 2.4. 文件分析  

通过 [`VirusTotal API`](https://www.virustotal.com/) 提供在线文件安全扫描功能。  
<img src="https://github.com/JiuLing-zhang/Toolbox/raw/main/resources/images/security-analysis.png" width="80%">

### 2.5. 文件传输  

一个点对点的文件发送模块  
> 优先通过 `WebRTC stun` 实现点对点打洞传输，如果连接无法建立，则使用 `SignalR` 通过服务器中转发送。  

<img src="https://github.com/JiuLing-zhang/Toolbox/raw/main/resources/images/file-transfer.png" width="80%">

## 3. 代码说明  
* `ToolboxApi.sln`: 后台 `API` 接口项目
* `ToolboxApp.sln`: 基于 `MAUI Blazor` 的客户端项目
* `ToolboxWeb.sln`: 基于 `Blazor` 构建的 `Web` 端