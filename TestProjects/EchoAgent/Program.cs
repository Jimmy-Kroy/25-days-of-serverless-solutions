using A2A;
using A2A.AspNetCore;
using EchoAgent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var agent = new Echo();
var taskManager = new TaskManager();

agent.Attach(taskManager);

app.MapA2A(taskManager, "/agent");

Console.WriteLine("A2A Echo Agent is running on http://localhost:5000/agent");

// Start the web app in the background
await app.RunAsync();



//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
