using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Controls.Inputs;

public partial class Entry : ContentView
{

    public enum AvailableValidationTypes
    {
        EMAIL,
        NONE
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(Entry), string.Empty, BindingMode.TwoWay);
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(Entry), string.Empty);
    public static readonly BindableProperty ValidationRequiredProperty = BindableProperty.Create(nameof(ValidationRequired), typeof(bool), typeof(Entry), false);
    public static readonly BindableProperty EntryKeyboardProperty = BindableProperty.Create(nameof(EntryKeyboard), typeof(Keyboard), typeof(Entry), Keyboard.Default);
    public static readonly BindableProperty ValidationTypeProperty = BindableProperty.Create(nameof(ValidationType), typeof(AvailableValidationTypes), typeof(Entry), AvailableValidationTypes.NONE);
    public static readonly BindableProperty ValidationMessageProperty = BindableProperty.Create(nameof(ValidationMessage), typeof(string), typeof(Entry), string.Empty);

    public string ValidationMessage
    {
        get => (string)GetValue(ValidationMessageProperty);
        set => SetValue(ValidationMessageProperty, value);
    }

    public Keyboard EntryKeyboard
    {
        get => (Keyboard)GetValue(EntryKeyboardProperty);
        set => SetValue(EntryKeyboardProperty, value);
    }

    public bool ValidationRequired
    {
        get => (bool)GetValue(ValidationRequiredProperty);
        set => SetValue(ValidationRequiredProperty, value);
    }

    public AvailableValidationTypes ValidationType
    {
        get => (AvailableValidationTypes)GetValue(ValidationTypeProperty);
        set => SetValue(ValidationTypeProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    private bool _valid = false;

    public bool Valid
    {
        get { return _valid; }
        set { _valid = value; OnPropertyChanged(nameof(Valid)); }
    }

    public Entry()
    {
        InitializeComponent();

        this.PropertyChanged += Entry_PropertyChanged;
        this.Loaded += Entry_Loaded;
    }

    private void Entry_Loaded(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            Valid = false;
        }
    }

    private void Entry_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Text):
                if (!ValidationRequired)
                {
                    Valid = true;
                    break;
                }
                Valid = Validate();
                break;
        }
    }

    private bool Validate()
    {

        switch (ValidationType)
        {
            case AvailableValidationTypes.EMAIL:

                if (string.IsNullOrWhiteSpace(ValidationMessage))
                {
                    ValidationMessage = "You must enter an email address!";
                }

                if (String.IsNullOrWhiteSpace(Text))
                {
                    return false;
                }

                return MyRegex().IsMatch(Text);
            default:
                return true;
        }
    }

    [GeneratedRegex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex MyRegex();
}