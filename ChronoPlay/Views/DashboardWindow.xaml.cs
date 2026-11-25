using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Effects;
using System.Windows.Data;
using System.Windows.Shapes;
using ChronoPlay.Controls;
using ChronoPlay.Models;
using ChronoPlay.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ChronoPlay.Views;

public partial class DashboardWindow : Window
{
    private Usuario _usuario;


    private readonly ConfiguracionService
      _configuracionService = new();

    private List<Consola> _consolas = new();
    private List<Sesion> _sesiones = new();
    private readonly HashSet<int> _sesionesConErrorAlFinalizar = new();

    private DispatcherTimer _timer = new();

    private readonly CajaService _cajaService = new();




    public DashboardWindow(Usuario usuario)
    {
        InitializeComponent();
        UpdateResponsiveLayout(Width);

        CargarNombreNegocio();

        _usuario = usuario;
        string nombreUsuario = _usuario.Nombre.Trim();
        avatarUsuario.UserName = nombreUsuario;
        avatarUsuario.Role = _usuario.Rol;
        txtUsuarioSesion.Text = string.IsNullOrWhiteSpace(nombreUsuario)
            ? _usuario.UsuarioLogin
            : nombreUsuario;
        txtRolUsuario.Text = $"Rol: {_usuario.Rol}";

        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += Timer_Tick;
        _timer.Start();

        InicializarConsolas();
        CargarHistorialSesiones();

        AplicarPermisos();
    }


    private void DashboardWindow_SizeChanged(
        object sender,
        SizeChangedEventArgs e)
    {
        UpdateResponsiveLayout(e.NewSize.Width);
    }

    private void UpdateResponsiveLayout(double windowWidth)
    {
        bool compact = windowWidth < 1000;
        SidebarColumn.Width = new GridLength(compact ? 72 : 268);
        SidebarContent.Margin = compact ? new Thickness(8) : new Thickness(18);
        BrandLabel.Visibility = compact ? Visibility.Collapsed : Visibility.Visible;
        MenuLabel.Visibility = compact ? Visibility.Collapsed : Visibility.Visible;

        SetNavigationButton(btnAdministrarConsolas, compact);
        SetNavigationButton(btnInventario, compact);
        SetNavigationButton(btnCaja, compact);
        SetNavigationButton(btnReportes, compact);
        SetNavigationButton(btnHistorial, compact);
        SetNavigationButton(btnUsuarios, compact);
        SetNavigationButton(btnConfiguracion, compact);
        SetNavigationButton(btnGastos, compact);
        SetNavigationButton(btnSalir, compact);

        double contentWidth = Math.Max(0, windowWidth - SidebarColumn.Width.Value - 56);
        bool stackCards = contentWidth < 700;
        HudCardsGrid.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
        HudCardsGrid.ColumnDefinitions[1].Width = stackCards ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
        HudCardsGrid.ColumnDefinitions[2].Width = stackCards ? new GridLength(0) : new GridLength(1, GridUnitType.Star);
        for (int index = 0; index < HudCardsGrid.Children.Count; index++)
        {
            var card = HudCardsGrid.Children[index];
            Grid.SetRow(card, stackCards ? index : 0);
            Grid.SetColumn(card, stackCards ? 0 : index);

            if (card is FrameworkElement element)
            {
                element.Margin = stackCards
                    ? new Thickness(0, 0, 0, 10)
                    : new Thickness(index == 0 ? 0 : 10, 0, index == 2 ? 0 : 10, 10);
            }
        }
    }

    private void SetNavigationButton(Button button, bool compact)
    {
        Geometry iconGeometry = (Geometry)button.Tag;
        Path icon = new()
        {
            Data = iconGeometry,
            Width = 19,
            Height = 19,
            Fill = Brushes.Transparent,
            StrokeThickness = 1.7,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            StrokeLineJoin = PenLineJoin.Round,
            Stretch = Stretch.Uniform,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        icon.SetBinding(Shape.StrokeProperty, new Binding(nameof(Button.Foreground))
        {
            RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Button), 1)
        });

