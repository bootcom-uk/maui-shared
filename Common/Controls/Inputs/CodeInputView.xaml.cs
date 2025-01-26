namespace Controls.Inputs;

public partial class CodeInputView : ContentView
{
    private readonly List<Microsoft.Maui.Controls.Entry> _entries;

    public static readonly BindableProperty CodeProperty =
        BindableProperty.Create(nameof(Code), typeof(string), typeof(CodeInputView), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty CodeCompleteProperty =
        BindableProperty.Create(nameof(CodeComplete), typeof(bool), typeof(CodeInputView), false, BindingMode.OneWayToSource);

    public string Code
    {
        get => (string)GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    public bool CodeComplete
    {
        get => (bool)GetValue(CodeCompleteProperty);
        set => SetValue(CodeCompleteProperty, value);
    }

    public CodeInputView()
    {
        InitializeComponent();
        _entries = new List<Microsoft.Maui.Controls.Entry> { Digit0, Digit1, Digit2, Digit3, Digit4, Digit5 };

        foreach (var entry in _entries)
        {
            entry.TextChanged += OnTextChanged;
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Microsoft.Maui.Controls.Entry;

        // Move to the next or previous input box
        if (entry!.Text.Length == 1)
        {
            MoveFocusForward(entry);
        }
        else if (string.IsNullOrEmpty(entry.Text))
        {
            MoveFocusBackward(entry);
        }

       ValidateCode();
        
    }

    private void MoveFocusForward(Microsoft.Maui.Controls.Entry currentEntry)
    {
        int index = _entries.IndexOf(currentEntry);
        if (index < _entries.Count - 1)
        {
            _entries[index + 1].Focus();
        }
    }

    private void MoveFocusBackward(Microsoft.Maui.Controls.Entry currentEntry)
    {
        int index = _entries.IndexOf(currentEntry);
        if (index > 0)
        {
            _entries[index - 1].Focus();
        }
    }

    private void ValidateCode()
    {

        Code = string.Concat(_entries.Select(x => x.Text));
        CodeComplete = Code.Length == 6;

        foreach (var entry in _entries)
        {
            entry.BackgroundColor = CodeComplete ? Colors.LightGreen : Colors.LightCoral;
        }
    }
}