using SnakeGame.Application.Services;

namespace SnakeGame.Infrastructure.Rendering
{
    public interface IGameRenderer
    {
        void RenderGame(IGameEngine gameEngine);
        void RenderGameOver(GameStats stats);
        void RenderInstructions();
        void Clear();
    }
}
