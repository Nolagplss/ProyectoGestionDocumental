using ProyectoDocumentales.Backend.Modelo.Utiles;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.MVVM;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using QuestPDF.Fluent;
using ProyectoDocumentales.Backend.Modelo;
using NLog;

namespace ProyectoDocumentales.Frontend.Formularios
{
    /// <summary>
    /// Lógica de interacción para NewDocumentForm.xaml
    /// </summary>
    public partial class NewDocumentForm : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MvDocumentos _mvDocumentos;

        private Usuario _usuario;

        private MvEmpresa _mvEmpresa;

        private String _tituloDocumento;


        private Documentales2Context _contexto;

        public NewDocumentForm(MvDocumentos mvDocumentos, Usuario usuario, Documentales2Context contexto, string tituloDocumento)
        {
            InitializeComponent();
            logger.Info("Inicializando NewDocumentForm");

            _mvDocumentos = mvDocumentos;
            _usuario = usuario;
            _contexto = contexto;

            

            //Asignamos el titulo
            _tituloDocumento = tituloDocumento;
            txtTituloDocumento.Text = _tituloDocumento;

            if(_tituloDocumento.Contains("Crear documento"))
            {
              
                LimpiarFormulario();
            }
            if (_tituloDocumento.Contains("Editar documento"))
            {
                _mvDocumentos.CrearCopiaSeguridad();
            }

            DataContext = _mvDocumentos;
            _mvDocumentos.btnGuardar = btnGuardar;
            _mvDocumentos.btnGenerarDocumento = btnGenerarDocumento;

            _mvDocumentos.RegisterValidationSection("documento", btnGuardar,
               "txtNumeroConcierto", "cmbCentroEducativo", "cmbEmpresa");

            _mvDocumentos.RegisterValidationSection("generarDocumento", btnGenerarDocumento,
              "txtNumeroConcierto", "cmbCentroEducativo", "cmbEmpresa");




            this.AddHandler(Validation.ErrorEvent, new RoutedEventHandler(_mvDocumentos.OnErrorEvent));

            this.Loaded += async (s, e) => await CargarPermiso();
        }

        private async Task CargarPermiso()
        {
            logger.Info("Comprobando permisos para DOC_ASSIGN_CENTER y CREATE_JOB_CENTER");

            if (!RoleManager.HasPermission("DOC_ASSIGN_CENTER"))
            {
                switch (_tituloDocumento)
                {
                    case "Crear documento":


                        cmbCentroEducativo.IsEnabled = false;

                        //Obtener el centro educativo del usuario
                        if (_usuario?.IdCentroEducativo != null)
                        {
                            //Buscar el centro en la lista ya cargada por el ViewModel
                            var centroUsuario = _mvDocumentos.ListaCentrosEducativos
                                .FirstOrDefault(c => c.IdCentroEducativo == _usuario.IdCentroEducativo);

                            if (centroUsuario != null)
                            {
                                //Establecer como seleccionado
                                _mvDocumentos.CentroEducativoSeleccionado = centroUsuario;
                                cmbCentroEducativo.SelectedItem = centroUsuario;
                                logger.Info($"Centro educativo seleccionado por usuario: {centroUsuario.IdCentroEducativo}");

                            }
                        }
                        break;
                    case "Editar documento":
                        //Si es editar documento, no hacemos nada, ya que el centro educativo ya estar seleccionado
                        cmbCentroEducativo.IsEnabled = false;
                        logger.Info("Centro educativo deshabilitado en modo edición de documento");
                        break;

                }


            }
            //Si no tiene el permiso ocultar boton y mostrar texto
            if (!RoleManager.HasPermission("CREATE_JOB_CENTER"))
            {
                btnCrearEmpresa.Visibility = Visibility.Collapsed;
                txtContactaAdmin.Visibility = Visibility.Visible;
                logger.Info("Usuario no tiene permiso CREATE_JOB_CENTER, ocultando botón crear empresa");

            }

        }

