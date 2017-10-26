using Xamarin.Forms;

namespace GesturePatternViewXF
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void MyGesturePatternView_OnGesturePatternCompleted(object sender, GesturePatternCompletedEventArgs e)
        {
            await DisplayAlert("Gesture", e.GesturePatternValue, "Ok", "Cancel");
            this.MyGesturePatternView.Clear();
        }
    }
}
