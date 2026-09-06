using System.Diagnostics;
using TaskBoard.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ITaskService, InMemoryTaskService>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();

    context.Response.OnStarting(() =>
    {
        sw.Stop();

        context.Response.Headers["X-Response-Time-ms"] =
            sw.ElapsedMilliseconds.ToString();

        return Task.CompletedTask;
    });

    await next(context);
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-App-Name", "TaskBoard");

    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("--> {Method} {Path}", context.Request.Method,
    context.Request.Path);

    await next(context);

    logger.LogInformation("<-- {StatusCode}", context.Response.StatusCode);
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/health")
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("healthy");
        return; // не вызываем next — запрос не идёт дальше
    }
    await next(context);
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
