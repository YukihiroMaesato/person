using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;
using System.Net.WebSockets;

namespace WebSocketServer
{
    /// <summary>
    /// webHandler の概要の説明です
    /// </summary>
    public class webHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(RepeatWebSocket);
            }
            

        }

        public async Task RepeatWebSocket(AspNetWebSocketContext context)
        {

            WebSocket socket = context.WebSocket;
            while (true)
            {
                if (socket.State == WebSocketState.Open)
                {
                    string returnMessage = string.Format("{0}:こんにちは金城さん", DateTime.Now);
                    //レスポンス処理　
                    ArraySegment<byte> returnBuffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(returnMessage));

                    System.Threading.Thread.Sleep(3000);
                    await socket.SendAsync(returnBuffer, WebSocketMessageType.Text,true, System.Threading.CancellationToken.None);
                }
                else
                {
                    break;
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}