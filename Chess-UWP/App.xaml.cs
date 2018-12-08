using Caliburn.Micro;
using Chess_UWP.Infrastructure.Initializers;
using Chess_UWP.ViewModels;
using Chess_UWP.Views;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Chess_UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        private WinRTContainer container;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Initialize();
            this.InitializeComponent();
        }

        protected override void Configure()
        {
            container = new WinRTContainer();
            container.RegisterWinRTServices();

            container.
                PerRequest<MainMenuViewModel>().
                PerRequest<SettingsViewModel>().
                PerRequest<NewGameSettingsViewModel>().
                PerRequest<BoardViewModel>().
                PerRequest<FigureControlViewModel>().
                PerRequest<FigureViewModel>().
                Singleton<IFiguresImagesInitializer, FiguresimagesInitializerDefault>().
                Singleton<IFiguresInitializer, FiguresInitializer>();
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            var navigationService = container.RegisterNavigationService(rootFrame);
            var navigationManager = SystemNavigationManager.GetForCurrentView();

            navigationService.Navigated += (s, e) =>
            {
                navigationManager.AppViewBackButtonVisibility = navigationService.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;
            };
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (e.PreviousExecutionState == ApplicationExecutionState.Running)
                return;

            DisplayRootView<BoardView>();
        }


        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }
}
