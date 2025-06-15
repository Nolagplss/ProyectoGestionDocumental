using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Backend.Modelo;
using ProyectoDocumentales.MVVM.Base;
using ProyectoDocumentales.Servicios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProyectoDocumentales.MVVM
{
    public class MvDocumentos : MVBasePaginado<Documento>
    {
        //Contexto
        private Documentales2Context _contexto;


        private Documento _documentoOriginal;

        private DocumentoServicio documentoServicio;

        private CentroTrabajoServicio centroTrabajoServicio;
        private CentroEducativoServicio centroEducativoServicio;
        private ResponsableServicio responsableServicio;

        private bool _filtrosActivos = false;
        public bool FiltrosActivos
        {
            get => _filtrosActivos;
            set
            {
                _filtrosActivos = value;
                OnPropertyChanged(nameof(FiltrosActivos));
            }
        }

        //Guarda el nombre del Responsable
        private String _txtResponsable;


        private EmpresaServicio _empresaServicio;

        private Empresa _empresa;

        private DateTime? _fechaInicial;
        public DateTime? FechaInicial
        {
            get => _fechaInicial;
            set
            {
                _fechaInicial = value;
                OnPropertyChanged(nameof(FechaInicial));
            }
        }

        private DateTime? _fechaFinal;
        public DateTime? FechaFinal
        {
            get => _fechaFinal;
            set
            {
                _fechaFinal = value;
                OnPropertyChanged(nameof(FechaFinal));
            }
        }



        private Documento _documento;
        public Documento documento
        {
            get { return _documento; }
            set
            {
                _documento = value;
                OnPropertyChanged(nameof(documento));
            }
        }

        //Lista IEnumerable de documentos
        //public IEnumerable<Documento> ListaDocumentos { get { return Task.Run(documentoServicio.GetAllAsync).Result; } }
        //ListCollectionView para los documentos con filtros

        private ListCollectionView _listaDocumentosCV;

        public ListCollectionView ListaDocumentosCV
        {
            get => _listaDocumentosCV;
            private set
            {
                if (_listaDocumentosCV != value)
                {
                    _listaDocumentosCV = value;
                    OnPropertyChanged(nameof(ListaDocumentosCV));
                }
            }
        }



        private IEnumerable<CentrosTrabajo> _listaCentrosTrabajo;

        public IEnumerable<CentrosTrabajo> ListaCentrosTrabajo
        {
            get { return _listaCentrosTrabajo; }
            set
            {
                _listaCentrosTrabajo = value;
                OnPropertyChanged(nameof(ListaCentrosTrabajo));
            }
        }

        
        //Lista de empresas para el combobox
        private IEnumerable<Empresa> _listaEmpresasCache;

        public IEnumerable<Empresa> ListaEmpresas
        {
            get
            {
                if (_listaEmpresasCache == null)
                {
                    _listaEmpresasCache = Task.Run(_empresaServicio.GetAllAsync).Result;
                }
                return _listaEmpresasCache;
            }

        }
        public Empresa EmpresaSeleccionada
        {
            get => _empresa;
            set
            {
                _empresa = value;
                OnPropertyChanged(nameof(EmpresaSeleccionada));

            }
        }



        //Lista de sectores para el combobox
        private IEnumerable<string> _listaSectores;

        public IEnumerable<string> ListaSectores
        {
            get { return _listaSectores; }
            set
            {
                _listaSectores = value;
                OnPropertyChanged(nameof(ListaSectores));
            }

        }


        //Sector seleccionado
        private string _sectorSeleccionado;

        public string SectorSeleccionado
        {
            get => _sectorSeleccionado;
            set
            {
                _sectorSeleccionado = value;
                OnPropertyChanged(nameof(SectorSeleccionado));
            }
        }


        //Lista centrosEducativos
        private IEnumerable<CentrosEducativo> _listaCentrosEducativos;

        public IEnumerable<CentrosEducativo> ListaCentrosEducativos
        {
            get { return _listaCentrosEducativos; }
            set
            {
                _listaCentrosEducativos = value;
                OnPropertyChanged(nameof(ListaCentrosEducativos));
            }
        }

        //Centro educativo Seleccionado
        private CentrosEducativo _centroEducativoSeleccionado;

        public CentrosEducativo CentroEducativoSeleccionado
        {
            get => _centroEducativoSeleccionado;
            set
            {
                _centroEducativoSeleccionado = value;
                OnPropertyChanged(nameof(CentroEducativoSeleccionado));

                //Validacion manual
               // ValidateProperty(nameof(documento.IdCentroEducativo));
             

            }
        }



        //Lista responsable

        private IEnumerable<Responsable> _listaResponsables;

        public IEnumerable<Responsable> ListaResponsables
        {
            get { return _listaResponsables; }
            set
            {
                _listaResponsables = value;
                OnPropertyChanged(nameof(ListaResponsables));
            }
        }

        //Responsable Seleccionado
        private Responsable _responsableSeleccionado;
        public Responsable ResponsableSeleccionado
        {
            get => _responsableSeleccionado;
            set
            {
                _responsableSeleccionado = value;
                OnPropertyChanged(nameof(ResponsableSeleccionado));
            }
        }


        //Responsable
        public String txtResponsable
        {
            get => _txtResponsable;
            set
            {
                _txtResponsable = value;
                OnPropertyChanged(nameof(txtResponsable));
            }
        }

        //String para ruta de Cargar Archivo
        private string _archivoSeleccionado;
        public string ArchivoSeleccionado
        {
            get { return _archivoSeleccionado; }
            set
            {
                _archivoSeleccionado = value;
                OnPropertyChanged(nameof(ArchivoSeleccionado));
            }
        }




        //Constructor con el contexto
        public MvDocumentos(Documentales2Context contexto)
        {
            _contexto = contexto;
            documento = new Documento();

        }


        //Metodo inicializar
        public async Task Inicializa()
        {
            documentoServicio = new DocumentoServicio(_contexto);

            servicio = documentoServicio;

            centroTrabajoServicio = new CentroTrabajoServicio(_contexto);
            centroEducativoServicio = new CentroEducativoServicio(_contexto);
            responsableServicio = new ResponsableServicio(_contexto);

            ListaCentrosTrabajo = await centroTrabajoServicio.GetAllAsync();
            ListaCentrosEducativos = await centroEducativoServicio.GetAllAsync();
            ListaResponsables = await responsableServicio.GetAllAsync();

           


            _empresaServicio = new EmpresaServicio(_contexto);

            //Cargar sectores
            await CargarSectoresAsync();


            await CargarPaginaAsync();



        }

        //Copia de seguridad
        public void CrearCopiaSeguridad()
        {
            if (documento != null)
            {
                _documentoOriginal = new Documento
                {
                    IdDocumento = documento.IdDocumento,
                    NumeroConcierto = documento.NumeroConcierto,
                    FechaFirma = documento.FechaFirma,
                    Ruta = documento.Ruta,
                    IdCentroEducativo = documento.IdCentroEducativo,
                    IdEmpresa = documento.IdEmpresa,
                    IdUsuario = documento.IdUsuario,
                    
                    IdCentroEducativoNavigation = documento.IdCentroEducativoNavigation,
                    IdEmpresaNavigation = documento.IdEmpresaNavigation
                };
            }
        }
        //Restaurar documento original
        public void RestaurarDocumento()
        {
            if (_documentoOriginal != null)
            {
                documento.IdDocumento = _documentoOriginal.IdDocumento;
                documento.NumeroConcierto = _documentoOriginal.NumeroConcierto;
                documento.FechaFirma = _documentoOriginal.FechaFirma;
                documento.Ruta = _documentoOriginal.Ruta;
                documento.IdCentroEducativo = _documentoOriginal.IdCentroEducativo;
                documento.IdEmpresa = _documentoOriginal.IdEmpresa;
                documento.IdUsuario = _documentoOriginal.IdUsuario;
                documento.IdCentroEducativoNavigation = _documentoOriginal.IdCentroEducativoNavigation;
                documento.IdEmpresaNavigation = _documentoOriginal.IdEmpresaNavigation;

                //Restaurar las selecciones del formulario
                if (_documentoOriginal.IdCentroEducativoNavigation != null)
                {
                    CentroEducativoSeleccionado = _documentoOriginal.IdCentroEducativoNavigation;
                }

                if (_documentoOriginal.IdEmpresaNavigation != null)
                {
                    EmpresaSeleccionada = _documentoOriginal.IdEmpresaNavigation;
                }
            }
        }
        public override async Task CargarPaginaAsync()
        {
            var query = _contexto.Documentos
                .Include(d => d.IdEmpresaNavigation)
                    .ThenInclude(e => e.IdResponsableNavigation)
                .Include(d => d.IdCentroEducativoNavigation)
                .AsQueryable();

            //Aplicar filtros si están activos
            if (FiltrosActivos)
            {
                //Filtro por responsable
                if (!string.IsNullOrWhiteSpace(txtResponsable))
                {
                    var filtroResponsable = txtResponsable.ToLower();
                    query = query.Where(d => d.IdEmpresaNavigation != null &&
                                           d.IdEmpresaNavigation.IdResponsableNavigation != null &&
                                           d.IdEmpresaNavigation.IdResponsableNavigation.Nombre != null &&
                                           d.IdEmpresaNavigation.IdResponsableNavigation.Nombre.ToLower().StartsWith(filtroResponsable));
                }

                //Filtro por empresa
                if (EmpresaSeleccionada != null)
                {
                    query = query.Where(d => d.IdEmpresaNavigation != null &&
                                           d.IdEmpresaNavigation.IdEmpresa == EmpresaSeleccionada.IdEmpresa);
                }

                //Filtro por sector
                if (!string.IsNullOrWhiteSpace(SectorSeleccionado))
                {
                    query = query.Where(d => d.IdEmpresaNavigation != null &&
                                           d.IdEmpresaNavigation.Sector != null &&
                                           d.IdEmpresaNavigation.Sector.Equals(SectorSeleccionado));
                }

                //Filtro fechas
                if (FechaInicial.HasValue && FechaFinal.HasValue)
                {
                    var fechaIni = FechaInicial.Value.Date;
                    var fechaFin = FechaFinal.Value.Date.AddDays(1).AddTicks(-1); //Incluir todo el dia final

                    query = query.Where(d => d.FechaFirma >= fechaIni && d.FechaFirma <= fechaFin);
                    
                }
            }

            //Contar total de registros para la paginacion
            int totalRegistros = await query.CountAsync();
            TotalPaginas = (int)Math.Ceiling((double)totalRegistros / TamanioPagina);

            //Obtener los documentos de la pagina actual
            var documentosPagina = await query
                .OrderBy(d => d.IdDocumento)
                .Skip((PaginaActual - 1) * TamanioPagina)
                .Take(TamanioPagina)
                .ToListAsync();

            ListaDocumentosCV = new ListCollectionView(documentosPagina);
        }

        //Para refrescar la lista de empresas
        public async Task RefrescarListaEmpresas()
        {
            try
            {
                _listaEmpresasCache = await _empresaServicio.GetAllAsync();
                OnPropertyChanged(nameof(ListaEmpresas));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al refrescar empresas: {ex.Message}");
            }
        }

        public async Task RefrescarListaCentrosEducativos()
        {
            try
            {
                ListaCentrosEducativos = await centroEducativoServicio.GetAllAsync();
                OnPropertyChanged(nameof(ListaCentrosEducativos));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al refrescar la lista de centros educativos: {ex.Message}");
            }
        }
        private async Task CargarSectoresAsync()
        {
            var empresas = await _empresaServicio.GetAllAsync();
            // Obtenemos sectores únicos y eliminamos los nulos o vacíos
            _listaSectores = empresas
                .Where(e => !string.IsNullOrEmpty(e.Sector))
                .Select(e => e.Sector)
                .Distinct()
                .OrderBy(s => s)
                .ToList();

            OnPropertyChanged(nameof(ListaSectores));
        }

      


        //Cogemos el atributo privado para poder usar la lsita, es como un get
        public async void Filtrar()
        {
            FiltrosActivos = true;
            PaginaActual = 1;
            await CargarPaginaAsync();
        }

      
    
        public async Task ActualizarListaDocumentos()
        {
            try
            {
                await CargarPaginaAsync();


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la lista de documentos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public async Task<bool> Guarda()
        {
            try
            {
                await Add(documento);
                //Limpiar el documento despues de guardarlo
                documento = new Documento();
                await ActualizarListaDocumentos();
                return true;
            }
            catch (Exception ex)
            {
                // Manejo de errores...
                return false;
            }
        }

        private async Task<bool> ComprobadorCamposUnicos()
        {
            //Verificar si otro documento tiene el mismo NºConcierto
            var nConciertoExistente = await documentoServicio.FindAsync(r =>
                r.NumeroConcierto == documento.NumeroConcierto && r.IdDocumento != documento.IdDocumento);
            if (nConciertoExistente.Any())
            {
                MessageBox.Show("El NºConcierto ya está registrado. Introduce uno diferente.",
                              "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;

        }

        public async Task<bool> ActualizarDocumento()
        {
            try
            {
                if (documento != null)
                {
                    //Comprobamos campos unicos
                    if (!await ComprobadorCamposUnicos()) return false;

                    await documentoServicio.UpdateAsync(documento);

                    //Recargar la lista de documentos
                    await ActualizarListaDocumentos();

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores en el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al actualizar documento: {ex.Message}");
                });
            }

            return false;
        }

    }
}
