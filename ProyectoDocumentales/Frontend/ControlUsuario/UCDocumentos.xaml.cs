using NLog;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Backend.Modelo;
using ProyectoDocumentales.Frontend.Formularios;
using ProyectoDocumentales.MVVM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
   
    public partial class UCDocumentos : UserControl
    {

        private MvDocumentos _mvDocumentos;

        private Usuario _usuario;

        private Documentales2Context _contexto;

        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public UCDocumentos()
        {
            InitializeComponent();
        }

        public UCDocumentos(MvDocumentos mvDocumentos, Usuario usuario, Documentales2Context contexto)
        {
            InitializeComponent();
            _mvDocumentos = mvDocumentos;
            DataContext = _mvDocumentos;

            _usuario = usuario;
            _contexto = contexto;

            logger.Info($"UCDocumentos inicializado para usuario ID {usuario?.IdUsuario}.");


            if (!RoleManager.HasPermission("DOC_CREATE")){
                btnCrearDocumento.Visibility = Visibility.Collapsed;
                logger.Info("Botón de crear documento ocultado por falta de permisos.");

            }

        }



        private void btnAplicar_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Aplicando filtros en documentos.");

            _mvDocumentos.Filtrar();
        }

        private async void btnBorrarFiltros_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Borrando filtros de documentos.");

            _mvDocumentos.txtResponsable = string.Empty;
            _mvDocumentos.EmpresaSeleccionada = null;
            _mvDocumentos.SectorSeleccionado = null;
            _mvDocumentos.FechaInicial = null;
            _mvDocumentos.FechaFinal = null;
            datePickerInicial.SelectedDate = null;
            datePickerFinal.SelectedDate = null;
            _mvDocumentos.FiltrosActivos = false;
            _mvDocumentos.PaginaActual = 1;
            await _mvDocumentos.CargarPaginaAsync();
        }

        private void ComboEmpresas_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            combo.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void ComboEmpresas_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            combo.Foreground = new SolidColorBrush(Colors.White);
        }

        private void ComboSectores_DropDownOpened(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            combo.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void ComboSectores_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            combo.Foreground = new SolidColorBrush(Colors.White);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnVerDocumento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                string rutaPdf = btn?.Tag?.ToString();

                if (string.IsNullOrEmpty(rutaPdf))
                {
                    logger.Warn("No se encontró la ruta del documento para abrir.");

                    MessageBox.Show("No se ha encontrado la ruta del documento.", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //Verificar si el archivo existe
                if (!File.Exists(rutaPdf))
                {
                    logger.Warn($"Archivo no existe: {rutaPdf}");

                    MessageBox.Show($"El archivo no existe en la ruta especificada:\n{rutaPdf}",
                                  "Archivo no encontrado", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = rutaPdf,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
                logger.Info("Documento abierto correctamente.");



            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al abrir el documento.");

                MessageBox.Show($"Error al abrir el documento: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void datePickerInicial_CalendarOpened(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            datePicker.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void datePickerInicial_CalendarClosed(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            datePicker.Foreground = new SolidColorBrush(Colors.White);
        }

       

        private void datePickerFinal_CalendarOpened(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            datePicker.Foreground = new SolidColorBrush(Colors.Black);

        }

        private void datePickerFinal_CalendarClosed(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            datePicker.Foreground = new SolidColorBrush(Colors.White);
        }

        private async void btnEditar_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                //Asigna el documento seleccionado desde el boton
                Button button = sender as Button;
                Documento documentoSeleccionado = button?.Tag as Documento;

                if (documentoSeleccionado == null)
                {
                    MessageBox.Show("No se pudo obtener el documento seleccionado.", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //Verifica los permisos
                bool puedeEditar =
                    RoleManager.HasPermission("DOC_EDIT_ALL") ||
                    (RoleManager.HasPermission("DOC_EDIT_OWN") &&
                     documentoSeleccionado.IdUsuario == _usuario.IdUsuario);

                if (!puedeEditar)
                {
                    logger.Warn($"Acceso denegado para editar documento ID {documentoSeleccionado.IdDocumento} por usuario ID {_usuario?.IdUsuario}.");

                    MessageBox.Show("No tiene permiso para editar este documento.", "Acceso denegado",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                //Asigna el documento al ViewModel
                _mvDocumentos.documento = documentoSeleccionado;

                //Crea y muestra el formulario de edicion
                NewDocumentForm newDocumentForm = new NewDocumentForm(_mvDocumentos, _usuario, _contexto, "Editar documento");
                newDocumentForm.ShowDialog();

                //Si se confirmó la edicion, actualiza la lista
                if (newDocumentForm.DialogResult.Equals(true))
                {
                    logger.Info($"Documento ID {documentoSeleccionado.IdDocumento} editado correctamente.");

                    await _mvDocumentos.ActualizarListaDocumentos();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error inesperado en edición de documento.");

                MessageBox.Show($"Error inesperado: {ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }


      

        private async void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Cogemos el documento desde el tag del boton
                Button button = sender as Button;
                Documento documentoSeleccionado = button?.Tag as Documento;

                bool puedeEliminar =
                 RoleManager.HasPermission("DOC_DELETE_ALL") ||
                 (RoleManager.HasPermission("DOC_DELETE_OWN") &&
                  documentoSeleccionado.IdUsuario == _usuario.IdUsuario);

                if (!puedeEliminar)
                {
                    logger.Warn($"Acceso denegado para eliminar documento ID {documentoSeleccionado.IdDocumento}");

                    MessageBox.Show("No tiene permiso para eliminar este documento.", "Acceso denegado",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

               

                if (documentoSeleccionado == null)
                {
                    logger.Warn("Intento de eliminar sin seleccionar documento.");

                    MessageBox.Show("No se pudo obtener el documento seleccionado.", "Error",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                //Confirmar la eliminacion con el usuario
                var result = MessageBox.Show(
                    $"¿Está seguro que desea eliminar al documento {documentoSeleccionado.NumeroConcierto}?\n\n" +
                    "Esta acción no se puede deshacer.",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    //Eliminar el responsable usando el servicio
                    bool eliminado = await _mvDocumentos.Delete(documentoSeleccionado);

                    if (eliminado)
                    {
                        logger.Info($"Documento ID {documentoSeleccionado.IdDocumento} eliminado correctamente.");

                        MessageBox.Show("Documento eliminado correctamente.", "Éxito",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        //Actualizar la lista de documentos
                        await _mvDocumentos.ActualizarListaDocumentos();
                    }
                    else
                    {
                        logger.Warn($"Error al eliminar documento ID {documentoSeleccionado.IdDocumento}.");

                        MessageBox.Show("Error al eliminar el documento.", "Error",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }



            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error inesperado en eliminación de documento.");

                MessageBox.Show($"Error inesperado: {ex.Message}", "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCrearDocumento_Click(object sender, RoutedEventArgs e)
        {
            NewDocumentForm newDocumentForm = new NewDocumentForm(_mvDocumentos, _usuario, _contexto, "Crear documento");
            newDocumentForm.ShowDialog();

        }

        private async void btnAnterior_Click(object sender, RoutedEventArgs e)
        {
            if (_mvDocumentos.PaginaActual > 1)
            {
                _mvDocumentos.PaginaActual--;
                await _mvDocumentos.CargarPaginaAsync();
            }
        }

        private async void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            if (_mvDocumentos.PaginaActual < _mvDocumentos.TotalPaginas)
            {
                _mvDocumentos.PaginaActual++;
                await _mvDocumentos.CargarPaginaAsync();
            }
        }
    }
}
