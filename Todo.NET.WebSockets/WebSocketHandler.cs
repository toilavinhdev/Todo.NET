using System.Net.WebSockets;
using System.Text;

namespace Todo.NET.WebSockets;
public abstract class WebSocketHandler(WebSocketConnectionManager webSocketConnectionManager)
{
    protected WebSocketConnectionManager WebSocketConnectionManager { get; set; } = webSocketConnectionManager;

    public virtual Task OnConnected(string id, WebSocket socket)
    {
        WebSocketConnectionManager.AddSocket(id, socket);
        return Task.CompletedTask;
    }

    public virtual async Task OnDisconnected(string id, WebSocketReceiveResult result)
    {
        await WebSocketConnectionManager.RemoveSocket(id, result);
    }
    
    public async Task SendMessageAsync(string socketId, string message)
    {
        var socket = WebSocketConnectionManager.GetSocket(socketId);
        if (socket is null) return;
        await SendMessageAsync(socket, message);
    }
    
    public async Task SendMessageBroadcastAsync(string message)
    {
        foreach (var pair in WebSocketConnectionManager.GetAllSockets())
        {
            var socket = pair.Value;
            await SendMessageAsync(socket, message);
        }
    }
    
    private async Task SendMessageAsync(WebSocket socket, string message)
    {
        if (socket.State != WebSocketState.Open) return;

        var buffer = new ArraySegment<byte>(
            array: Encoding.ASCII.GetBytes(message),
            offset: 0,
            count: message.Length);
        
        await socket.SendAsync(
            buffer, 
            WebSocketMessageType.Text, 
            true, 
            CancellationToken.None);
    }
    
    public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);

    public static string BufferToString(WebSocketReceiveResult result, byte[] buffer) 
        => Encoding.UTF8.GetString(buffer, 0, result.Count);
}