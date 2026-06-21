using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace ContactToVCard.Controls;

public partial class ActionSummaryControl : UserControl
{
    public static readonly StyledProperty<string> ButtonTextProperty =
        AvaloniaProperty.Register<ActionSummaryControl, string>(nameof(ButtonText), string.Empty);

    public static readonly StyledProperty<string> SummaryTextProperty =
        AvaloniaProperty.Register<ActionSummaryControl, string>(nameof(SummaryText), string.Empty);

    public static readonly StyledProperty<ICommand?> ActionCommandProperty =
        AvaloniaProperty.Register<ActionSummaryControl, ICommand?>(nameof(ActionCommand));

    public string ButtonText
    {
        get => GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    public string SummaryText
    {
        get => GetValue(SummaryTextProperty);
        set => SetValue(SummaryTextProperty, value);
    }

    public ICommand? ActionCommand
    {
        get => GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    public ActionSummaryControl()
    {
        InitializeComponent();
    }
}