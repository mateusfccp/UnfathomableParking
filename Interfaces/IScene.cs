using UnfathomableParking.Services;

namespace UnfathomableParking.Interfaces;

/// <summary>
/// A game scene that draws to the screen.
/// </summary>
public interface IScene
{
    /// <summary>
    /// Draws the scene to the screen.
    /// </summary>
    /// <param name="canvas">A canvas to draw onto.</param>
    void Draw(Engine.Canvas canvas);

    /// <summary>
    /// Called when a key is pressed.
    /// </summary>
    /// <param name="keyInfo">The key pressed.</param>
    void OnKeyPressed(ConsoleKeyInfo keyInfo);

    /// <summary>
    /// Called when the scene is disposed.
    /// <br />
    /// This is called by the <see cref="Engine"/> when the scene is either updated or the engine is stopped.
    /// </summary>
    void Dispose()
    {
    }
}
