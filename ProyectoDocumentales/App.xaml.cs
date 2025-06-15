using ProyectoDocumentales.Frontend;
using ProyectoDocumentales.Frontend.Login;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ProyectoDocumentales
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Exception ex = (Exception)e.ExceptionObject;
                MessageBox.Show($"Error :\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {

            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            base.OnStartup(e);

            System.Windows.Media.RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;

            //Mostrar ventana splash con animacion
            var splash = new Frontend.SplashScreen();
            splash.Show();

            Task.Run(async () =>
            {
                await Task.Delay(5000);

                //Devolver al hilo UI
                splash.Dispatcher.Invoke(() =>
                {


                    var loginForm = new LoginForm();

                    // Asignar ventana principal para controlar cierre de app
                    Application.Current.MainWindow = loginForm;

                    loginForm.Show();
                    splash.Close();
                });
            });

        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"An unhandled error has occurred:\n\n{e.Exception.Message}", "Critical Error");
            e.Handled = true; // Evita que se cierre la app
        }

    }


}
