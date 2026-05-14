using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;
using UnfathomableParking.Scenes;
using UnfathomableParking.Services;


var manager = new ParkingBeachManager();
manager.AddParkingBeach("Parking bitch", 10, 10);
manager.AddParkingBeach("Nigga", 15, 15);
var previousScene = new NoScene();
var scene = new EditBeachScene(manager, previousScene);
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
