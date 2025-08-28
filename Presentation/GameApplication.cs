using SnakeGame.Application.Services;
using SnakeGame.Domain.Entities;
using SnakeGame.Infrastructure.Rendering;
using static SnakeGame.Domain.Entities.Enums;

namespace SnakeGame.Presentation
{
    public class GameApplication : IDisposable
    {
        private readonly IGameEngine _gameEngine;
        private readonly IGameRenderer _renderer;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private bool _isRunning = true;

        public GameApplication(IGameEngine gameEngine, IGameRenderer renderer)
        {
            _gameEngine = gameEngine ?? throw new ArgumentNullException(nameof(gameEngine));
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));

            _gameEngine.GameOver += OnGameOver;
            _gameEngine.ScoreChanged += OnScoreChanged;
        }

        public async Task RunAsync()
        {
            Console.CursorVisible = false;
            Console.Clear();

            _renderer.RenderInstructions();
            _renderer.Clear();

            // Start game loop
            await _gameEngine.StartGameAsync(_cancellationTokenSource.Token);

            var inputTask = HandleInputAsync(_cancellationTokenSource.Token);
            var renderTask = RenderLoopAsync(_cancellationTokenSource.Token);

            // Wait for both tasks to complete or cancellation
            try
            {
                await Task.WhenAll(inputTask, renderTask);
            }
            catch (OperationCanceledException)
            {
                // Expected when game ends
            }
        }

        private async Task HandleInputAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                while (_isRunning && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(true).Key;

                            switch (key)
                            {
                                case ConsoleKey.Escape:
                                    _isRunning = false;
                                    if (!_cancellationTokenSource.IsCancellationRequested)
                                    {
                                        _cancellationTokenSource.Cancel();
                                    }
                                    return;
                                case ConsoleKey.P:
                                    if (_gameEngine.CurrentState == GameState.Playing)
                                        _gameEngine.PauseGame();
                                    else if (_gameEngine.CurrentState == GameState.Paused)
                                        _gameEngine.ResumeGame();
                                    break;
                                default:
                                    _gameEngine.HandleInput(key);
                                    break;
                            }
                        }
                        Thread.Sleep(16); // ~60 FPS input polling
                    }
                    catch (InvalidOperationException)
                    {
                        // Console operations can fail during shutdown
                        break;
                    }
                }
            }, cancellationToken);
        }

        private async Task RenderLoopAsync(CancellationToken cancellationToken)
        {
            while (_isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_gameEngine.CurrentState == GameState.Playing || _gameEngine.CurrentState == GameState.Paused)
                    {
                        _renderer.RenderGame(_gameEngine);
                    }
                    await Task.Delay(50, cancellationToken); // 20 FPS rendering
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    // Ignore rendering errors during shutdown
                    break;
                }
            }
        }

        private void OnGameOver(object? sender, GameStats stats)
        {
            _isRunning = false;
            _renderer.RenderGameOver(stats);
            Console.ReadKey();

            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private void OnScoreChanged(object? sender, GameStats stats)
        {
            // Could add sound effects or other notifications here
        }

        public void Dispose()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }

            _cancellationTokenSource?.Dispose();

            if (_gameEngine is IDisposable disposableEngine)
                disposableEngine.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
