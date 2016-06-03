using Windows.UI.Xaml.Controls;





// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HomeControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


        public MainPage()
        {
            this.InitializeComponent();
            //Storyboard1.Begin();
            mediaElement.Volume = 5;

            mediaElement.Play();
            Storyboard1.Begin();




        }

     




        private void button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //Storyboard1.Stop();
            // mediaElement.Stop();

            //this.Frame.Navigate(typeof(BudgetView));

        }

        private void History_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HistoryView));
        }

        private void budgetBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BudgetView));
        }

        private void loanBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GetALoan));
        }

        private void quitBarButton1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            

        }
    }
}
   


