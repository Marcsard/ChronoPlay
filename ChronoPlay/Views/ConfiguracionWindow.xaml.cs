using System.Windows;
using System.Windows.Controls;
using ChronoPlay.Data;
using ChronoPlay.Services;

namespace ChronoPlay.Views;

public partial class ConfiguracionWindow : Window
{
    private readonly ConfiguracionService _configuracionService = new();

    public ConfiguracionWindow()
    {
        InitializeComponent();

        var configuracion = _configuracionService.Obtener();
        txtNombre.Text = configuracion.NombreNegocio;
        chkSonido.IsChecked = configuracion.SonidoAlarma;
        cmbHistorial.SelectedValue = configuracion.RetencionHistorial;
        if (cmbHistorial.SelectedItem is null)
        {
            cmbHistorial.SelectedValue = "30 días";
        }
        chkRespaldo.IsChecked = configuracion.RespaldosAutomaticos;
    }

    private void Guardar_Click(
        object sender,
        RoutedEventArgs e)
    {
        string nombreNegocio = txtNombre.Text.TrimEnd();
        if (string.IsNullOrWhiteSpace(nombreNegocio))
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// VALIDACIÓN",
                "NOMBRE REQUERIDO",
                "Escribe el nombre del negocio antes de guardar.");
            return;
        }

        var configuracion = _configuracionService.Obtener();
        configuracion.NombreNegocio = nombreNegocio;
        configuracion.SonidoAlarma = chkSonido.IsChecked == true;
        configuracion.RespaldosAutomaticos = chkRespaldo.IsChecked == true;

        if (cmbHistorial.SelectedValue is not string retencionHistorial)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// VALIDACIÓN",
                "RETENCIÓN REQUERIDA",
                "Selecciona un periodo de retención del historial.");
            return;
        }

        configuracion.RetencionHistorial = retencionHistorial;
        _configuracionService.Guardar(configuracion);

        AppDialog.Show(
            this,
            AppDialogType.Success,
            "// CAMBIOS GUARDADOS",
            "CONFIGURACIÓN ACTUALIZADA",
            "La configuración se guardó correctamente.");

        DialogResult = true;
    }
    private void CrearRespaldo_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            string archivo = new BackupService().CrearRespaldo();
            AppDialog.Show(
                this,
                AppDialogType.Success,
                "// RESPALDO CREADO",
                "RESPALDO COMPLETADO",
                $"Se creó el respaldo de la base de datos.\n{archivo}");
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

            AppDialog.Show(
                this,
                AppDialogType.Error,
                "// ERROR DE RESPALDO",
                "NO SE PUDO CREAR EL RESPALDO",
                "Revisa el log en %LOCALAPPDATA%\\ChronoPlay\\Logs\\startup.log.");
        }
    }
}