namespace SnakeGame.Domain.Entities
{
    public class Food
    {
        public Position Position { get; }

        public Food(Position position)
        {
            Position = position;
        }

        public bool IsEaten(Position snakeHead) => Position.Equals(snakeHead);
    }
}
