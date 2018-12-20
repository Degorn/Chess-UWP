using Caliburn.Micro;
using Chess_UWP.Database;
using System.Collections.Generic;

namespace Chess_UWP.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private IEnumerable<GameInfo> gameInfos;
        public IEnumerable<GameInfo> GameInfos
        {
            get => gameInfos;
            set
            {
                gameInfos = value;
                NotifyOfPropertyChange(() => GameInfos);
            }
        }

        private IRepository repository;

        public SettingsViewModel(INavigationService pageNavigationService) : base(pageNavigationService)
        {
        }

        protected override void OnActivate()
        {
            repository = IoC.Get<IRepository>();
            GameInfos = repository.GetAll();
        }

        private void Clear()
        {
            repository.ClearGameStatisticsAsync();
            GameInfos = repository.GetAll();
        }
    }
}
