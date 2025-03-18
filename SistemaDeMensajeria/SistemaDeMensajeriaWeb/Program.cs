using System.Text;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SistemaDeMensajeriaWeb;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddSingleton<ConcurrentQueue<string>>();
builder.Services.AddHostedService<RabbitMQConsumerService>();

var app = builder.Build();

// Ruta para mostrar los mensajes
app.MapGet("/", async (HttpContext context, ConcurrentQueue<string> messages) => {
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync("<html><head><title>Mensajes RabbitMQ</title>");
    await context.Response.WriteAsync("<meta http-equiv='refresh' content='5'>");
    await context.Response.WriteAsync("<style>body{font-family:Arial;margin:20px;}h1{color:#333;}</style>");
    await context.Response.WriteAsync("</head><body>");
    await context.Response.WriteAsync("<h1>Mensajes Recibidos</h1>");
    
    if (messages.IsEmpty)
    {
        await context.Response.WriteAsync("<p>No hay mensajes recibidos.</p>");
    }
    else
    {
        await context.Response.WriteAsync("<ul>");
        foreach (var message in messages)
        {
            await context.Response.WriteAsync($"<li>{message}</li>");
        }
        await context.Response.WriteAsync("</ul>");
    }
    
    await context.Response.WriteAsync("</body></html>");
});

app.Run();