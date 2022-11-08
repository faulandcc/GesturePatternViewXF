using System.Windows.Input;

namespace GesturePatternView.Maui
{
    public partial class MainPage : ContentPage
    {
        public ICommand GestureCompletedCommand { get; }

        private string _gestureValue;

        public string GestureValue
        {
            get => _gestureValue;
            set
            {
                if (_gestureValue == value) return;

                OnPropertyChanging();
                _gestureValue = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            InitializeComponent();

            GestureCompletedCommand = new Command(ExecuteGestureCompletedCommand, _ => true);

            BindingContext = this;
        }

        public void ExecuteGestureCompletedCommand(object gesture)
        {
            GestureValue = gesture.ToString();
            DisplayAlert("Gesture", $"Your code: [{GestureValue}]", "Ok");
            this.MyGesturePatternView.Clear();
        }
    }
}