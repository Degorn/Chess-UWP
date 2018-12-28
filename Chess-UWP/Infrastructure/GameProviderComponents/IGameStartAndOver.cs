using System;

namespace Chess_UWP.Infrastructure.GameProviderComponents
{
    public interface IGameStartAndOver
    {
        event EventHandler GameStart;
        event GameOverDelegate GameOver;
    }
}
