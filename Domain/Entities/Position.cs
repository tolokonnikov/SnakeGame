namespace SnakeGame.Domain.Entities
{
    public record Position(int X, int Y)
    {
        public Position Move(Direction direction) => direction switch
        {
            Direction.Up => this with { Y = Y - 1 },
            Direction.Down => this with { Y = Y + 1 },
            Direction.Left => this with { X = X - 1 },
            Direction.Right => this with { X = X + 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };

        public bool IsOutOfBounds(int width, int height) =>
            X < 0 || X >= width || Y < 0 || Y >= height;
    }

    public enum Direction { Up, Down, Left, Right }

    public enum GameState { Playing, GameOver, Paused }

    public record GameSettings(int Width = 20, int Height = 15, int InitialSpeed = 200);
}
