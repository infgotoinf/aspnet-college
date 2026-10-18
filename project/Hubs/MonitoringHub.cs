using LiveCenterMonitoring.Services;
using Microsoft.AspNetCore.SignalR;

namespace LiveCenterMonitoring.Hubs;

public class MonitoringHub : Hub
{
    private readonly ConnectionService _connectionService;

    public MonitoringHub(ConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public override async Task OnConnectedAsync()
    {
        await _connectionService.RegisterConnectionAsync(
            Context.ConnectionId);

        await Clients.Caller.SendAsync(
            "ConnectionId",
            Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _connectionService.RemoveConnectionAsync(
            Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        _connectionService.UpdateRoom(Context.ConnectionId, roomName);

        await Clients.Caller.SendAsync("RoomJoined", roomName);
    }

    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        _connectionService.UpdateRoom(Context.ConnectionId, null);

        await Clients.Caller.SendAsync("RoomLeft", roomName);
    }

    public async Task SendMessage(
        string roomName,
        string user,
        string message)
    {
        await Clients.Group(roomName).SendAsync(
            "ReceiveMessage",
            user,
            message);
    }

    public async Task SendPrivateMessage(
        string targetConnectionId,
        string user,
        string message)
    {
        await _connectionService.SendPrivateMessageAsync(
            targetConnectionId,
            $"{user}: {message}");

        await Clients.Caller.SendAsync(
            "PrivateMessageSent",
            targetConnectionId);
    }
}
