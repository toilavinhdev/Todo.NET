using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Todo.NET.WebSockets;

public class WebSocketConnectionManager
{
    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();
    
    public ConcurrentDictionary<string, WebSocket> GetAllSockets() => _sockets;

    public WebSocket? GetSocket(string id) => _sockets.FirstOrDefault(x => x.Key == id).Value;

    public string GetId(WebSocket socket) => _sockets.FirstOrDefault(x => x.Value == socket).Key;
    
    public void AddSocket(string id, WebSocket socket) => _sockets.TryAdd(id, socket);

    public async Task RemoveSocket(string id, WebSocketReceiveResult result)
    {
        _sockets.TryRemove(id, out var socket);
        if (socket is null) return;
        await socket.CloseAsync(
            closeStatus: result.CloseStatus!.Value,
            statusDescription: result.CloseStatusDescription,
            cancellationToken: CancellationToken.None);
    }
}