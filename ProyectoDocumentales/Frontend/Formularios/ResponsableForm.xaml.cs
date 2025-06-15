using NLog;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.MVVM;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProyectoDocumentales.Frontend.Formularios
{
    /// <summary>
    /// Lógica de interacción para ResponsableForm.xaml
    /// </summary>
    public partial class ResponsableForm : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MvResponsables _mvResponsables;
        private string _tituloResponsableForm;

        public ResponsableForm(MvResponsables mvResponsables, string tituloResponsableForm)
        {
            InitializeComponent();

            logger.Info("Inicializando ResponsableForm con título: {0}", tituloResponsableForm);

            _mvResponsables = mvResponsables;

            this.DataContext = _mvResponsables;

            _tituloResponsableForm = tituloResponsableForm;
            tituloForm.Text = _tituloResponsableForm;

            // Si añadimos nuevo responsable limpiamos el formulario por si editó antes para que se limpie el objeto
            if (_tituloResponsableForm.Contains("Añadir nuevo responsable"))
            {
                logger.Debug("Limpiando formulario al iniciar para nuevo responsable.");

                LimpiarFormulario();
            }
            if (_tituloResponsableForm.Contains("Editar Responsable"))
            {
                _mvResponsables.CrearCopiaSeguridad();
                logger.Debug("Copia de seguridad creada para edición de responsable.");
            }

            // Registrar validación
            _mvResponsables.RegisterValidationSection("responsable", btnGuardarResponsable,
               "responsable.Nombre", "responsable.Apellidos", "responsable.Correo", "responsable.Dni"
             );

            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(_mvResponsables.OnErrorEvent));
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Cancelando operación en formulario con título: {0}", _tituloResponsableForm);

            switch (_tituloResponsableForm)
            {
                case "Añadir nuevo responsable":
                    LimpiarFormulario();
                    break;

                case "Editar Responsable":
                    _mvResponsables.RestaurarResponsable();
                    this.Close();
                    break;
            }
        }

        private void LimpiarFormulario()
        {
            logger.Debug("Limpiando formulario y reseteando responsable en ViewModel.");

            txtNombre.Text = string.Empty;
            txtApellidos.Text = string.Empty;
            txtDni.Text = string.Empty;
            txtCorreo.Text = string.Empty;

            _mvResponsables.responsable = new Responsable();
        }

        private async void btnGuardarResponsable_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Intentando guardar responsable, modo: {0}", _tituloResponsableForm);

            try
            {
                switch (_tituloResponsableForm)
                {
                    case "Añadir nuevo responsable":

                        if (await _mvResponsables.GuardarResponsable())
                        {
                            logger.Info("Responsable guardado correctamente.");

                            MessageBox.Show("Responsable guardado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            LimpiarFormulario();
                        }
                        else
                        {
                            logger.Warn("Error al guardar el responsable.");

                            MessageBox.Show("Error al guardar el responsable", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;

                    case "Editar Responsable":

                        if (await _mvResponsables.ActualizarResponsable())
                        {
                            logger.Info("Responsable actualizado correctamente.");

                            MessageBox.Show("Responsable actualizado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                        else
                        {
                            logger.Warn("Error al actualizar el responsable.");

                            MessageBox.Show("Error al actualizar el responsable", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Excepción al procesar responsable.");

                MessageBox.Show($"Error al procesar el responsable: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Cerrando formulario con título: {0}", _tituloResponsableForm);

            switch (_tituloResponsableForm)
            {
                case "Añadir nuevo responsable":
                    LimpiarFormulario();
                    this.Close();
                    break;

                case "Editar Responsable":
                    _mvResponsables.RestaurarResponsable();
                    this.Close();
                    break;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove(); // Permite mover la ventana al arrastrar
            }
        }
    }
}
