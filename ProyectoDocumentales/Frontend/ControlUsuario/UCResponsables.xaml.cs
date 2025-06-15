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
    /// Lógica de interacción para UCResponsables.xaml
    /// </summary>
    public partial class UCResponsables : UserControl
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MvResponsables _mvResponsables;

        public UCResponsables(MvResponsables mvResponsables)
        {
            InitializeComponent();
            _mvResponsables = mvResponsables;
            this.DataContext = _mvResponsables;
            logger.Info("UCResponsables inicializado.");

        }

        private void btnAplicarFiltros_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Aplicando filtros en responsables.");

            _mvResponsables.Filtrar();
        }

        private async void btnBorrarFiltros_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Borrando filtros en responsables.");

            _mvResponsables.txtDni = string.Empty;
            _mvResponsables.FiltrosActivos = false;
            _mvResponsables.PaginaActual = 1;
            await _mvResponsables.CargarPaginaAsync();
        }

        private void btnNuevoResponsable_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Abriendo formulario para añadir nuevo responsable.");

            ResponsableForm responsableForm = new ResponsableForm(_mvResponsables, "Añadir nuevo responsable");
            responsableForm.ShowDialog();

        }

        private async void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            
            //Asigna el responsable seleccionado en el DataGrid al ViewModel
            _mvResponsables.responsable = (Responsable)dgResponsables.SelectedItem;

            //Crea y muestra el dialogo de edicion de responsable
            ResponsableForm responsableForm = new ResponsableForm(_mvResponsables, "Editar Responsable");
            responsableForm.ShowDialog();

            //Si el responsable confirma la edicion, refresca la tabla
            if (responsableForm.DialogResult.Equals(true))
            {
                logger.Info("Responsable editado, actualizando lista.");

                //Actualizar la lista de responsables
                await _mvResponsables.ActualizarListaResponsables();
            }
            
        }

        private async void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Cogemos el responsable desde el tag del boton
                Button button = sender as Button;
                Responsable responsableSeleccionado = button?.Tag as Responsable;

                if (responsableSeleccionado == null)
                {
                    logger.Warn("No se pudo obtener el responsable seleccionado para eliminar.");

                    MessageBox.Show("No se pudo obtener el responsable seleccionado.", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //Confirmar la eliminacion con el usuario
                var result = MessageBox.Show(
                    $"¿Está seguro que desea eliminar al responsable {responsableSeleccionado.Nombre} {responsableSeleccionado.Apellidos}?\n\n" +
                    "Esta acción no se puede deshacer.",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    //Eliminar el responsable usando el servicio
                    bool eliminado = await _mvResponsables.Delete(responsableSeleccionado);

                    if (eliminado)
                    {
                        logger.Info($"Responsable ID {responsableSeleccionado.IdResponsable} eliminado correctamente.");

                        MessageBox.Show("Responsable eliminado correctamente.", "Éxito",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        //Actualizar la lista de responsables
                        await _mvResponsables.ActualizarListaResponsables();
                    }
                    else
                    {
                        logger.Warn($"Error al eliminar responsable ID {responsableSeleccionado.IdResponsable}.");

                        MessageBox.Show("Error al eliminar el responsable.", "Error",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error inesperado al eliminar responsable.");

                MessageBox.Show($"Error inesperado: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnAnterior_Click(object sender, RoutedEventArgs e)
        {
            if (_mvResponsables.PaginaActual > 1)
            {
                _mvResponsables.PaginaActual--;
                logger.Info($"Página anterior solicitada. Página actual: {_mvResponsables.PaginaActual}");

                await _mvResponsables.CargarPaginaAsync();
            }
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            if (_mvResponsables.PaginaActual < _mvResponsables.TotalPaginas)
            {
                _mvResponsables.PaginaActual++;
                logger.Info($"Página siguiente solicitada. Página actual: {_mvResponsables.PaginaActual}");
                await _mvResponsables.CargarPaginaAsync();
            }
        }

    }
}
