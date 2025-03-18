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
    
    var html = new StringBuilder();
    html.Append(@"
<!DOCTYPE html>
<html lang='es'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Sistema de Mensajería</title>
    <meta http-equiv='refresh' content='5'>
    <style>
        :root {
            --primary-color: #3498db;
            --background-color: #f8f9fa;
            --text-color: #333;
            --card-bg: #ffffff;
            --shadow: 0 4px 6px rgba(0,0,0,0.1);
        }
        
        body {
            font-family: 'Segoe UI', Roboto, Oxygen, Ubuntu, sans-serif;
            margin: 0;
            padding: 0;
            background-color: var(--background-color);
            color: var(--text-color);
            line-height: 1.6;
        }
        
        .container {
            max-width: 800px;
            margin: 0 auto;
            padding: 2rem;
        }
        
        header {
            text-align: center;
            margin-bottom: 2rem;
        }
        
        h1 {
            color: var(--primary-color);
            font-weight: 300;
            margin-bottom: 0.5rem;
        }
        
        .subtitle {
            color: #777;
            font-size: 0.9rem;
        }
        
        .message-card {
            background-color: var(--card-bg);
            border-radius: 8px;
            box-shadow: var(--shadow);
            padding: 1.5rem;
            margin-bottom: 2rem;
        }
        
        .message-list {
            list-style-type: none;
            padding: 0;
            margin: 0;
        }
        
        .message-item {
            padding: 0.8rem 0;
            border-bottom: 1px solid #eee;
        }
        
        .message-item:last-child {
            border-bottom: none;
        }
        
        .timestamp {
            color: #777;
            font-size: 0.8rem;
            margin-right: 0.5rem;
        }
        
        .empty-state {
            text-align: center;
            padding: 2rem;
            color: #777;
        }
        
        footer {
            text-align: center;
            font-size: 0.8rem;
            color: #777;
            margin-top: 2rem;
        }
        
        @media (prefers-color-scheme: dark) {
            :root {
                --primary-color: #61dafb;
                --background-color: #121212;
                --text-color: #e0e0e0;
                --card-bg: #1e1e1e;
            }
        }
    </style>
</head>
<body>
    <div class='container'>
        <header>
            <h1>Sistema de Mensajería</h1>
            <div class='subtitle'>Mensajes en tiempo real vía RabbitMQ</div>
        </header>
        
        <div class='message-card'>");
    
    if (messages.IsEmpty)
    {
        html.Append(@"
            <div class='empty-state'>
                <p>No hay mensajes recibidos.</p>
                <p>Los mensajes aparecerán aquí cuando se envíen desde la aplicación de consola.</p>
            </div>");
    }
    else
    {
        html.Append(@"
            <ul class='message-list'>");
        
        foreach (var message in messages)
        {
            var parts = message.Split(": ", 2, StringSplitOptions.None);            
            if (parts.Length == 2)
            {
                html.Append($@"
                <li class='message-item'>
                    <span class='timestamp'>{parts[0]}</span>
                    {parts[1]}
                </li>");
            }
            else
            {
                html.Append($@"<li class='message-item'>{message}</li>");
            }
        }
        
        html.Append(@"
            </ul>");
    }
    
    html.Append(@"
        </div>
        
        <footer>
            &copy; " + DateTime.Now.Year + @" Sistema de Mensajería
        </footer>
    </div>
</body>
</html>");

    await context.Response.WriteAsync(html.ToString());
});

app.Run();