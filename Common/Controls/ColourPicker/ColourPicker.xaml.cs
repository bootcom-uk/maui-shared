using System.Windows.Input;

namespace Controls.ColourPicker;

public partial class ColourPicker : ContentView
{

    public ICommand HexEnteredCommand { get; set; }

    public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(ColourPicker), Color.FromRgb(0, 0, 0));

    public Color Color
    {
        get => (Color)GetValue(ColourPicker.ColorProperty);
        set {
            txtHexColor.Text = value.ToHex();
            SetColor(false);
            SetValue(ColourPicker.ColorProperty, value);
        } 
    }

    public ColourPicker()
	{
		InitializeComponent();
        HexEnteredCommand = new Command(() =>
        {
            if (txtHexColor.Text.Length < 3) return;
            SetColorFromHex();
        });

    }

    private void SetColorFromHex()
    {
        var color = Color.FromArgb(txtHexColor.Text);
        this.Color = color;
    }

    internal bool _settingColor;

    private void SetColor(bool fromSlider)
    {
        _settingColor = true;

        var red = redSlider.Value;
        var green = greenSlider.Value;
        var blue = blueSlider.Value;

        var color = Color.FromRgb(red, green, blue);

        if (fromSlider)
        {
            txtHexColor.Text = color.ToHex();
            colorBox.BackgroundColor = Color.FromArgb(txtHexColor.Text);
            _settingColor = false;
            return;
        }
        
        colorBox.BackgroundColor = Color.FromArgb(txtHexColor.Text);
        redSlider.Value = colorBox.BackgroundColor.Red;
        blueSlider.Value = colorBox.BackgroundColor.Blue;
        greenSlider.Value = colorBox.BackgroundColor.Green;
        _settingColor = false;
    }

    private void blueSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if(_settingColor) { return; }
        SetColor(true);
    }

    private void greenSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (_settingColor) { return; }
        SetColor(true);
    }

    private void redSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (_settingColor) { return; }
        SetColor(true);
    }

    private void txtHexColor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_settingColor) { return; }
        SetColor(false);
    }

    private void ContentView_Loaded(object sender, EventArgs e)
    {
        this.txtHexColor.Text = Color.ToArgbHex();
        SetColor(false);
    }
}