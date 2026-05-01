using System.Text;
using UnfathomableParking.Interfaces;
using UnfathomableParking.Models;

namespace UnfathomableParking.Services;

/// <summary>
/// A basic engine that can be used to draw to the console and process user input.
/// <br />
/// After creating a new instance of this class, call <see cref="Start"/> when you are ready to start the engine.
/// </summary>
public class Engine
{
    private int _width = Console.WindowWidth;
    private int _height = Console.WindowHeight;
    private bool _isRunning;
    private Cell[,] _currentBuffer;
    private Cell[,] _nextBuffer;
    private IScene _scene;

    /// <summary>
    /// The current instance of the engine.
    /// <br />
    /// This is null if the engine is not running.
    /// </summary>
    public static Engine? Instance { get; private set; }

    /// <summary>
    /// Creates a new engine with the given initial scene.
    /// </summary>
    /// <param name="initialScene">The screen with which the engine initializes.</param>
    public Engine(IScene initialScene)
    {
        _currentBuffer = new Cell[_width, _height];
        _nextBuffer = new Cell[_width, _height];
        _scene = initialScene;
    }

    /// <summary>
    /// Starts the engine.
    /// </summary>
    public void Start()
    {
        if (Instance != null)
            throw new InvalidOperationException(
                $"""
                 {nameof(Engine)} has already been started. Dispose of the existing instance before starting a new one."
                 """);

        _isRunning = true;
        Console.CursorVisible = false;
        Instance = this;
        Loop();
        Instance = null;
        Console.CursorVisible = true;
        Console.Clear();
    }

    /// <summary>
    /// Stops the engine.
    /// <br />
    /// When the engine stops, it will dispose of the current scene and other resources used by the engine.
    /// </summary>
    public void Stop()
    {
        _scene.Dispose();
        _isRunning = false;
    }

    /// <summary>
    /// Sets the current scene to the given scene.
    /// <br />
    /// The old scene will be disposed of by calling <see cref="IScene.Dispose"/> before the new scene is set.
    /// </summary>
    /// <param name="newScene"></param>
    public void UpdateScene(IScene newScene)
    {
        _scene.Dispose();
        _scene = newScene;
    }

    private void Loop()
    {
        while (_isRunning)
        {
            PollConsoleSize();
            var canvas = new Canvas(_width, _height);
            var key = ProcessInput();

            _scene.OnKeyPressed(key);
            _scene.Draw(canvas);

            _nextBuffer = canvas.Buffer;

            DrawBuffer();
        }
    }

    private void PollConsoleSize()
    {
        if (Console.WindowWidth != _width || Console.WindowHeight != _height)
        {
            _width = Console.WindowWidth;
            _height = Console.WindowHeight;
            _currentBuffer = new Cell[_width, _height];
        }
    }

    private static ConsoleKey ProcessInput()
    {
        if (Console.KeyAvailable)
        {
            return Console.ReadKey(intercept: true).Key;
        }
        else
        {
            return ConsoleKey.None;
        }
    }

    private void DrawBuffer()
    {
        var framePayload = new StringBuilder();
        Style? activeStyle = null;

        for (var y = 0; y < _currentBuffer.GetLength(1); y++)
        {
            for (var x = 0; x < _currentBuffer.GetLength(0); x++)
            {
                if (_currentBuffer[x, y] == _nextBuffer[x, y]) continue;

                var targetCell = _nextBuffer[x, y];
                _currentBuffer[x, y] = targetCell;

                framePayload.Append(Ansi.MoveTo(x, y));

                if (activeStyle == null || activeStyle.Value != targetCell.Style)
                {
                    framePayload.Append(Ansi.GetStyleSequence(targetCell.Style));
                    activeStyle = targetCell.Style;
                }

                framePayload.Append(targetCell.Character);
            }
        }

        // Flush the entire frame to the console in one single I/O operation
        if (framePayload.Length > 0)
        {
            framePayload.Append(Ansi.Reset);
            Console.Write(framePayload.ToString());
        }
    }

    /// <summary>
    ///  A canvas that can be used to draw to.
    /// </summary>
    public class Canvas
    {
        /// <summary>
        /// Creates a new canvas with the given width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Canvas(int width, int height)
        {
            Buffer = new Cell[width, height];
        }

        /// <summary>
        /// Clears the canvas.
        /// <br/>
        /// You should probably call this before drawing anything.
        /// </summary>
        public void Clear()
        {
            for (var x = 0; x < Buffer.GetLength(0); x++)
            {
                for (var y = 0; y < Buffer.GetLength(1); y++)
                {
                    Buffer[x, y] = Cell.Empty;
                }
            }
        }

        /// <summary>
        /// Draws a string at the given coordinates.
        /// </summary>
        /// <param name="content">The content to be drawn.</param>
        /// <param name="x">The X position of the content.</param>
        /// <param name="y">The Y position of the content.</param>
        /// <param name="style">An optional style to be used in the content.</param>
        public void Draw(string content, int x, int y, Style style = new())
        {
            var currentX = x;
            var currentY = y;

            for (var i = 0; i < content.Length; i++)
            {
                currentX++;
                if (content[i] == Environment.NewLine[0])
                {
                    currentY++;
                    currentX = x;
                    i = i + Environment.NewLine.Length - 1;
                }
                else if (currentX >= 0 && currentX < Width && currentY >= 0 && currentY < Height)
                {
                    Buffer[currentX, currentY] = new Cell(content[i], style);
                }
            }
        }

        /// <summary>
        /// The buffer that is being drawn to.
        /// </summary>
        public Cell[,] Buffer { get; }

        /// <summary>
        /// The width of the canvas.
        /// </summary>
        public int Width => Buffer.GetLength(0);

        /// <summary>
        /// The height of the canvas.
        /// </summary>
        public int Height => Buffer.GetLength(1);
    }

    /// <summary>
    ///  Represents a cell in the console.
    /// </summary>
    /// <param name="character">The character in the cell.</param>
    /// <param name="style">The style of the cell.</param>
    public readonly struct Cell(char character, Style style) : IEquatable<Cell>
    {
        /// <summary>
        ///  Represents an empty cell.
        /// </summary>
        public static Cell Empty { get; } = new Cell(' ', new Style());

        public char Character { get; } = character;
        public Style Style { get; } = style;

        /// <summary>
        /// Compares two cells for equality.
        /// </summary>
        /// <param name="other">The other cell being compared to this one.</param>
        /// <returns>Whether the cells are equal.</returns>
        public bool Equals(Cell other)
        {
            return Character == other.Character && Style.Equals(other.Style);
        }

        public override bool Equals(object? obj) => obj is Cell other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Character, Style);

        public static bool operator ==(Cell left, Cell right) => left.Equals(right);
        public static bool operator !=(Cell left, Cell right) => !left.Equals(right);
    }
}
