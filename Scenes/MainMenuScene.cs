using UnfathomableParking.Interfaces;
using UnfathomableParking.Services;

namespace UnfathomableParking.Scenes;

internal class MainMenuScene : IScene
{
    public void Draw(Engine.Canvas canvas)
    {
        canvas.Draw("Menu", 1, 1);
    }

    public void OnKeyPressed(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key == ConsoleKey.Enter)
        {
            Engine.Instance?.UpdateScene(new ParkingBeachScene(new Models.ParkingBeach(10, 10), 1, 1));
        }
    }
}
