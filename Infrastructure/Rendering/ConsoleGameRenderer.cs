using SnakeGame.Application.Services;
using SnakeGame.Domain.Entities;

namespace SnakeGame.Infrastructure.Rendering
{
    public class ConsoleGameRenderer : IGameRenderer
    {
        private const char SNAKE_HEAD = '█';
        private const char SNAKE_BODY = '▓';
        private const char FOOD = '●';
        private const char WALL = '│';

        public void RenderGame(IGameEngine gameEngine)
        {
            Console.SetCursorPosition(0, 0);

            var settings = gameEngine.Settings;
            var snake = gameEngine.Snake;
            var food = gameEngine.Food;
            var stats = gameEngine.Stats;

            // Render top border
            Console.WriteLine($"┌{new string('─', settings.Width * 2)}┐");

            // Render game area
            for (var y = 0; y < settings.Height; y++)
            {
                Console.Write(WALL);
                for (var x = 0; x < settings.Width; x++)
                {
                    var position = new Position(x, y);
                    var character = GetCharacterAt(position, snake, food);
                    Console.Write($"{character} ");
                }
                Console.WriteLine(WALL);
            }

            // Render bottom border
            Console.WriteLine($"└{new string('─', settings.Width * 2)}┘");

            // Render stats
            Console.WriteLine($"Score: {stats.Score} | Level: {stats.Level} | Time: {stats.PlayTime:mm\\:ss}");
            Console.WriteLine($"Length: {snake.Length} | State: {gameEngine.CurrentState}");
        }

        public void RenderGameOver(GameStats stats)
        {
            Console.Clear();
            Console.WriteLine("═══════════════════════════════");
            Console.WriteLine("           GAME OVER           ");
            Console.WriteLine("═══════════════════════════════");
            Console.WriteLine($"Final Score: {stats.Score}");
            Console.WriteLine($"Level Reached: {stats.Level}");
            Console.WriteLine($"Time Played: {stats.PlayTime:mm\\:ss}");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
        }

        public void RenderInstructions()
        {
            Console.WriteLine("Snake Game - Modern C# Edition");
            Console.WriteLine("==============================");
            Console.WriteLine("Controls:");
            Console.WriteLine("  W/↑ - Move Up");
            Console.WriteLine("  S/↓ - Move Down");
            Console.WriteLine("  A/← - Move Left");
            Console.WriteLine("  D/→ - Move Right");
            Console.WriteLine("  P   - Pause/Resume");
            Console.WriteLine("  ESC - Quit");
            Console.WriteLine();
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();
        }

        public void Clear()
        {
            Console.Clear();
        }

        private static char GetCharacterAt(Position position, Snake snake, Food? food)
        {
            if (position.Equals(snake.Head))
                return SNAKE_HEAD;

            if (snake.Body.Skip(1).Contains(position))
                return SNAKE_BODY;

            if (food?.Position.Equals(position) == true)
                return FOOD;

            return ' ';
        }
    }
}
