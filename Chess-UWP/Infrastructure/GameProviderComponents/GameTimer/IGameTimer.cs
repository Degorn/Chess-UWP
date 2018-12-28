namespace Chess_UWP.Infrastructure.GameProviderComponents.GameTimer
{
    public interface IGameTimer
    {
        void StartTimer();
        void StopTimer();
        int GetTheElapsedTime();
    }
}
