using NLog;
using ProyectoDocumentales.MVVM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProyectoDocumentales.Frontend.Formularios
{
    /// <summary>
    /// Lógica de interacción para CentroEducativoForm.xaml
    /// </summary>
    public partial class CentroEducativoForm : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MvCentrosEducativo _mvCentrosEducativo;
        public CentroEducativoForm(MvCentrosEducativo mvCentrosEducativo)
        {
            InitializeComponent();
            _mvCentrosEducativo = mvCentrosEducativo;
            DataContext = _mvCentrosEducativo;

            _mvCentrosEducativo.RegisterValidationSection("Centro", btnGuardarCentro,
             "Centro.Nombre", "Centro.Cif", "Centro.Telefono", "Centro.CodigoPostal", "Centro.Director"
           );



            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(_mvCentrosEducativo.OnErrorEvent));

            logger.Info("Formulario CentroEducativoForm inicializado.");


        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Cerrando formulario CentroEducativoForm, recargando centro.");

            //Volvemos a establecer el centro para que el formulario se restablezca como siempre
            _mvCentrosEducativo.RestaurarCambios();

            this.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {

            var resultado = MessageBox.Show("¿Está seguro de que desea cancelar? Se perderán los cambios no guardados.",
                                          "Confirmar cancelación",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (resultado == MessageBoxResult.Yes)
            {
                logger.Info("Cancelación confirmada, recargando centro y cerrando formulario.");

                //Volvemos a establecer el centro para que el formulario se restablezca como siempre
                _mvCentrosEducativo.RestaurarCambios();
                this.Close();
            }
        }

        private async void btnGuardarCentro_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.Info("Intentando guardar datos del centro.");

                //Guardamos
                bool guardadoExitoso = await _mvCentrosEducativo.ActualizarCentro();
                await _mvCentrosEducativo.CargarCentroDelUsuario();

                if (guardadoExitoso)
                {
                    logger.Info("Datos del centro guardados correctamente, cerrando formulario.");


                    this.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al guardar los datos del centro.");

                MessageBox.Show($"Error al guardar los datos del centro: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

    

    }
}
