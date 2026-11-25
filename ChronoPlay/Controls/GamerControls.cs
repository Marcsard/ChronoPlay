using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChronoPlay.Controls;

public sealed class ChamferBorder : Decorator
{
    public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
        nameof(Background), typeof(Brush), typeof(ChamferBorder),
        new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
        nameof(BorderBrush), typeof(Brush), typeof(ChamferBorder),
        new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
        nameof(BorderThickness), typeof(Thickness), typeof(ChamferBorder),
        new FrameworkPropertyMetadata(new Thickness(1), FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
        nameof(Padding), typeof(Thickness), typeof(ChamferBorder),
        new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty CutSizeProperty = DependencyProperty.Register(
        nameof(CutSize), typeof(double), typeof(ChamferBorder),
        new FrameworkPropertyMetadata(14d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    public static readonly DependencyProperty TopLineBrushProperty = DependencyProperty.Register(
        nameof(TopLineBrush), typeof(Brush), typeof(ChamferBorder),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

    public Brush Background { get => (Brush)GetValue(BackgroundProperty); set => SetValue(BackgroundProperty, value); }
    public Brush BorderBrush { get => (Brush)GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }
    public Thickness BorderThickness { get => (Thickness)GetValue(BorderThicknessProperty); set => SetValue(BorderThicknessProperty, value); }
    public Thickness Padding { get => (Thickness)GetValue(PaddingProperty); set => SetValue(PaddingProperty, value); }
    public double CutSize { get => (double)GetValue(CutSizeProperty); set => SetValue(CutSizeProperty, value); }
    public Brush? TopLineBrush { get => (Brush?)GetValue(TopLineBrushProperty); set => SetValue(TopLineBrushProperty, value); }

    protected override Size MeasureOverride(Size constraint)
    {
        var inset = GetInset();
        var childConstraint = new Size(
            Math.Max(0, constraint.Width - inset.Left - inset.Right),
            Math.Max(0, constraint.Height - inset.Top - inset.Bottom));
        Child?.Measure(childConstraint);
        var desired = Child?.DesiredSize ?? default;
        return new Size(desired.Width + inset.Left + inset.Right, desired.Height + inset.Top + inset.Bottom);
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
        var inset = GetInset();
        var childRect = new Rect(inset.Left, inset.Top,
            Math.Max(0, arrangeSize.Width - inset.Left - inset.Right),
            Math.Max(0, arrangeSize.Height - inset.Top - inset.Bottom));
        if (Child is not null)
        {
            Child.Arrange(childRect);
            Child.Clip = CreateChamferGeometry(childRect.Size, Math.Max(0, CutSize - Math.Max(inset.Left, inset.Top)));
        }
        return arrangeSize;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        var rect = new Rect(RenderSize);
        if (rect.Width <= 0 || rect.Height <= 0)
        {
            return;
        }

        var geometry = CreateChamferGeometry(rect.Size, CutSize);
        drawingContext.DrawGeometry(Background, null, geometry);

        if (BorderBrush is not null && BorderThickness.Left + BorderThickness.Top + BorderThickness.Right + BorderThickness.Bottom > 0)
        {
            double thickness = Math.Max(0.5, (BorderThickness.Left + BorderThickness.Top + BorderThickness.Right + BorderThickness.Bottom) / 4);
            drawingContext.DrawGeometry(null, new Pen(BorderBrush, thickness), geometry);
        }

        if (TopLineBrush is not null && rect.Width > 2 * CutSize)
        {
            drawingContext.DrawLine(new Pen(TopLineBrush, 2), new Point(CutSize, 1), new Point(rect.Width - CutSize, 1));
        }
    }

    private Thickness GetInset() => new(
        Padding.Left + BorderThickness.Left,
        Padding.Top + BorderThickness.Top,
        Padding.Right + BorderThickness.Right,
        Padding.Bottom + BorderThickness.Bottom);

    private static Geometry CreateChamferGeometry(Size size, double cutSize)
    {
        double cut = Math.Min(Math.Max(0, cutSize), Math.Min(size.Width, size.Height) / 2);
        var geometry = new StreamGeometry();
        using (var context = geometry.Open())
        {
            context.BeginFigure(new Point(cut, 0), true, true);
            context.LineTo(new Point(size.Width - cut, 0), true, false);
            context.LineTo(new Point(size.Width, cut), true, false);
            context.LineTo(new Point(size.Width, size.Height - cut), true, false);
            context.LineTo(new Point(size.Width - cut, size.Height), true, false);
            context.LineTo(new Point(cut, size.Height), true, false);
            context.LineTo(new Point(0, size.Height - cut), true, false);
            context.LineTo(new Point(0, cut), true, false);
        }
        geometry.Freeze();
        return geometry;
    }
}

public sealed class SegmentBar : FrameworkElement
{
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
        nameof(Progress), typeof(double), typeof(SegmentBar),
        new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty SegmentCountProperty = DependencyProperty.Register(
        nameof(SegmentCount), typeof(int), typeof(SegmentBar),
        new FrameworkPropertyMetadata(12, FrameworkPropertyMetadataOptions.AffectsRender));

    public double Progress { get => (double)GetValue(ProgressProperty); set => SetValue(ProgressProperty, value); }
    public int SegmentCount { get => (int)GetValue(SegmentCountProperty); set => SetValue(SegmentCountProperty, value); }

    protected override Size MeasureOverride(Size availableSize) => new(availableSize.Width, 12);

    protected override void OnRender(DrawingContext drawingContext)
    {
        int count = Math.Clamp(SegmentCount, 1, 100);
        double gap = 3;
        double height = ActualHeight;
        double segmentWidth = Math.Max(0, (ActualWidth - gap * (count - 1)) / count);
        int activeCount = (int)Math.Ceiling(Math.Clamp(Progress, 0, 1) * count);

        for (int index = 0; index < count; index++)
        {
            double left = index * (segmentWidth + gap);
            double slant = Math.Min(4, height / 3);
            var polygon = new StreamGeometry();
            using (var context = polygon.Open())
            {
                context.BeginFigure(new Point(left + slant, 0), true, true);
                context.LineTo(new Point(left + segmentWidth, 0), true, false);
                context.LineTo(new Point(left + segmentWidth - slant, height), true, false);
                context.LineTo(new Point(left, height), true, false);
            }
            polygon.Freeze();
            var fill = index < activeCount ? new SolidColorBrush(Color.FromRgb(34, 211, 238)) : new SolidColorBrush(Color.FromRgb(27, 37, 71));
            fill.Freeze();
            drawingContext.DrawGeometry(fill, null, polygon);
        }
    }
}

public sealed class AvatarHex : ContentControl
{
    static AvatarHex()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(AvatarHex), new FrameworkPropertyMetadata(typeof(AvatarHex)));
    }
}
