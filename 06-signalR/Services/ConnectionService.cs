using LiveCenterMonitoring.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace LiveCenterMonitoring.Services;

public class ConnectionService
{
    private readonly IHubContext<MonitoringHub> _hubContext;
    private readonly ConcurrentDictionary<string, string?> _connections = new();

    public ConnectionService(IHubContext<MonitoringHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task RegisterConnectionAsync(string connectionId)
    {
        _connections[connectionId] = null;
        await SendOnlineCountAsync();
    }

    public async Task RemoveConnectionAsync(string connectionId)
    {
        _connections.TryRemove(connectionId, out _);
        await SendOnlineCountAsync();
    }

    public void UpdateRoom(string connectionId, string? roomName)
    {
        if (_connections.ContainsKey(connectionId))
        {
            _connections[connectionId] = roomName;
        }
    }

    public async Task SendSystemMessageAsync(string message)
    {
        await _hubContext.Clients.All.SendAsync(
            "ReceiveSystemMessage",
            message);
    }

    public async Task SendRoomMessageAsync(string roomName, string message)
    {
        await _hubContext.Clients.Group(roomName).SendAsync(
            "ReceiveRoomNotification",
            message);
    }

    public async Task SendPrivateMessageAsync(
        string connectionId,
        string message)
    {
        await _hubContext.Clients.Client(connectionId).SendAsync(
            "ReceivePrivateMessage",
            message);
    }

    private async Task SendOnlineCountAsync()
    {
        await _hubContext.Clients.All.SendAsync(
            "OnlineCountUpdated",
            _connections.Count);
    }
}
