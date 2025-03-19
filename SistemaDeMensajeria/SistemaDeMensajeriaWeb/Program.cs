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
                --primary-color: #4361ee;
                --secondary-color: #3a0ca3;
                --accent-color: #f72585;
                --background-color: #f8f9fa;
                --text-color: #333;
                --card-bg: #ffffff;
                --shadow: 0 10px 20px rgba(0,0,0,0.08);
                --border-radius: 12px;
            }
    
            body {
                font-family: 'Segoe UI', Roboto, system-ui, sans-serif;
                margin: 0;
                padding: 0;
                background-color: var(--background-color);
                color: var(--text-color);
                line-height: 1.6;
                transition: all 0.3s ease;
            }
    
            .container {
                max-width: 900px;
                margin: 2rem auto;
                padding: 0 1.5rem;
            }
    
            header {
                text-align: center;
                margin-bottom: 2.5rem;
                padding: 1.5rem 0;
                position: relative;
                animation: fadeIn 0.8s ease-out;
            }
    
            header::after {
                content: '';
                position: absolute;
                bottom: 0;
                left: 25%;
                right: 25%;
                height: 3px;
                background: linear-gradient(to right, transparent, var(--accent-color), transparent);
            }
    
            h1 {
                color: var(--primary-color);
                font-weight: 600;
                margin-bottom: 0.5rem;
                font-size: 2.5rem;
                letter-spacing: -0.5px;
            }
    
            .subtitle {
                color: var(--secondary-color);
                font-size: 1.1rem;
                font-weight: 500;
                opacity: 0.85;
            }
    
            .status-indicator {
                display: inline-block;
                width: 12px;
                height: 12px;
                background-color: #38b000;
                border-radius: 50%;
                margin-right: 8px;
                animation: pulse 2s infinite;
            }
    
            .connection-status {
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 0.9rem;
                margin-top: 10px;
            }
    
            .message-card {
                background-color: var(--card-bg);
                border-radius: var(--border-radius);
                box-shadow: var(--shadow);
                padding: 2rem;
                margin-bottom: 2rem;
                animation: slideUp 0.5s ease-out;
                transform-origin: bottom;
            }
    
            .message-list {
                list-style-type: none;
                padding: 0;
                margin: 0;
            }
    
            .message-item {
                padding: 1rem;
                border-bottom: 1px solid #eee;
                margin-bottom: 0.5rem;
                transition: all 0.2s ease;
                border-radius: 8px;
                animation: fadeIn 0.5s ease-out forwards;
                opacity: 0;
                transform: translateY(10px);
            }
    
            .message-item:hover {
                background-color: rgba(67, 97, 238, 0.05);
            }
    
            .message-item:last-child {
                border-bottom: none;
                margin-bottom: 0;
            }
    
            .timestamp {
                color: #777;
                font-size: 0.8rem;
                margin-right: 0.8rem;
                background-color: rgba(67, 97, 238, 0.1);
                padding: 4px 8px;
                border-radius: 20px;
                font-weight: 500;
            }
    
            .message-content {
                font-size: 1.05rem;
                word-break: break-word;
            }
    
            .empty-state {
                text-align: center;
                padding: 3rem 1rem;
                color: #777;
                animation: pulse 2s infinite alternate;
            }
    
            .empty-state svg {
                width: 80px;
                height: 80px;
                margin-bottom: 1.5rem;
                color: var(--primary-color);
            }
    
            footer {
                text-align: center;
                font-size: 0.9rem;
                color: #777;
                margin-top: 2rem;
                padding: 1.5rem;
            }
    
            .tech-badge {
                display: inline-block;
                padding: 5px 10px;
                background-color: var(--primary-color);
                color: white;
                border-radius: 20px;
                font-size: 0.8rem;
                margin: 0 5px;
                font-weight: 500;
            }
    
            @keyframes fadeIn {
                from { opacity: 0; }
                to { opacity: 1; }
            }
    
            @keyframes slideUp {
                from { 
                    opacity: 0;
                    transform: translateY(20px);
                }
                to { 
                    opacity: 1;
                    transform: translateY(0);
                }
            }
    
            @keyframes pulse {
                0% { opacity: 0.6; }
                50% { opacity: 1; }
                100% { opacity: 0.6; }
            }
    
            /* Apply staggered animation to list items */
            .message-item:nth-child(1) { animation-delay: 0.1s; }
            .message-item:nth-child(2) { animation-delay: 0.2s; }
            .message-item:nth-child(3) { animation-delay: 0.3s; }
            .message-item:nth-child(4) { animation-delay: 0.4s; }
            .message-item:nth-child(5) { animation-delay: 0.5s; }
    
            @media (prefers-color-scheme: dark) {
                :root {
                    --primary-color: #4cc9f0;
                    --secondary-color: #7209b7;
                    --accent-color: #f72585;
                    --background-color: #121212;
                    --text-color: #e0e0e0;
                    --card-bg: #1e1e1e;
                }
            }
            
            @media (max-width: 768px) {
                .container {
                    padding: 0 1rem;
                    margin: 1rem auto;
                }
                
                h1 {
                    font-size: 2rem;
                }
                
                .message-card {
                    padding: 1.5rem;
                }
            }
        </style>
    </head>
    <body>
        <div class='container'>
            <header>
                <h1>Sistema de Mensajería</h1>
                <div class='subtitle'>Mensajes en tiempo real vía RabbitMQ</div>
                <div class='connection-status'>
                    <span class='status-indicator'></span>
                    Conexión activa
                </div>
            </header>
    
            <div class='message-card'>");
    
        if (messages.IsEmpty)
        {
            html.Append(@"
                <div class='empty-state'>
                    <svg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 24 24' stroke='currentColor'>
                        <path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z' />
                    </svg>
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
                        <span class='message-content'>{parts[1]}</span>
                    </li>");
                }
                else
                {
                    html.Append($@"<li class='message-item'><span class='message-content'>{message}</span></li>");
                }
            }
    
            html.Append(@"
                </ul>");
        }
    
        html.Append(@"
            </div>
    
            <footer>
                <div>
                    <span class='tech-badge'>RabbitMQ</span>
                    <span class='tech-badge'>.NET</span>
                    <span class='tech-badge'>C#</span>
                    <span class='tech-badge'>Docker</span>
                </div>
                <div style='margin-top: 10px;'>
                    &copy; " + DateTime.Now.Year + @" Sistema de Mensajería
                </div>
            </footer>
        </div>
    </body>
    </html>");

    await context.Response.WriteAsync(html.ToString());
});

app.Run();