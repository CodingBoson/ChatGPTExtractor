using ChatGPT;
using Cocona;

var builder = CoconaApp.CreateBuilder();

var app = builder.Build();

app.AddCommands<Commands>();

app.Run();