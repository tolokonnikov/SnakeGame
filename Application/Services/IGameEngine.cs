using SnakeGame.Domain.Entities;

namespace SnakeGame.Application.Services
{
    public interface IGameEngine
    {
        GameState CurrentState { get; }
        Snake Snake { get; }
        Food? Food { get; }
        GameStats Stats { get; }
        GameSettings Settings { get; }

        Task StartGameAsync(CancellationToken cancellationToken = default);
        void HandleInput(ConsoleKey key);
        void PauseGame();
        void ResumeGame();
        event EventHandler<GameStats>? GameOver;
        event EventHandler<GameStats>? ScoreChanged;
    }
}
