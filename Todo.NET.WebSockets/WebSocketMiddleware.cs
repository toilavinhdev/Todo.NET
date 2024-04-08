using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;

namespace Todo.NET.WebSockets;

public class WebSocketMiddleware(RequestDelegate next, WebSocketHandler webSocketHandler)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            await next(context);
            return;
        }

        var socket = await context.WebSockets.AcceptWebSocketAsync();
        var socketId = Guid.NewGuid().ToString();

        await webSocketHandler.OnConnected(socketId, socket);
        await webSocketHandler.SendMessageAsync(socketId, socketId);
        
        await Receive(socket, HandleMessage);
        return;

        async void HandleMessage(WebSocketReceiveResult result, byte[] buffer)
        {
            switch (result.MessageType)
            {
                case WebSocketMessageType.Text:
                    await webSocketHandler.ReceiveAsync(socket, result, buffer);
                    break;
                case WebSocketMessageType.Close:
                    await webSocketHandler.OnDisconnected(socketId, result);
                    break;
                case WebSocketMessageType.Binary:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(result.MessageType.ToString());
            }
        }

    }

    private static async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];
        
        // While Open WebSocket Connect
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(
                buffer: new ArraySegment<byte>(buffer),
                cancellationToken: CancellationToken.None);
            handleMessage(result, buffer);
        }
    }
}