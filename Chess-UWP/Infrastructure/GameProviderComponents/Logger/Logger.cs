using System;
using Chess_UWP.Infrastructure.GameProviderComponents.GameTimer;
using Chess_UWP.Models;
using Windows.Foundation;

namespace Chess_UWP.Infrastructure.GameProviderComponents.Logger
{
    public class Logger : ILogger
    {
        public event EventHandler GameStart;
        public event GameOverDelegate GameOver;
        public event MovingDelegate Moving;
        public event MovedDelegate Moved;

        private readonly IGameProvider gameProvider;
        private readonly IGameTimer gameTimer;

        public Logger(IGameProvider gameProvider, IGameTimer gameTimer)
        {
            this.gameProvider = gameProvider;
            this.gameTimer = gameTimer;

            this.gameProvider.GameStart += GameProvider_GameStart;
            this.gameProvider.GameOver += GameProvider_GameOver;
            this.gameProvider.Moved += GameProvider_Moved;
        }

        private void GameProvider_GameStart(object sender, EventArgs e)
        {
            GameStart?.Invoke(sender, e);
        }

        private void GameProvider_GameOver(object sender, GameOverEventArgs e)
        {
            e.GameLength = TimeSpan.FromSeconds(gameTimer.GetTheElapsedTime()).ToString(@"hh\:mm\:ss");
            GameOver?.Invoke(sender, e);
        }

        public void FinalizeMove()
        {
            throw new NotImplementedException();
        }

        private void GameProvider_Moved(object sender, MovedEventArgs e)
        {
            e.StartPosition = AdaptPositionToBoard(e.StartPosition);
            e.EndPosition = AdaptPositionToBoard(e.EndPosition);
            Moved?.Invoke(sender, e);
        }

        private Point AdaptPositionToBoard(Point position)
        {
            return new Point(position.X, Board.BOARD_HEIGHT - position.Y);
        }
    }
}
