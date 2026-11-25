using System.Windows;
using System.Windows.Controls;

namespace ChronoPlay.Controls;

public partial class UserAvatar : UserControl
{
    public static readonly DependencyProperty UserNameProperty = DependencyProperty.Register(
        nameof(UserName), typeof(string), typeof(UserAvatar), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty RoleProperty = DependencyProperty.Register(
        nameof(Role), typeof(string), typeof(UserAvatar), new PropertyMetadata(string.Empty));

    public string UserName
    {
        get => (string)GetValue(UserNameProperty);
        set => SetValue(UserNameProperty, value);
    }

    public string Role
    {
        get => (string)GetValue(RoleProperty);
        set => SetValue(RoleProperty, value);
    }

    public UserAvatar()
    {
        InitializeComponent();
    }
}
