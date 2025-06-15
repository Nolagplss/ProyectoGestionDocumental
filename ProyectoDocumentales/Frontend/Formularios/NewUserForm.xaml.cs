using NLog;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Backend.Modelo;
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
    /// Lógica de interacción para NewUserForm.xaml
    /// </summary>
    public partial class NewUserForm : Window
    {

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();


        private MvUsuarios _mvUsuarios;

        private string _tituloUserForm;

        public NewUserForm(MvUsuarios mvUsuarios, string tituloUserForm)
        {
            InitializeComponent();
            logger.Info("Inicializando NewUserForm con título ", tituloUserForm);


            _mvUsuarios = mvUsuarios;


            this.DataContext = _mvUsuarios;

            CargarPermiso();

            _tituloUserForm = tituloUserForm;
            tituloForm.Text = _tituloUserForm;
 

            //Si añadimos nuevo usuario limpiarmos el formulario por si edito antes para que se limpie el objeto
            if(_tituloUserForm.Contains("Añadir nuevo usuario"))
            {
                logger.Debug("Limpiando formulario al iniciar para nuevo usuario.");

                LimpiarFormulario();
            }

            if (_tituloUserForm.Contains("Editar Usuario"))
            {
                _mvUsuarios.CrearCopiaSeguridad();
                logger.Debug("Copia de seguridad creada para edición de usuario.");
            }





            _mvUsuarios.RegisterValidationSection("usuario", btnGuardarUsuario,
               "usuario.Nombre", "usuario.Apellidos","usuario.Correo", "usuario.Dni", "usuario.Contrasenia", "cmbRol"
             );


          
            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(_mvUsuarios.OnErrorEvent));
            ValidatePasswordBox();



        }

        private void CargarPermiso()
        {
            if (!RoleManager.HasPermission("PASSWORD_CHANGE_ALL"))
            {

                txtContrasenia.IsEnabled = false;
                logger.Debug("Deshabilitado el campo de contraseña por permisos insuficientes.");

            }
        }
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Cancelando operación en formulario con título: ", _tituloUserForm);

            switch (_tituloUserForm)
            {
                case "Añadir nuevo usuario":
                    LimpiarFormulario();
                    _mvUsuarios.RoleSeleccionado = null;
                    break;

                case "Editar Usuario":
                    _mvUsuarios.RestaurarUsuario();
                    _mvUsuarios.RoleSeleccionado = null;
                    this.Close();

                    break;
            }
           
        }

        private void LimpiarFormulario()
        {

            logger.Debug("Limpiando formulario y reseteando usuario en ViewModel.");

            cmbRol.SelectedValue = null;

            txtNombre.Text = string.Empty;
            txtApellidos.Text = string.Empty;
            txtDni.Text = string.Empty;
            txtCorreo.Text = string.Empty;
            txtObservaciones.Text = string.Empty;
            txtContrasenia.Password = string.Empty;

            _mvUsuarios.usuario = new Usuario();

        }


        private async void btnGuardarUsuario_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.Info("Intentando guardar usuario, modo: ", _tituloUserForm);

                switch (_tituloUserForm)
                {
                    case "Añadir nuevo usuario":

                        if (await _mvUsuarios.GuardarUsuario())
                        {
                            logger.Info("Usuario guardado correctamente.");

                            MessageBox.Show("Usuario guardado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);


                            LimpiarFormulario();



                        }
                        else
                        {
                            logger.Warn("Error al guardar el usuario.");

                            MessageBox.Show("Error al guardar el usuario", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    break;

                    case "Editar Usuario":

                        if (await _mvUsuarios.ActualizarUsuario())
                        {
                            logger.Info("Usuario actualizado correctamente.");

                            MessageBox.Show("Usuario Actualizado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);


                            this.Close();
                           
                        }
                        else
                        {
                            logger.Warn("Error al actualizar el usuario.");

                            MessageBox.Show("Error al Actualizar el usuario", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    break;
                        

                }


            }
            catch (Exception ex)
            {
                logger.Error(ex, "Excepción al guardar usuario.");

                MessageBox.Show($"Error al guardar el usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Cerrando formulario con título: {0}", _tituloUserForm);

            switch (_tituloUserForm)
            {
                case "Añadir nuevo usuario":
                    LimpiarFormulario();
                    _mvUsuarios.RoleSeleccionado = null;
                    this.Close();
                    break;

                case "Editar Usuario":
                    _mvUsuarios.RestaurarUsuario();
                    _mvUsuarios.RoleSeleccionado = null;
                    this.Close();
                    break;
            }
           
            
        }

      

        private void cmbRol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (cmbRol.SelectedItem != null)
            {
                var rolSeleccionado = (Role)cmbRol.SelectedItem;
                _mvUsuarios.usuario.IdRolNavigation = rolSeleccionado;
                _mvUsuarios.usuario.IdRol = rolSeleccionado.IdRol;
            }
            else
            {
                _mvUsuarios.usuario.IdRolNavigation = null;
                _mvUsuarios.usuario.IdRol = null;
            }

        }
      
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove(); //Permite mover la ventana al arrastrar
            }
        }
        private void ValidatePasswordBox()
        {
            var password = txtContrasenia.Password;

            if (string.IsNullOrWhiteSpace(password))
            {
                txtContrasenia.Style = (Style)FindResource("PasswordBoxErrorStyle");
                txtContraseniaError.Visibility = Visibility.Visible;
                logger.Debug("Contraseña vacía o nula, mostrando error visual.");

            }
            else
            {
                txtContrasenia.Style = (Style)FindResource("PasswordBoxStyle");
                txtContraseniaError.Visibility = Visibility.Collapsed;
            }
        }
        private void txtContrasenia_PasswordChanged(object sender, RoutedEventArgs e)
        {

            _mvUsuarios.usuario.Contrasenia = txtContrasenia.Password;
            txtContraseniaHidden.Text = txtContrasenia.Password;

            var bindingExpression = txtContraseniaHidden.GetBindingExpression(TextBox.TextProperty);
            bindingExpression?.UpdateSource();

            ValidatePasswordBox();
            logger.Debug("Contraseña modificada en el formulario.");



        }
    }
}
