using System.Runtime.InteropServices;

Console.CursorVisible = false;
using var registration = PosixSignalRegistration.Create(PosixSignal.SIGINT, context =>
{
    context.Cancel = true; // Stop the immediate kill
    Console.Clear();
    Environment.Exit(0);
});

GameEngine gameEngine = new();
gameEngine.Start();
