using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ChronoPlay.Controls;

public static class WindowWorkAreaBehavior
{
    private const int WmGetMinMaxInfo = 0x0024;
    private const uint MonitorDefaultToNearest = 0x00000002;

    public static readonly DependencyProperty KeepInWorkAreaProperty =
        DependencyProperty.RegisterAttached(
            "KeepInWorkArea",
            typeof(bool),
            typeof(WindowWorkAreaBehavior),
            new PropertyMetadata(false, OnKeepInWorkAreaChanged));

    public static bool GetKeepInWorkArea(DependencyObject element) =>
        (bool)element.GetValue(KeepInWorkAreaProperty);

    public static void SetKeepInWorkArea(DependencyObject element, bool value) =>
        element.SetValue(KeepInWorkAreaProperty, value);

    private static void OnKeepInWorkAreaChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not Window window)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            window.SourceInitialized += Window_SourceInitialized;
            window.Closed += Window_Closed;

            if (PresentationSource.FromVisual(window) is HwndSource source)
            {
                source.AddHook(WindowHook);
            }

            return;
        }

        window.SourceInitialized -= Window_SourceInitialized;
        window.Closed -= Window_Closed;

        if (PresentationSource.FromVisual(window) is HwndSource existingSource)
        {
            existingSource.RemoveHook(WindowHook);
        }
    }

    private static void Window_SourceInitialized(object? sender, EventArgs e)
    {
        if (sender is Window window &&
            new WindowInteropHelper(window).Handle is var handle &&
            handle != IntPtr.Zero &&
            HwndSource.FromHwnd(handle) is HwndSource source)
        {
            source.AddHook(WindowHook);
        }
    }

    private static void Window_Closed(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            window.SourceInitialized -= Window_SourceInitialized;
            window.Closed -= Window_Closed;

            if (PresentationSource.FromVisual(window) is HwndSource source)
            {
                source.RemoveHook(WindowHook);
            }
        }
    }

    private static IntPtr WindowHook(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (message != WmGetMinMaxInfo || lParam == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        IntPtr monitor = MonitorFromWindow(hwnd, MonitorDefaultToNearest);
        if (monitor == IntPtr.Zero)
        {
            return IntPtr.Zero;
        }

        MonitorInfo monitorInfo = new()
        {
            Size = Marshal.SizeOf<MonitorInfo>()
        };

        if (!GetMonitorInfo(monitor, ref monitorInfo))
        {
            return IntPtr.Zero;
        }

        MinMaxInfo minMaxInfo = Marshal.PtrToStructure<MinMaxInfo>(lParam);
        minMaxInfo.MaxPosition.X = monitorInfo.Work.Left - monitorInfo.Monitor.Left;
        minMaxInfo.MaxPosition.Y = monitorInfo.Work.Top - monitorInfo.Monitor.Top;
        minMaxInfo.MaxSize.X = monitorInfo.Work.Right - monitorInfo.Work.Left;
        minMaxInfo.MaxSize.Y = monitorInfo.Work.Bottom - monitorInfo.Work.Top;
        minMaxInfo.MaxTrackSize = minMaxInfo.MaxSize;

        Marshal.StructureToPtr(minMaxInfo, lParam, true);
        handled = true;
        return IntPtr.Zero;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint flags);

    [DllImport("user32.dll", EntryPoint = "GetMonitorInfoW", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo monitorInfo);

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MinMaxInfo
    {
        public Point Reserved;
        public Point MaxSize;
        public Point MaxPosition;
        public Point MinTrackSize;
        public Point MaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MonitorInfo
    {
        public int Size;
        public Rect Monitor;
        public Rect Work;
        public uint Flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}
