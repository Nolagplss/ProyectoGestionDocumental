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
    /// Lógica de interacción para CompanyForm.xaml
    /// </summary>
    public partial class EmpresaForm : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MvEmpresa _mvEmpresa;

        //Callback que se ejecutara al guardar para actualizar la lista de empresas
        private Action _onEmpresaGuardada;

        public EmpresaForm(MvEmpresa mvEmpresa, Action onEmpresaGuardada = null)
        {
            InitializeComponent();

            _mvEmpresa = mvEmpresa;
            _onEmpresaGuardada = onEmpresaGuardada;
            this.DataContext = _mvEmpresa;

           _mvEmpresa.btnGuardar = btnGuardarEmpresa;

            _mvEmpresa.RegisterValidationSection("empresa", btnGuardarEmpresa,
               "empresa.RazonSocial", "empresa.Cif", "empresa.Direccion", "empresa.Telefono", "empresa.Localidad",
               "empresa.Provincia", "empresa.CodigoPostal", "empresa.Sector", "cmbResponsable");

            //Registrar la seccion del responsable con sus campos especificos
            _mvEmpresa.RegisterValidationSection("responsable", btnGuardarResponsable,
                "responsable.Nombre", "responsable.Apellidos", "responsable.Dni", "responsable.Correo");


            //Registrar la seccion del centro de trabajo con sus campos especificos
            _mvEmpresa.RegisterValidationSection("centroTrabajo", btnAnadirCentro,
               "centroTrabajo.Direccion", "centroTrabajo.Telefono");


             this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(_mvEmpresa.OnErrorEvent));

            logger.Info("Formulario EmpresaForm inicializado.");


        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Cierre del formulario EmpresaForm solicitado.");

            this.Close();
        }

        private void cmbResponsable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void btnGuardarResponsable_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                logger.Info("Intentando guardar responsable.");

                //Asignar valores del formulario al objeto responsable
                _mvEmpresa.responsable.Nombre = txtNombreResponsable.Text;
                _mvEmpresa.responsable.Apellidos = txtApellidosResponsable.Text;
                _mvEmpresa.responsable.Dni = txtDniResponsable.Text;
                _mvEmpresa.responsable.Correo = txtCorreoResponsable.Text;


                if (await _mvEmpresa.GuardaResponsable())
                {
                    logger.Info("Responsable guardado correctamente.");

                    MessageBox.Show("Responsable guardado correctamente", "Actualización del responsable", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarFormularioResponsable();

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al guardar el responsable.");

                MessageBox.Show($"Error al guardar el responsable: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        

        private async void LimpiarFormulario(bool recargarCentros)
        {
            logger.Info("Limpiando formulario empresa. Recargar centros: " + recargarCentros);

            // Datos de empresa
            txtRazonSocial.Text = string.Empty;
            txtCIF.Text = string.Empty;
            txtSector.Text = string.Empty;
            cmbResponsable.SelectedIndex = -1;

            txtDireccion.Text = string.Empty;
            txtLocalidad.Text = string.Empty;
            txtProvincia.Text = string.Empty;
            txtCodigoPostal.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            txtFax.Text = string.Empty;

            _mvEmpresa.empresa = new Empresa();

            LimpiarFormularioResponsable();
            LimpiarFormularioCentroTrabajo();

            // Limpiar centros seleccionados y recargar libres
            _mvEmpresa.CentrosSeleccionados.Clear();

            if (recargarCentros)
            {
                await _mvEmpresa.CargarCentrosLibres();
            }

            // Colapsar expansores si estaban abiertos
            expanderResponsable.IsExpanded = false;
        }

        private void LimpiarFormularioResponsable()
        {
            logger.Info("Limpiando formulario responsable.");

            // Responsable nuevo
            txtNombreResponsable.Text = string.Empty;
            txtApellidosResponsable.Text = string.Empty;
            txtDniResponsable.Text = string.Empty;
            txtCorreoResponsable.Text = string.Empty;

            _mvEmpresa.responsable = new Responsable();
        }

        private void LimpiarFormularioCentroTrabajo()
        {
            logger.Info("Limpiando formulario centro de trabajo.");

            // Centro de trabajo nuevo
            txtDireccionCentro.Text = string.Empty;
            txtTelefonoCentro.Text = string.Empty;

            _mvEmpresa.centroTrabajo = new CentrosTrabajo();
        }

        private void btnEliminarCentro_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CentrosTrabajo centro)
            {
                logger.Info($"Eliminando centro de trabajo: {centro.Direccion}");

                _mvEmpresa.QuitarCentroDeLaEmpresa(centro);
            }

        }

        private async void btnAnadirCentro_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.Info("Intentando añadir centro de trabajo.");


                //Asignar valores del formulario al objeto centroTrabajo
                _mvEmpresa.centroTrabajo.Direccion = txtDireccionCentro.Text;
                _mvEmpresa.centroTrabajo.Telefono = txtTelefonoCentro.Text;
            


                if (await _mvEmpresa.GuardarCentroTrabajo())
                {
                    logger.Info("Centro de trabajo añadido correctamente.");

                    MessageBox.Show("Centro de trabajo añadido correctamente", "Actualización del Centro de Trabajo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LimpiarFormularioCentroTrabajo();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al guardar el centro de trabajo.");

                MessageBox.Show($"Error al guardar el centro de trabajo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        private void btnSeleccionarCentro_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CentrosTrabajo centro)
            {
                logger.Info($"Añadiendo centro de trabajo a la empresa: {centro.Direccion}");

                _mvEmpresa.AnadirCentroAEmpresa(centro);
            }
        }


        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Cancelando edición y limpiando formulario.");

            LimpiarFormulario(true);
        }

        private async void btnGuardarEmpresa_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (await _mvEmpresa.GuardarEmpresa())
                {
                    logger.Info("Empresa guardada correctamente.");

                    MessageBox.Show("Empresa guardada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                    //Ejecutar el callback si existe
                    _onEmpresaGuardada?.Invoke();

                    LimpiarFormulario(false);
                }
                else
                {
                    logger.Warn("Error al guardar la empresa.");

                    MessageBox.Show("Error al guardar la empresa", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al guardar la empresa.");

                MessageBox.Show($"Error al guardar la empresa: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove(); //Permite mover la ventana al arrastrar
            }
        }
    }
}
