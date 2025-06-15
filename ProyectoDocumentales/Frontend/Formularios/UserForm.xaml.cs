using NLog;
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
    /// Lógica de interacción para UserForm.xaml
    /// </summary>
    public partial class UserForm : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MvUsuario _mvUsuario;

        public UserForm(MvUsuario mvUsuario)
        {
            InitializeComponent();
            _mvUsuario = mvUsuario;
            DataContext = _mvUsuario;
            logger.Info("UserForm inicializado para el usuario.");

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Formulario cerrado mediante btnCerrar.");

            this.Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Formulario cerrado mediante btnCancelar.");

            this.Close();
        }

        private async void btnCambiarPassword_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Intento de cambio de contraseña iniciado.");

            try
            {
                //Validar que todos los campos de contraseña esten llenos
                if (string.IsNullOrWhiteSpace(txtPasswordActual.Password))
                {
                    logger.Warn("Cambio de contraseña falló: contraseña actual vacía.");

                    MessageBox.Show("Debe ingresar su contraseña actual", "Error de validación",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPasswordActual.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPasswordNueva.Password))
                {
                    logger.Warn("Cambio de contraseña falló: nueva contraseña vacía.");

                    MessageBox.Show("Debe ingresar la nueva contraseña", "Error de validación",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPasswordNueva.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPasswordConfirmar.Password))
                {
                    logger.Warn("Cambio de contraseña falló: confirmación de contraseña vacía.");

                    MessageBox.Show("Debe confirmar la nueva contraseña", "Error de validación",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPasswordConfirmar.Focus();
                    return;
                }

                //Validar que las contraseñas nuevas coincidan
                if (txtPasswordNueva.Password != txtPasswordConfirmar.Password)
                {
                    logger.Warn("Cambio de contraseña falló: las nuevas contraseñas no coinciden.");

                    MessageBox.Show("Las contraseñas nuevas no coinciden", "Error de validación",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtPasswordConfirmar.Focus();
                    txtPasswordConfirmar.SelectAll();
                    return;
                }

                //Confirmar el cambio
                var resultado = MessageBox.Show("¿Está seguro que desea cambiar su contraseña?",
                                              "Confirmar cambio",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (resultado == MessageBoxResult.Yes)
                {
                    logger.Info("Usuario confirmó el cambio de contraseña.");

                    //Llamar al metodo para cambiar la contraseña
                    bool exito = await _mvUsuario.CambiarContrasena(txtPasswordActual.Password, txtPasswordNueva.Password);

                    if (exito)
                    {
                        logger.Info("Contraseña cambiada con éxito.");

                        MessageBox.Show("Contraseña cambiada correctamente", "Éxito",
                                      MessageBoxButton.OK, MessageBoxImage.Information);

                        //Limpiar los campos de contraseña
                        txtPasswordActual.Clear();
                        txtPasswordNueva.Clear();
                        txtPasswordConfirmar.Clear();

                        this.Close();
                    }
                    else
                    {
                        logger.Warn("Cambio de contraseña falló: contraseña actual incorrecta.");

                        MessageBox.Show("La contraseña actual no es correcta", "Error",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                        txtPasswordActual.Focus();
                        txtPasswordActual.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Excepción al cambiar la contraseña.");

                MessageBox.Show($"Error al cambiar la contraseña: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
