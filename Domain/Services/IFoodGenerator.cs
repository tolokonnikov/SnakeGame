using SnakeGame.Domain.Entities;

namespace SnakeGame.Domain.Services
{
    public interface IFoodGenerator
    {
        Food GenerateFood(GameSettings settings, IReadOnlyList<Position> snakeBody);
    }
}