        private void LimpiarFormulario()
        {
            txtNumeroConcierto.Text = string.Empty;
            dpFechaFirma.SelectedDate = null;
            cmbEmpresa.SelectedIndex = -1;

            txtRuta.Text = string.Empty;

            //Limpiar los detalles de la empresa
            txtEmpresaSector.Text = string.Empty;
            txtEmpresaCIF.Text = string.Empty;
            txtEmpresaResponsable.Text = string.Empty;

            //Datos del centro
            txtCentroDireccion.Text = string.Empty;
            txtCentroCIF.Text = string.Empty;
            txtCentroDirector.Text = string.Empty;
            cmbCentroEducativo.SelectedIndex = -1;

            _mvDocumentos.CentroEducativoSeleccionado = null;
            _mvDocumentos.EmpresaSeleccionada = null;
            _mvDocumentos.documento = new Documento();


        }

        private async void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

               

                //Verificar si el documento ya está inciializado en el ViewModel
                if (_mvDocumentos.documento == null)
                {
                    _mvDocumentos.documento = new Documento();
                    logger.Info("Nuevo objeto Documento creado");

                }

                //Asignar valores del formulario al objeto documento
                _mvDocumentos.documento.NumeroConcierto = txtNumeroConcierto.Text;
                _mvDocumentos.documento.FechaFirma = dpFechaFirma.SelectedDate;
                _mvDocumentos.documento.Ruta = txtRuta.Text;

                //Asignar el centro educativo seleccionado
                if (_mvDocumentos.CentroEducativoSeleccionado != null)
                {
                    _mvDocumentos.documento.IdCentroEducativo = _mvDocumentos.CentroEducativoSeleccionado.IdCentroEducativo;
                    _mvDocumentos.documento.IdCentroEducativoNavigation = _mvDocumentos.CentroEducativoSeleccionado;
                }

                //Asignar la empresa seleccionada
                if (_mvDocumentos.EmpresaSeleccionada != null)
                {
                    _mvDocumentos.documento.IdEmpresa = _mvDocumentos.EmpresaSeleccionada.IdEmpresa;
                    _mvDocumentos.documento.IdEmpresaNavigation = _mvDocumentos.EmpresaSeleccionada;
                }

               

