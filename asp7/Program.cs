using LiveCenterMonitoring.Hubs;
using LiveCenterMonitoring.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<ConnectionService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<MonitoringHub>("/monitoringHub");
app.MapPost(
    "/api/system-message",
    async (SystemMessageRequest request, ConnectionService service) =>
    {
        await service.SendSystemMessageAsync(request.Message);
        return Results.Ok();
    });

app.MapPost(
    "/api/room-notification",
    async (
        RoomNotificationRequest request,
        ConnectionService service) =>
    {
        await service.SendRoomMessageAsync(
            request.RoomName,
            request.Message);

        return Results.Ok();
    });

app.MapPost(
    "/api/private-notification",
    async (
        PrivateNotificationRequest request,
        ConnectionService service) =>
    {
        await service.SendPrivateMessageAsync(
            request.TargetConnectionId,
            request.Message);

        return Results.Ok();
    });

app.Run();
public record SystemMessageRequest(string Message);
public record RoomNotificationRequest(
    string RoomName,
    string Message);

public record PrivateNotificationRequest(
    string TargetConnectionId,
    string Message);
