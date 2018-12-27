using System;

namespace Chess_UWP.Infrastructure.GameProviderComponents.MoveTimer
{
    public interface IMoveTimer
    {
        event TimerTickDelegate TimerTick;
        event EventHandler MoveEnds;

        void SetTimer(int secondsOnMove);
        void StartTimer();
    }
}
