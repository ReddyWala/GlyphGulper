
using System.Runtime.InteropServices;

using GlyphGulper.Engine;
using GlyphGulper.Entities.Food;
using GlyphGulper.Entities.Player;
using GlyphGulper.Services.Input;
using GlyphGulper.Services.Rendering;
using GlyphGulper.Services.Resolution;
using GlyphGulper.Services.Timer;

using Microsoft.Extensions.DependencyInjection;


Console.CursorVisible = false;
using var registration = PosixSignalRegistration.Create(PosixSignal.SIGINT, context =>
{
    context.Cancel = true; // Stop the immediate kill
    Console.Clear();
    Environment.Exit(0);
});

var services = new ServiceCollection();

services.AddSingleton<IGameTimerFactory, GameTimerFactory>();

services.AddSingleton<IConsole, WindowsConsole>();
services.AddSingleton<IInputService, ConsoleInputService>();
services.AddSingleton<IResolutionManager, ResolutionManager>();
services.AddSingleton<IRenderManager, RenderManager>();
services.AddSingleton<IPlayer, Player>();
services.AddSingleton<IFood, Food>();

services.AddSingleton<IGameEngine, GameEngine>();

var serviceProvider = services.BuildServiceProvider();

IGameEngine gameEngine = serviceProvider.GetRequiredService<IGameEngine>();
gameEngine.Start();