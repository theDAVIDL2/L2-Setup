using System.Windows;
using WindowsSetup.App.Utils;

namespace WindowsSetup.App
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Check if running as administrator
            if (!AdminHelper.IsAdministrator())
            {
                var result = MessageBox.Show(
                    "This application requires administrator privileges.\n\nWould you like to restart as administrator?",
                    "Administrator Required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    AdminHelper.RestartAsAdmin();
                }
                else
                {
                    Current.Shutdown();
                }
            }
        }
    }
}

