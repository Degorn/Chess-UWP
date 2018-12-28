using System;
using System.Threading;

namespace Chess_UWP.Infrastructure.GameProviderComponents.GameTimer
{
    public class GameTimer : IGameTimer
    {
        private Timer gameTimer;
        private int gameLengthInSeconds;

        private readonly IGameStartAndOver game;

        public GameTimer(IGameStartAndOver game)
        {
            this.game = game;

            this.game.GameStart += Game_Start;
            this.game.GameOver += Game_Over;
        }

        private void Game_Start(object sender, EventArgs e)
        {
            StartTimer();
        }

        private void Game_Over(object sender, GameOverEventArgs e)
        {
            StopTimer();
        }

        public void StartTimer()
        {
            gameLengthInSeconds = 0;
            gameTimer = new Timer(Tick, null, 1000, 1000);
        }

        public void StopTimer()
        {
            gameTimer?.Dispose();
        }

        private void Tick(object state)
        {
            gameLengthInSeconds++;
        }

        public int GetTheElapsedTime()
        {
            return gameLengthInSeconds;
        }
    }
}
