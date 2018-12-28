using System;
using Chess_UWP.Infrastructure.GameProviderComponents.GameTimer;

namespace Chess_UWP.Infrastructure.GameProviderComponents.Logger
{
    public class Logger : ILogger
    {
        public event EventHandler GameStart;
        public event GameOverDelegate GameOver;

        private readonly IGameProvider gameProvider;
        private readonly IGameTimer gameTimer;

        public Logger(IGameProvider gameProvider, IGameTimer gameTimer)
        {
            this.gameProvider = gameProvider;
            this.gameTimer = gameTimer;

            this.gameProvider.GameStart += GameProvider_GameStart;
            this.gameProvider.GameOver += GameProvider_GameOver;
        }

        private void GameProvider_GameStart(object sender, EventArgs e)
        {
            GameStart?.Invoke(sender, e);
        }

        private void GameProvider_GameOver(object sender, GameOverEventArgs e)
        {
            e.GameLength = TimeSpan.FromSeconds(gameTimer.GetTheElapsedTime()).ToString(@"hh\:mm\:ss");
            GameOver(sender, e);
        }
    }
}
