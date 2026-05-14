using UnfathomableParking.Interfaces;
using UnfathomableParking.Services;

var scene = new NoScene();
var engine = new Engine(scene);
engine.Start();

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
