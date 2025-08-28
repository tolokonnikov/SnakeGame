using SnakeGame.Domain.Entities;

namespace SnakeGame.Domain.Services
{
    public class RandomFoodGenerator : IFoodGenerator
    {
        private readonly Random _random = new();

        public Food GenerateFood(GameSettings settings, IReadOnlyList<Position> snakeBody)
        {
            Position position;
            do
            {
                position = new Position(_random.Next(settings.Width), _random.Next(settings.Height));
            } while (snakeBody.Contains(position));

            return new Food(position);
        }
    }
}
