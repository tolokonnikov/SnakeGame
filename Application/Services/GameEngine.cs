using SnakeGame.Domain.Entities;
using SnakeGame.Domain.Services;

namespace SnakeGame.Application.Services
{
    public class GameEngine : IGameEngine, IDisposable
    {
        private readonly IFoodGenerator _foodGenerator;
        private readonly Timer _gameTimer;
        private readonly object _lockObject = new();
        private readonly DateTime _startTime;

        private Snake _snake = null!;
        private Food? _food;
        private GameState _state = GameState.Playing;
        private int _score = 0;
        private int _level = 1;
        private int _currentSpeed;

        public GameEngine(IFoodGenerator foodGenerator, GameSettings? settings = null)
        {
            _foodGenerator = foodGenerator ?? throw new ArgumentNullException(nameof(foodGenerator));
            Settings = settings ?? new GameSettings();
            _currentSpeed = Settings.InitialSpeed;
            _startTime = DateTime.Now;

            Initialize();
            _gameTimer = new Timer(GameTick, null, Timeout.Infinite, Timeout.Infinite);
        }

        public GameState CurrentState => _state;
        public Snake Snake => _snake;
        public Food? Food => _food;
        public GameStats Stats => new(_score, _level, DateTime.Now - _startTime);
        public GameSettings Settings { get; }

        public event EventHandler<GameStats>? GameOver;
        public event EventHandler<GameStats>? ScoreChanged;

        public Task StartGameAsync(CancellationToken cancellationToken = default)
        {
            if (_state != GameState.Playing) return Task.CompletedTask;

            _gameTimer.Change(0, _currentSpeed);
            return Task.CompletedTask;
        }

        public void HandleInput(ConsoleKey key)
        {
            if (_state != GameState.Playing) return;

            lock (_lockObject)
            {
                var newDirection = key switch
                {
                    ConsoleKey.W or ConsoleKey.UpArrow => Direction.Up,
                    ConsoleKey.S or ConsoleKey.DownArrow => Direction.Down,
                    ConsoleKey.A or ConsoleKey.LeftArrow => Direction.Left,
                    ConsoleKey.D or ConsoleKey.RightArrow => Direction.Right,
                    _ => (Direction?)null
                };

                if (newDirection.HasValue)
                    _snake.ChangeDirection(newDirection.Value);
            }
        }

        public void PauseGame()
        {
            _state = GameState.Paused;
            _gameTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void ResumeGame()
        {
            if (_state == GameState.Paused)
            {
                _state = GameState.Playing;
                _gameTimer.Change(_currentSpeed, _currentSpeed);
            }
        }

        private void Initialize()
        {
            var centerX = Settings.Width / 2;
            var centerY = Settings.Height / 2;
            _snake = new Snake(new Position(centerX, centerY));
            GenerateFood();
        }

        private void GameTick(object? state)
        {
            if (_state != GameState.Playing) return;

            lock (_lockObject)
            {
                _snake.Move();

                // Check collisions
                if (_snake.HasCollision(Settings))
                {
                    _state = GameState.GameOver;
                    _gameTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    GameOver?.Invoke(this, Stats);
                    return;
                }

                // Check food consumption
                if (_food?.IsEaten(_snake.Head) == true)
                {
                    _snake.Grow();
                    _score += 10;
                    UpdateLevel();
                    GenerateFood();
                    ScoreChanged?.Invoke(this, Stats);
                }
            }
        }

        private void GenerateFood()
        {
            _food = _foodGenerator.GenerateFood(Settings, _snake.Body);
        }

        private void UpdateLevel()
        {
            var newLevel = (_score / 100) + 1;
            if (newLevel > _level)
            {
                _level = newLevel;
                _currentSpeed = Math.Max(50, Settings.InitialSpeed - (_level * 20));
                _gameTimer.Change(_currentSpeed, _currentSpeed);
            }
        }

        public void Dispose()
        {
            _gameTimer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
