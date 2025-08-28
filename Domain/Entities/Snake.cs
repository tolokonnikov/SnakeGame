namespace SnakeGame.Domain.Entities
{
    public class Snake
    {
        private readonly List<Position> _body = [];
        private Direction _direction = Direction.Right;

        public Snake(Position startPosition)
        {
            _body.Add(startPosition);
        }

        public IReadOnlyList<Position> Body => _body.AsReadOnly();
        public Position Head => _body[0];
        public Direction Direction => _direction;
        public int Length => _body.Count;

        public void ChangeDirection(Direction newDirection)
        {
            // Prevent reversing into itself
            if (IsOppositeDirection(newDirection, _direction)) return;
            _direction = newDirection;
        }

        public void Move()
        {
            var newHead = Head.Move(_direction);
            _body.Insert(0, newHead);
            _body.RemoveAt(_body.Count - 1);
        }

        public void Grow()
        {
            var tail = _body[^1];
            _body.Add(tail);
        }

        public bool HasCollision(GameSettings settings) =>
            Head.IsOutOfBounds(settings.Width, settings.Height) ||
            _body.Skip(1).Contains(Head);

        private static bool IsOppositeDirection(Direction dir1, Direction dir2) =>
            (dir1, dir2) switch
            {
                (Direction.Up, Direction.Down) or (Direction.Down, Direction.Up) => true,
                (Direction.Left, Direction.Right) or (Direction.Right, Direction.Left) => true,
                _ => false
            };
    }
}
