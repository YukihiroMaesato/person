using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;
using System.Net.WebSockets;
using System.Text.Json;

namespace WebSocketServer
{
    /// <summary>
    /// ResponseHandler の概要の説明です
    /// </summary>
    public class ResponseHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(RepeatCloseWebSocket);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public async Task RepeatCloseWebSocket(AspNetWebSocketContext context)
        {
            WebSocket socket = context.WebSocket;
            while (true)
            {
                if (socket.State == WebSocketState.Open)
                {
                    //string returnMessage = string.Format("{0}:やあ、こんにちは", DateTime.Now);
                    //ArraySegment<byte> returnBuffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(returnMessage));
                    //await socket.SendAsync(returnBuffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);

                    ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(new byte[1024]);
                    //クライアントからリクエストされるまで待機状態
                    WebSocketReceiveResult result = await socket.ReceiveAsync(receiveBuffer, System.Threading.CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        byte[] receive_byte = receiveBuffer.Take(result.Count).ToArray();
                        string receiveText = System.Text.Encoding.UTF8.GetString(receive_byte);

                        if (receiveText[0] == 'C')
                        {
                            break;
                        }

                        string returnMessage = String.Format("{0} 　サーバーからのレスポンス成功", receiveText);
                        Console.WriteLine("test");
                        ArraySegment<byte> returnBuffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(returnMessage));
                        await socket.SendAsync(returnBuffer, WebSocketMessageType.Text, true, System.Threading.CancellationToken.None);

                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}