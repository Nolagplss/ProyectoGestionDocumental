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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoDocumentales.Frontend.Formularios
{
    /// <summary>
    /// Lógica de interacción para RolesForm.xaml
    /// </summary>
    public partial class RolesForm : UserControl
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MvRolesPermisos _mvRolesPermisos;
        private bool _esNuevoRol = false;

        public RolesForm(MvRolesPermisos mvRolesPermisos)
        {
            InitializeComponent();
            _mvRolesPermisos = mvRolesPermisos;
            DataContext = _mvRolesPermisos;
            logger.Info("RolesForm iniciado.");

        }

        private void btnNuevoRol_Click(object sender, RoutedEventArgs e)
        {
            //Crear nuevo rol
            _mvRolesPermisos.RolSeleccionado = new Role();
            _esNuevoRol = true;
            txtNombreRol.Focus();
            MessageBox.Show("Nuevo rol creado. Complete los detalles y guarde.");

            logger.Info("Nuevo rol creado y formulario preparado para entrada.");

        }


        private async void btnEliminarRol_Click(object sender, RoutedEventArgs e)
        {
            if (_mvRolesPermisos.RolSeleccionado != null && _mvRolesPermisos.RolSeleccionado.IdRol != 0)
            {
                var result = MessageBox.Show(
                    $"¿Está seguro de eliminar el rol '{_mvRolesPermisos.RolSeleccionado.NombreRol}'?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    logger.Info("Intentando eliminar rol: {0}", _mvRolesPermisos.RolSeleccionado.NombreRol);

                    var eliminado = await _mvRolesPermisos.Delete(_mvRolesPermisos.RolSeleccionado);
                    if (eliminado)
                    {

                        MessageBox.Show("Rol eliminado correctamente.");
                        _mvRolesPermisos.RolSeleccionado = null;
                        logger.Info("Rol eliminado exitosamente.");

                    }
                }
            }
            else
            {
                logger.Warn("Intento de eliminar rol sin seleccionar uno válido.");
                MessageBox.Show("Seleccione un rol existente para eliminar.");
            }
        }

        private async void btnAgregarPermiso_Click(object sender, RoutedEventArgs e)
        {
            if (_mvRolesPermisos.RolSeleccionado == null)
            {

                MessageBox.Show("Seleccione un rol primero.");
                logger.Warn("Intento de agregar permiso sin rol seleccionado.");

                return;
            }

            if (cmbPermisos.SelectedItem is Permiso permisoSeleccionado)
            {
                //Verificar si el permiso ya esta asignado
                if (_mvRolesPermisos.RolSeleccionado.IdPermisos?.Any(p => p.IdPermiso == permisoSeleccionado.IdPermiso) == true)
                {
                    MessageBox.Show("Este permiso ya esta asignado al rol.");
                    logger.Warn("Intento de agregar permiso duplicado: {0}", permisoSeleccionado.Codigo);

                    return;
                }

                //Si es un rol nuevo, agregamos directamente a la coleccion
                if (_mvRolesPermisos.RolSeleccionado.IdRol == 0)
                {
                    if (_mvRolesPermisos.RolSeleccionado.IdPermisos == null)
                    {
                        _mvRolesPermisos.RolSeleccionado.IdPermisos = new List<Permiso>();
                    }
                    _mvRolesPermisos.RolSeleccionado.IdPermisos.Add(permisoSeleccionado);
                    MessageBox.Show($"Permiso '{permisoSeleccionado.Codigo}' agregado al rol.");
                    logger.Info("Permiso '{0}' agregado a rol nuevo.", permisoSeleccionado.Codigo);
    
                }
                else
                {
                    //Si es un rol existente, usamos el metodo del ViewModel
                    var agregado = await _mvRolesPermisos.AgregarPermisoARole(
                        _mvRolesPermisos.RolSeleccionado.IdRol,
                        permisoSeleccionado.IdPermiso);

                    if (agregado)
                    {
                        MessageBox.Show($"Permiso '{permisoSeleccionado.Codigo}' agregado al rol.");
                        logger.Info("Permiso '{0}' agregado a rol existente.", permisoSeleccionado.Codigo);

                    }
                }

                //Actualizar la vista
                lvPermisosAsignados.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Seleccione un permiso de la lista.");
                logger.Warn("Intento de agregar permiso sin selección.");

            }
        }

        private async void btnQuitarPermiso_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int idPermiso)
            {
                if (_mvRolesPermisos.RolSeleccionado == null)
                {
                    MessageBox.Show("No hay rol seleccionado.");
                    logger.Warn("Intento de quitar permiso sin rol seleccionado.");

                    return;
                }

                var permiso = _mvRolesPermisos.RolSeleccionado.IdPermisos?.FirstOrDefault(p => p.IdPermiso == idPermiso);
                if (permiso != null)
                {
                    //Si es un rol nuevo removemos directamente de la coleccion
                    if (_mvRolesPermisos.RolSeleccionado.IdRol == 0)
                    {
                        _mvRolesPermisos.RolSeleccionado.IdPermisos.Remove(permiso);
                        MessageBox.Show($"Permiso '{permiso.Codigo}' removido del rol.");
                        logger.Info("Permiso '{0}' removido de rol nuevo.", permiso.Codigo);

                    }
                    else
                    {
                        //Si es un rol existente, usamos el metodo del ViewModel
                        var removido = await _mvRolesPermisos.QuitarPermisoDeRole(
                            _mvRolesPermisos.RolSeleccionado.IdRol,
                            idPermiso);

                        if (removido)
                        {
                            MessageBox.Show($"Permiso '{permiso.Codigo}' removido del rol.");
                            logger.Info("Permiso '{0}' removido de rol existente.", permiso.Codigo);

                        }
                    }

                    //Actualizar la vista
                    lvPermisosAsignados.Items.Refresh();
                }
            }
        }

        private async void btnGuardarRol_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_mvRolesPermisos.RolSeleccionado == null)
                {
                    MessageBox.Show("No hay rol para guardar.");
                    logger.Warn("Intento de guardar sin rol seleccionado.");

                    return;
                }

                if (string.IsNullOrWhiteSpace(_mvRolesPermisos.RolSeleccionado.NombreRol))
                {
                    MessageBox.Show("El nombre del rol es obligatorio.");
                    logger.Warn("Intento de guardar rol sin nombre.");

                    return;
                }

                if (!_esNuevoRol && _mvRolesPermisos.RolSeleccionado.IdRol != 0)
                {
                    MessageBox.Show("Este botón es solo para guardar roles nuevos. El rol seleccionado ya existe.");
                    logger.Warn("Intento de guardar rol existente con botón de nuevo rol.");

                    return;
                }

                bool guardado = false;

                //Nuevo rol
                if (_esNuevoRol || _mvRolesPermisos.RolSeleccionado.IdRol == 0)
                {
                    guardado = await _mvRolesPermisos.Add(_mvRolesPermisos.RolSeleccionado);
                    logger.Info("Guardando nuevo rol");

                }
                else
                {
                    //Rol existente
                    guardado = await _mvRolesPermisos.Update(_mvRolesPermisos.RolSeleccionado);
                    logger.Info("Actualizando rol existente");

                }

                if (guardado)
                {
                    MessageBox.Show("Rol guardado correctamente.");
                    logger.Info("Rol guardado correctamente.");

                    _esNuevoRol = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Excepción al guardar rol.");
                MessageBox.Show($"Error al guardar el rol: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            //Restablecer seleccion
            _mvRolesPermisos.RolSeleccionado = _mvRolesPermisos.Lista?.FirstOrDefault();
            _esNuevoRol = false;
            MessageBox.Show("Cambios cancelados.");
            logger.Info("Cambios cancelados y selección restablecida.");

        }
    }
}
