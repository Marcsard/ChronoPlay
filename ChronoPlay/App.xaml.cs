using System.Windows;
using ChronoPlay.Data;
using ChronoPlay.Services;
using ChronoPlay.Views;

namespace ChronoPlay;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

#if CHRONOPLAY_PUBLISHED
        try
        {
            DatabaseBootstrapper.Initialize();
            if (!new UsuarioService().ObtenerTodos().Any())
            {
                InitialAdminWindow initialAdminWindow = new();
                if (initialAdminWindow.ShowDialog() != true)
                {
                    Shutdown();
                    return;
                }
            }
        }
        catch (Exception exception)
        {
            try
            {
                StartupLog.Write(exception);
            }
            catch (Exception logException)
            {
                System.Diagnostics.Trace.WriteLine($"No se pudo escribir el log de inicio: {logException}");
            }

            try
            {
                AppDialog.Show(
                    null,
                    AppDialogType.Error,
                    "// ERROR DE INICIO",
                    "NO SE PUDO INICIAR CHRONOPLAY",
                    "No se pudo abrir la base de datos local. Revisa el log en %LOCALAPPDATA%\\ChronoPlay\\Logs\\startup.log.");
            }
            catch (Exception dialogException)
            {
                System.Diagnostics.Trace.WriteLine($"No se pudo mostrar el diálogo de inicio: {dialogException}");
                MessageBox.Show(
                    "No se pudo inicializar la base de datos local. Revisa %LOCALAPPDATA%\\ChronoPlay\\Logs\\startup.log.",
                    "ChronoPlay",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Shutdown(-1);
            return;
        }
#endif

        LoginWindow loginWindow = new();
        MainWindow = loginWindow;
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        loginWindow.Show();
    }
}