        if (compact)
        {
            button.Content = icon;
        }
        else
        {
            StackPanel content = new()
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };
            content.Children.Add(icon);
            TextBlock label = new()
            {
                Text = button.ToolTip?.ToString() ?? string.Empty,
                FontFamily = (FontFamily)FindResource("ChakraPetchFont"),
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(12, 0, 0, 0)
            };
            label.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(Button.Foreground))
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Button), 1)
            });
            content.Children.Add(label);
            button.Content = content;
        }

        button.HorizontalContentAlignment = compact
            ? HorizontalAlignment.Center
            : HorizontalAlignment.Stretch;
        button.Padding = new Thickness(8, 5, 8, 5);
    }

    private void CargarConsolas()
    {
        PanelConsolas.Children.Clear();

        foreach (var consola in _consolas)
        {
            CrearTarjeta(consola);
        }

        ActualizarDashboard();
    }

    private void CrearTarjeta(Consola consola)
    {
        bool enUso = consola.Ocupada;
        Brush colorEstado = (Brush)FindResource(enUso ? "InUse" : "Free");

        ChamferBorder tarjeta = new()
        {
            Width = 320,
            MinWidth = 290,
            MaxWidth = 340,
            MinHeight = 282,
            Margin = new Thickness(12),
            DataContext = consola,
            Style = (Style)FindResource("ConsoleCardStyle")
        };

        StackPanel contenido = new();
        contenido.Children.Add(new TextBlock
        {
            Text = consola.Tipo.ToUpperInvariant(),
            FontFamily = (FontFamily)FindResource("OrbitronFont"),
            FontSize = 11,
            FontWeight = FontWeights.Bold,
            Foreground = (Brush)FindResource("SectionLabel"),
            HorizontalAlignment = HorizontalAlignment.Left
        });

        contenido.Children.Add(new TextBlock
        {
            Text = consola.Nombre,
            FontFamily = (FontFamily)FindResource("OrbitronFont"),
            FontSize = 19,
            FontWeight = FontWeights.Bold,
            Foreground = (Brush)FindResource("TextPrimary"),
            Margin = new Thickness(0, 7, 0, 10)
        });

        Border estado = new()
        {
            Style = (Style)FindResource("ConsoleStatusPillStyle"),
            Child = new TextBlock
            {
                Style = (Style)FindResource("ConsoleStatusTextStyle"),
                Tag = $"estado_{consola.Id}"
            }
        };
        contenido.Children.Add(estado);

        TimeSpan tiempoMostrado = consola.TiempoJugado;
        if (consola.Ocupada && !consola.TiempoLibre)
        {
            tiempoMostrado = consola.TiempoObjetivo - consola.TiempoJugado;
            if (tiempoMostrado < TimeSpan.Zero)
            {
                tiempoMostrado = TimeSpan.Zero;
            }
        }

        contenido.Children.Add(new TextBlock
        {
            Text = tiempoMostrado.ToString(@"hh\:mm\:ss"),
            Foreground = enUso ? colorEstado : (Brush)FindResource("TextPrimary"),
            FontFamily = (FontFamily)FindResource("ShareTechMonoFont"),
            FontSize = 28,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 14, 0, 6),
            HorizontalAlignment = HorizontalAlignment.Left,
            Tag = $"tiempo_{consola.Id}"
        });

        double progreso = consola.TiempoObjetivo > TimeSpan.Zero
            ? consola.TiempoJugado.TotalSeconds / consola.TiempoObjetivo.TotalSeconds
            : 0;
        contenido.Children.Add(new SegmentBar
        {
            Height = 12,
            Margin = new Thickness(0, 0, 0, 14),
            SegmentCount = 12,
            Progress = progreso
        });

        decimal acumulado = Math.Round((decimal)consola.TiempoJugado.TotalHours * consola.PrecioHora, 2);
        Grid tarifa = new();
        tarifa.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        tarifa.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        TextBlock precio = new()
        {
            Text = $"${consola.PrecioHora:F2}/H",
            Foreground = (Brush)FindResource("Price"),
            FontFamily = (FontFamily)FindResource("ShareTechMonoFont"),
            FontSize = 13
        };
        TextBlock total = new()
        {
            Text = $"ACUMULADO  ${acumulado:F2}",
            Foreground = (Brush)FindResource("TextMuted"),
            FontFamily = (FontFamily)FindResource("ShareTechMonoFont"),
            FontSize = 11,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        Grid.SetColumn(total, 1);
        tarifa.Children.Add(precio);
        tarifa.Children.Add(total);
        contenido.Children.Add(tarifa);

        Button btn = new()
        {
            Content = enUso ? "DETENER Y COBRAR" : "INICIAR",
            Height = 40,
            Margin = new Thickness(0, 16, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Tag = consola,
            Style = (Style)FindResource(enUso ? "StopButtonStyle" : "PrimaryButtonStyle")
        };
        btn.Click += BtnConsola_Click;
        contenido.Children.Add(btn);

        tarjeta.Child = contenido;
        PanelConsolas.Children.Add(tarjeta);
    }

    private void BtnConsola_Click(
    object sender,
    RoutedEventArgs e)
    {
        Button btn = (Button)sender;

        Consola consola =
            (Consola)btn.Tag;

        if (!consola.Ocupada)
        {
            SeleccionarTiempoWindow ventana = new();

            if (ventana.ShowDialog() != true)
                return;

            consola.Ocupada = true;

            consola.HoraInicio =
                DateTime.Now;

            consola.TiempoJugado =
                TimeSpan.Zero;

            consola.TiempoLibre =
                ventana.TiempoLibre;

            consola.TiempoObjetivo =
                ventana.TiempoSeleccionado;
        }
        else
        {
            Sesion sesion;
            try
            {
                sesion = FinalizarSesion(consola);
            }
            catch (DbUpdateException ex)
            {
                MostrarErrorAlFinalizarSesion(ex.GetBaseException().Message);
                return;
            }
            catch (System.Data.Common.DbException ex)
            {
                MostrarErrorAlFinalizarSesion(ex.Message);
                return;
            }
            catch (InvalidOperationException ex)
            {
                MostrarErrorAlFinalizarSesion(ex.Message);
                return;
            }

            ActualizarDashboard();
            MostrarResumenSesion(sesion);
            CargarConsolas();
            return;
        }

        CargarConsolas();
    }
    private void Timer_Tick(
       object? sender,
       EventArgs e)
    {
        foreach (var consola in _consolas)
        {
            if (_sesionesConErrorAlFinalizar.Contains(consola.Id))
            {
                continue;
            }

            if (consola.Ocupada)
            {
                if (consola.TiempoLibre)
                {
                    consola.TiempoJugado =
                        consola.TiempoJugado.Add(
                            TimeSpan.FromSeconds(1));
                }
                else
                {
                    consola.TiempoJugado =
                        consola.TiempoJugado.Add(
                            TimeSpan.FromSeconds(1));

                    if (consola.TiempoJugado >= consola.TiempoObjetivo &&
                        !_sesionesConErrorAlFinalizar.Contains(consola.Id))
                    {
                        if (_configuracionService.Obtener().SonidoAlarma)
                        {
                            System.Media.SystemSounds.Exclamation.Play();
                        }

                        Sesion sesion;
                        try
                        {
                            sesion = FinalizarSesion(consola);
                        }
                        catch (DbUpdateException ex)
                        {
                            _sesionesConErrorAlFinalizar.Add(consola.Id);
                            MostrarErrorAlFinalizarSesion(ex.GetBaseException().Message);
                            continue;
                        }
                        catch (System.Data.Common.DbException ex)
                        {
                            _sesionesConErrorAlFinalizar.Add(consola.Id);
                            MostrarErrorAlFinalizarSesion(ex.Message);
                            continue;
                        }
                        catch (InvalidOperationException ex)
                        {
                            _sesionesConErrorAlFinalizar.Add(consola.Id);
                            MostrarErrorAlFinalizarSesion(ex.Message);
                            continue;
                        }

                        ActualizarDashboard();
                        MostrarResumenSesion(sesion);
                    }
                }
            }
        }

        ActualizarDashboard();
        CargarConsolas();
    }

    private Sesion FinalizarSesion(Consola consola)
    {
        TimeSpan jugado = consola.TiempoJugado;
        DateTime fin = DateTime.Now;
        decimal total = Math.Round(
            (decimal)jugado.TotalHours * consola.PrecioHora,
            2);

        Sesion sesion = new()
        {
            Consola = consola.Nombre,
            Inicio = consola.HoraInicio,
            Fin = fin,
            TiempoJugado = jugado,
            Total = total
        };

        SesionService sesionService = new();
        sesionService.LimpiarRegistrosAntiguos(30);
        sesionService.RegistrarSesionConIngreso(sesion);

        consola.Ocupada = false;
        consola.TiempoJugado = TimeSpan.Zero;
        _sesionesConErrorAlFinalizar.Remove(consola.Id);
        _sesiones.RemoveAll(item => item.Fin < DateTime.Now.AddDays(-30));
        _sesiones.Insert(0, sesion);
        ActualizarHistorial();

        return sesion;
    }

    private void MostrarResumenSesion(Sesion sesion)
    {
        AppDialog.Show(
            this,
            AppDialogType.Success,
            "// SESIÓN FINALIZADA",
            "RESUMEN DE LA SESIÓN",
            new[]
            {
                ("CONSOLA", sesion.Consola),
                ("TIEMPO JUGADO", sesion.TiempoJugado.ToString(@"hh\:mm\:ss"))
            },
            "TOTAL A COBRAR",
            sesion.Total.ToString("C"));
    }

    private void MostrarErrorAlFinalizarSesion(string mensaje)
    {
        AppDialog.Show(
            this,
            AppDialogType.Error,
            "// ERROR AL CERRAR SESIÓN",
            "NO SE PUDO GUARDAR LA SESIÓN",
            $"La consola sigue activa. Intenta detenerla manualmente para volver a guardar.\n{mensaje}");
    }

    private void InicializarConsolas()
    {
        ConsolaService service = new();

        _consolas = service.ObtenerConsolas();

        CargarConsolas();
    }
    private void CargarNombreNegocio()
    {
        var configuracion = _configuracionService.Obtener();
        BrandLabel.Text = configuracion.NombreNegocio;
        txtNombreNegocio.Text = configuracion.NombreNegocio;
    }

    private void ActualizarDashboard()
    {
        int ocupadas =
            _consolas.Count(c => c.Ocupada);

        int libres =
            _consolas.Count(c => !c.Ocupada);

        txtOcupadas.Text =
            ocupadas.ToString();

        txtLibres.Text =
            libres.ToString();

        txtVentas.Text =
            $"${ObtenerVentasDelDia():F2}";

    }
    private void CargarHistorialSesiones()
    {
        SesionService sesionService = new();
        sesionService.LimpiarRegistrosAntiguos(30);
        _sesiones = sesionService.ObtenerUltimos30Dias();
        ActualizarHistorial();
    }

    private void ActualizarHistorial()
    {
        dgHistorial.ItemsSource = null;

        dgHistorial.ItemsSource = _sesiones;
    }
    private void MostrarResumen()
    {
        string resumen = "";

        foreach (var sesion in _sesiones)
        {
            resumen +=
                $"{sesion.Consola}  " +
                $"{sesion.Total:C}\n";
        }

        if (resumen == "")
        {
            resumen = "Sin sesiones aún";
        }

        AppDialog.Show(
            this,
            AppDialogType.Information,
            "// HISTORIAL DE HOY",
            "RESUMEN DE SESIONES",
            resumen);
    }
    private decimal ObtenerVentasDelDia()
    {
        return _cajaService.ObtenerIngresos(DateTime.Today);
    }

    private decimal ObtenerIngresos()
    {
        return _cajaService.ObtenerIngresos();
    }

    private decimal ObtenerGastos()
    {
        return _cajaService.ObtenerGastos();
    }

    private decimal ObtenerGanancia()
    {
        return ObtenerIngresos()
             - ObtenerGastos();
    }

    private void RegistrarGastoPrueba()
    {
        _cajaService.RegistrarGasto(
            "PROVEEDOR",
            "Compra Coca-Cola",
            150);
    }

    private void VenderProducto(
    Producto producto,
    int cantidad)
    {
        if (producto.Stock < cantidad)
        {
            AppDialog.Show(
                this,
                AppDialogType.Warning,
                "// INVENTARIO",
                "STOCK INSUFICIENTE",
                "No hay suficiente stock.");

            return;
        }

        producto.Stock -= cantidad;

        decimal total =
            producto.PrecioVenta * cantidad;

        _cajaService.RegistrarIngreso(
    "PRODUCTO",
    producto.Nombre,
    total);

    }
    private void AbrirInventario()
    {
        InventarioWindow ventana = new();

        ventana.ShowDialog();
    }
    private void btnInventario_Click(
    object sender,
    RoutedEventArgs e)
    {
        AbrirInventario();
    }

    

    private void btnRegistrarVenta_Click(
        object sender,
        RoutedEventArgs e)
    {
        RegistrarVentaWindow ventana = new();
        ventana.Owner = this;

        if (ventana.ShowDialog() == true && ventana.VentaRegistrada is VentaProducto venta)
        {
            ActualizarDashboard();

            AppDialog.Show(
                this,
                AppDialogType.Success,
                "// VENTA REGISTRADA",
                "VENTA COMPLETADA",
                new[]
                {
                    ("PRODUCTO", venta.Producto.Nombre),
                    ("PROVEEDOR", venta.Producto.Proveedor),
                    ("CANTIDAD", venta.Cantidad.ToString()),
                    ("PRECIO UNITARIO", venta.Producto.PrecioVenta.ToString("C"))
                },
                "TOTAL",
                venta.Total.ToString("C"));
        }
    }

    private void AplicarPermisos()
    {
        if (_usuario.Rol == "Empleado")
        {
            btnUsuarios.Visibility =
                Visibility.Collapsed;

            btnReportes.Visibility =
                Visibility.Collapsed;

            btnConfiguracion.Visibility =
                Visibility.Collapsed;
        }
    }
    private void btnUsuarios_Click(
    object sender,
    RoutedEventArgs e)
    {
        UsuariosWindow ventana = new();

        ventana.ShowDialog();
    }

    private void btnConfiguracion_Click(
        object sender,
        RoutedEventArgs e)
    {
        ConfiguracionWindow ventana = new()
        {
            Owner = this
        };

        if (ventana.ShowDialog() == true)
        {
            CargarNombreNegocio();
        }
    }
/*
    private void btnGastos_Click(
        object sender,
        RoutedEventArgs e)
    {
        GastoExtraWindow ventana = new();
        ventana.Owner = this;

        if (ventana.ShowDialog() == true)
        {
            ActualizarDashboard();
        }
    }
*/
    private void dgHistorial_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
    private void btnAdministrarConsolas_Click(
    object sender,
    RoutedEventArgs e)
    {
        AdministrarConsolasWindow ventana =
            new();

        ventana.ShowDialog();

        InicializarConsolas();
    }
    private void btnGastos_Click(
        object sender,
        RoutedEventArgs e)
    {
        GastoExtraWindow ventana =
            new();

        ventana.ShowDialog();
    }

    private void btnReportes_Click(
      object sender,
      RoutedEventArgs e)
    {
        ReportesWindow ventana =
            new();

        ventana.ShowDialog();
    }

    private void btnHistorial_Click(
     object sender,
     RoutedEventArgs e)
    {
        HistorialWindow ventana =
            new();

        ventana.ShowDialog();
    }
    private void btnSalir_Click(
    object sender,
    RoutedEventArgs e)
    {
        bool confirmar = AppDialog.Confirm(
            this,
            "// CONFIRMAR ACCIÓN",
            "SALIR DE CHRONOPLAY",
            "¿Desea salir de ChronoPlay?",
            "Salir");

        if (confirmar)
        {
            Application.Current.Shutdown();
        }
    }
    private void btnCaja_Click(
    object sender,
    RoutedEventArgs e)
    {
        CorteCajaWindow ventana = new();

        ventana.ShowDialog();
    }

}