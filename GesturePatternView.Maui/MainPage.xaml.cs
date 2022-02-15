using System.Windows.Input;
using FaulandCc.MAUI.GesturePatternView;

namespace GesturePatternView.Maui;

public partial class MainPage : ContentPage
{
    private string _lastGesture;
    public ICommand GestureCompletedCommand { get; }

	public MainPage()
	{
		InitializeComponent();

        GestureCompletedCommand = new Command(ExecuteGestureCompletedCommand, _ => true);

        BindingContext = this;
    }

    private void MyGesturePatternView_OnGesturePatternCompleted(object sender, GesturePatternCompletedEventArgs e)
    {
        _lastGesture = this.MyGesturePatternView.GesturePatternValue;
		Device.BeginInvokeOnMainThread(DisplayGesturePatternValue);
        this.MyGesturePatternView.Clear();
    }

    private async void DisplayGesturePatternValue()
    {
        await DisplayAlert("Gesture", $"Your code: [{_lastGesture}]", "Ok");
    }

    public void ExecuteGestureCompletedCommand(object gesture)
    {
        _lastGesture = gesture.ToString();
        Device.BeginInvokeOnMainThread(DisplayGesturePatternValue);
        this.MyGesturePatternView.Clear();
    }
}

