using ProyectoDocumentales.Frontend.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProyectoDocumentales.Frontend
{
    /// <summary>
    /// Lógica de interacción para SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Simula un tiempo de carga
            await Task.Delay(4000);

            //Creamos una animacion de opacidad para desvanecer la ventana
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(1)
            };

            //Cuando la animacion termine, cerramos la ventana
            fadeOutAnimation.Completed += (s, _) =>
            {
                //Abre la ventana principal
                Application.Current.MainWindow = new LoginForm();
                Application.Current.MainWindow.Show();

                // Cierra la pantalla de splash
                this.Close();
            };

            //Inicia la animacion
            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }
    }
}
