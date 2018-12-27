using System;
using System.Threading;

namespace Chess_UWP.Infrastructure.GameProviderComponents.MoveTimer
{
    public class MoveTimer : IMoveTimer
    {
        public event TimerTickDelegate TimerTick;
        public event EventHandler TimeIsUp;

        private SynchronizationContext syncContext;
        private Timer moveTimer;
        private int secondsOnMove;
        private int secondsLeft;

        private IMotionHandler motionHandler;

        public MoveTimer(IMotionHandler motionHandler)
        {
            syncContext = SynchronizationContext.Current;

            this.motionHandler = motionHandler;
            this.motionHandler.Move += RestartTimer;  
            
        }

        private void RestartTimer()
        {
            StopTimer();
            StartTimer();
        }

        private void RestartTimer(object sender, MoveEventArgs e)
        {
            RestartTimer();
        }

        public void SetTimer(int secondsOnMove)
        {
            this.secondsOnMove = secondsOnMove;
        }

        public void StopTimer()
        {
            moveTimer?.Dispose();
        }

        public void StartTimer()
        {
            if (secondsOnMove == 0)
            {
                return;
            }

            secondsLeft = secondsOnMove;
            moveTimer = new Timer(Tick, null, 1000, 1000);
        }

        private void Tick(object state)
        {
            syncContext.Post(a =>
            {
                TimerTick?.Invoke(this, new TimerTickEventArgs { SecondsLeft = secondsLeft });
                secondsLeft--;
                if (secondsLeft < 0)
                {
                    motionHandler.FinalizeMove();
                    TimeIsUp(this, EventArgs.Empty);
                    RestartTimer();
                }
            }, null);
        }

    }
}
