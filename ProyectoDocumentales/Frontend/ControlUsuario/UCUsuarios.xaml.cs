using NLog;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Frontend.Formularios;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoDocumentales.Frontend.ControlUsuario
{
    /// <summary>
    /// Lógica de interacción para UCUsuarios.xaml
    /// </summary>
    public partial class UCUsuarios : UserControl
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MvUsuarios _mvUsuarios;

        public UCUsuarios(MvUsuarios mvUsuarios)
        {
            InitializeComponent();
            _mvUsuarios = mvUsuarios;
            this.DataContext = _mvUsuarios;
            logger.Info("UCUsuarios inicializado.");

        }

        private void btnAplicarFiltros_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Aplicando filtros en usuarios.");

            _mvUsuarios.Filtrar();
        }

        private async void btnBorrarFiltros_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Borrando filtros en usuarios.");

            _mvUsuarios.txtDni = string.Empty;
            _mvUsuarios.RoleSeleccionado = null;
            _mvUsuarios.FiltrosActivos = false;
            _mvUsuarios.PaginaActual = 1;
            await _mvUsuarios.CargarPaginaAsync();
        }

        private void btnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Abriendo formulario para añadir nuevo usuario.");

            NewUserForm newUserForm = new NewUserForm(_mvUsuarios, "Añadir nuevo usuario");
            newUserForm.ShowDialog();
        }

        private async void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            //Asigna el usuario seleccionado en el DataGrid al ViewModel
            _mvUsuarios.usuario = (Usuario)dgUsuarios.SelectedItem;

            //Crea y muestra el dialogo de edicion de usuario
            NewUserForm newUserForm = new NewUserForm(_mvUsuarios, "Editar Usuario");
            newUserForm.ShowDialog();

            //Si el usuario confirma la edicion, refresca la tabla
            if (newUserForm.DialogResult.Equals(true))
            {
                logger.Info("Usuario editado, actualizando lista.");

                //Actualizar la lista de usuarios
                await _mvUsuarios.ActualizarListaUsuarios();
            }
        }

        private async void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Cogemos el usuario desde el tag del boton
                Button button = sender as Button;
                Usuario usuarioSeleccionado = button?.Tag as Usuario;

                if (usuarioSeleccionado == null)
                {
                    logger.Warn("No se pudo obtener el usuario seleccionado para eliminar.");

                    MessageBox.Show("No se pudo obtener el usuario seleccionado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                logger.Info($"Intento de eliminar usuario ID {usuarioSeleccionado.IdUsuario}.");

                //Confirmar la eliminacion con el usuario
                var result = MessageBox.Show(
                    $"¿Está seguro que desea eliminar al usuario {usuarioSeleccionado.Nombre} {usuarioSeleccionado.Apellidos}?\n\n" +
                    "Esta acción no se puede deshacer.",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    //Eliminar el usuario usando el servicio
                    bool eliminado = await _mvUsuarios.Delete(usuarioSeleccionado);

                    if (eliminado)
                    {
                        logger.Info($"Usuario ID {usuarioSeleccionado.IdUsuario} eliminado correctamente.");

                        MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        // Actualizar la lista de usuarios
                        await _mvUsuarios.ActualizarListaUsuarios();
                    }
                    else
                    {
                        logger.Warn($"Error al eliminar usuario ID {usuarioSeleccionado.IdUsuario}.");

                        MessageBox.Show("Error al eliminar el usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error inesperado al eliminar usuario.");

                MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ComboRoles_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            combo.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void ComboRoles_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            combo.Foreground = new SolidColorBrush(Colors.White);
        }

        private async void DarBajaUsuario_Click(object sender, RoutedEventArgs e)
        {
            Usuario usuarioSeleccionado = dgUsuarios.SelectedItem as Usuario;

            if (usuarioSeleccionado == null)
            {
                logger.Warn("Intento de dar de baja sin seleccionar usuario.");

                MessageBox.Show("Seleccione un usuario para dar de baja.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //Confirmar accion con el usuario
            var result = MessageBox.Show($"¿Seguro que deseas dar de baja al usuario {usuarioSeleccionado.Nombre}?",
                "Confirmar baja", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {

                    //Asignar la fecha actual como fecha de baja
                    usuarioSeleccionado.FechaBaja = DateTime.Today;

                    logger.Info($"Dando de baja usuario ID {usuarioSeleccionado.IdUsuario}.");


                    //Actualizamos
                    bool guardado = await _mvUsuarios.Update(usuarioSeleccionado);

                    if (guardado)
                    {
                        logger.Info($"Usuario ID {usuarioSeleccionado.IdUsuario} dado de baja correctamente.");

                        MessageBox.Show("Usuario dado de baja correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        //Refrescar lista o actualizar UI
                        await _mvUsuarios.ActualizarListaUsuarios();
                    }
                    else
                    {
                        logger.Warn($"Error al guardar baja para usuario ID {usuarioSeleccionado.IdUsuario}.");

                        MessageBox.Show("Error al guardar el usuario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"Error al dar de baja usuario ID {usuarioSeleccionado.IdUsuario}.");

                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //Nuevos metodos para paginacion
        private async void btnAnterior_Click(object sender, RoutedEventArgs e)
        {

            //Metodo heredado
            await _mvUsuarios.PaginaAnteriorAsync();
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            //Metodo heredado
            await _mvUsuarios.PaginaSiguienteAsync();
        }
    }
}

