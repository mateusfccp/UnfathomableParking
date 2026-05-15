using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Scenes;
using UnfathomableParking.Services;

internal sealed class NoScene : IScene
{
    public void Draw(Engine.Canvas canvas)
    {
    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key != ConsoleKey.None)
        {
            Engine.Instance?.Stop();
        }
    }
}