                switch (_tituloDocumento)
                {
                    case "Crear documento":

                        //Cogemos el id del usuario que creo el documento
                        _mvDocumentos.documento.IdUsuario = _usuario.IdUsuario;

                        if (await _mvDocumentos.Guarda())
                        {
                            logger.Info("Documento guardado correctamente");

                            MessageBox.Show("Documento guardado correctamente", "Actualización del documento", MessageBoxButton.OK, MessageBoxImage.Information);
                            LimpiarFormulario();
                        }

                        break;
                    case "Editar documento":

                        if (await _mvDocumentos.ActualizarDocumento())
                        {
                            logger.Info("Documento actualizado correctamente");

                            MessageBox.Show("Documento actualizado correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.Close();
                        }
                        else
                        {
                            logger.Warn("Error al actualizar el documento");

                            MessageBox.Show("Error al actualizar el documento", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;

                       
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el documento: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }


        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {

            switch (_tituloDocumento)
            {
                case "Crear documento":
                    //Si es crear documento, limpiamos el formulario
                    LimpiarFormulario();
                    _mvDocumentos.EmpresaSeleccionada = null;
                    this.Close();
                    break;
                case "Editar documento":
                    _mvDocumentos.RestaurarDocumento();
                    _mvDocumentos.EmpresaSeleccionada = null;
                    this.Close();
                    
                    break;
            }

          
        }

        private void btnExaminarDocumento_Click(object sender, RoutedEventArgs e)
        {
            LoadFileRute.LoadFile(txtRuta, _mvDocumentos);
        }



        private void btnCrearEmpresa_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("btnCrearEmpresa_Click iniciado");

            //Inicializamos el viewModel de la Empresa con el contexto
            var mvEmpresa = new MvEmpresa(_contexto);

            //Callback para actualizar las empresas
            Action onEmpresaGuardada = async () =>
            {
                try
                {
                    //Actualizar la lista de empresas en el ViewModel
                    await _mvDocumentos.RefrescarListaEmpresas();

                    logger.Info("Lista de empresas actualizada tras creación");

                    MessageBox.Show("Lista de empresas actualizada", "Información",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error al actualizar la lista de empresas");

                    MessageBox.Show($"Error al actualizar lista: {ex.Message}", "Error",
                       MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };


            //LE pasamoe el mv
            EmpresaForm companyForm = new EmpresaForm(mvEmpresa, onEmpresaGuardada);
            companyForm.ShowDialog();

        }

        private void cmbEmpresa_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //Siempre por defecto borramos los anteriores
                txtEmpresaSector.Text = string.Empty;
                txtEmpresaCIF.Text = string.Empty;
                txtEmpresaResponsable.Text = string.Empty;

                //objeto empresa
                var empresa = cmbEmpresa.SelectedItem as Empresa;

                if (empresa != null)
                {
                    //Actualizar los TextBlocks con la informacion de la empresa
                    txtEmpresaSector.Text = empresa.Sector;
                    txtEmpresaCIF.Text = empresa.Cif;

                    //Si la empresa tiene un responsable asociado, mostrar su nombre
                    if (empresa.IdResponsableNavigation != null)
                    {
                        txtEmpresaResponsable.Text = empresa.IdResponsableNavigation.Nombre ?? "No disponible";


                    }
                    else
                    {
                        txtEmpresaResponsable.Text = "No asignado";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void cmbCentroEducativo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCentroDireccion.Text = string.Empty;
                txtCentroCIF.Text = string.Empty;
                txtCentroDirector.Text = string.Empty;

                //objeto empresa
                var centroEducativo = cmbCentroEducativo.SelectedItem as CentrosEducativo;

                if (centroEducativo != null)
                {
                    //Actualizar los TextBlocks con la informacion del centro
                    txtCentroDireccion.Text = centroEducativo.Direccion;
                    txtCentroCIF.Text = centroEducativo.Cif;
                    txtCentroDirector.Text = centroEducativo.Director;


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGenerarDocumento_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.Info("btnGenerarDocumento_Click iniciado");

                QuestPDF.Settings.License = LicenseType.Community;

                string rutaBase = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rutas");
                Directory.CreateDirectory(rutaBase);

                if (string.IsNullOrEmpty(txtNumeroConcierto.Text))
                {
                    logger.Warn("Número de concierto vacío al generar PDF");

                    MessageBox.Show("Por favor, introduce un número de concierto.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string nombrePdf = $"Documento_{txtNumeroConcierto.Text}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                string rutaPdf = System.IO.Path.Combine(rutaBase, nombrePdf);



                var documento = new DocumentoPDF(
                    numero: txtNumeroConcierto.Text,
                    fecha: dpFechaFirma.SelectedDate?.ToString("dd/MM/yyyy") ?? "No indicada",
                    centro: cmbCentroEducativo.Text,
                    direccionCentro: txtCentroDireccion.Text,
                    cifCentro: txtCentroCIF.Text,
                    directorCentro: txtCentroDirector.Text,
                    empresa: cmbEmpresa.Text,
                    sectorEmpresa: txtEmpresaSector.Text,
                    cifEmpresa: txtEmpresaCIF.Text,
                    responsableEmpresa: txtEmpresaResponsable.Text
                );

                documento.GeneratePdf(rutaPdf);

                logger.Info($"Documento PDF generado en: {rutaPdf}");

                MessageBox.Show($"Documento generado correctamente en:\n{rutaPdf}", "PDF generado", MessageBoxButton.OK, MessageBoxImage.Information);
                string rutaRelativa = System.IO.Path.GetRelativePath(AppDomain.CurrentDomain.BaseDirectory, rutaPdf);
                txtRuta.Text = rutaRelativa;

            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al generar el documento PDF");

                MessageBox.Show($"Error al generar el documento PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove(); //Permite mover la ventana al arrastrar
            }
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            switch (_tituloDocumento)
            {
                case "Crear documento":
                    LimpiarFormulario();
                    _mvDocumentos.EmpresaSeleccionada = null;
                    this.Close();
                    break;

                case "Editar documento":
                    _mvDocumentos.RestaurarDocumento();
                    _mvDocumentos.EmpresaSeleccionada = null;
                    this.Close();
                    break;
            }
           
        }
    }
}
