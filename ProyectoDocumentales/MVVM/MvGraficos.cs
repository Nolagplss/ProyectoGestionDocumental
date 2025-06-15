using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Frontend.Modelo;
using ProyectoDocumentales.MVVM.Base;
using ProyectoDocumentales.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDocumentales.MVVM
{
    public class MvGraficos : MVBase
    {


        private Documentales2Context _contexto;
        private DocumentoServicio _documentoServicio;
        private EmpresaServicio _empresaServicio;

        //Datos para graficos
        private List<DatoGrafico> _datosEmpresasPorSector;
        private List<DatoGraficoAnio> _datosDocumentosPorAnio;

        //Estadisticas generales
        private int _totalDocumentos;
        private int _totalEmpresas;
        private int _totalSectores;
        private int _documentosAnioActual;

        public List<DatoGrafico> DatosEmpresasPorSector
        {
            get => _datosEmpresasPorSector;
            set
            {
                _datosEmpresasPorSector = value;
                OnPropertyChanged(nameof(DatosEmpresasPorSector));
            }
        }

        public List<DatoGraficoAnio> DatosDocumentosPorAnio
        {
            get => _datosDocumentosPorAnio;
            set
            {
                _datosDocumentosPorAnio = value;
                OnPropertyChanged(nameof(DatosDocumentosPorAnio));
            }
        }

        public int TotalDocumentos
        {
            get => _totalDocumentos;
            set
            {
                _totalDocumentos = value;
                OnPropertyChanged(nameof(TotalDocumentos));
            }
        }

        public int TotalEmpresas
        {
            get => _totalEmpresas;
            set
            {
                _totalEmpresas = value;
                OnPropertyChanged(nameof(TotalEmpresas));
            }
        }

        public int TotalSectores
        {
            get => _totalSectores;
            set
            {
                _totalSectores = value;
                OnPropertyChanged(nameof(TotalSectores));
            }
        }

        public int DocumentosAnioActual
        {
            get => _documentosAnioActual;
            set
            {
                _documentosAnioActual = value;
                OnPropertyChanged(nameof(DocumentosAnioActual));
            }
        }

        public MvGraficos(Documentales2Context contexto)
        {
            _contexto = contexto;
        }

        public async Task Inicializa()
        {
            _documentoServicio = new DocumentoServicio(_contexto);
            _empresaServicio = new EmpresaServicio(_contexto);

            await CargarDatosGraficos();
            await CargarEstadisticasGenerales();
        }

        private async Task CargarDatosGraficos()
        {
            try
            {
                //Datos para grafico de empresas por sector
                await CargarDatosEmpresasPorSector();

                // Datos para grafico de documentos por año
                await CargarDatosDocumentosPorAño();
            }
            catch (Exception ex)
            {
                //error
                throw new Exception($"Error al cargar datos de gráficos: {ex.Message}");
            }
        }

        private async Task CargarDatosEmpresasPorSector()
        {
            var empresas = await _empresaServicio.GetAllAsync();

            var gruposPorSector = empresas
                .Where(e => !string.IsNullOrEmpty(e.Sector))
                .GroupBy(e => e.Sector)
                .Select(g => new DatoGrafico
                {
                    Sector = g.Key,
                    Cantidad = g.Count()
                })
                .OrderByDescending(d => d.Cantidad)
                .ToList();

            DatosEmpresasPorSector = gruposPorSector;
        }

        private async Task CargarDatosDocumentosPorAño()
        {
            var documentos = await _documentoServicio.GetAllAsync();

            var gruposPorAnio = documentos
                .Where(d => d.FechaFirma.HasValue)
                .GroupBy(d => d.FechaFirma.Value.Year)
                .Select(g => new DatoGraficoAnio
                {
                    Anio = g.Key,
                    Cantidad = g.Count()
                })
                .OrderBy(d => d.Anio)
                .ToList();

            DatosDocumentosPorAnio = gruposPorAnio;
        }

        private async Task CargarEstadisticasGenerales()
        {
            try
            {
                var documentos = await _documentoServicio.GetAllAsync();
                var empresas = await _empresaServicio.GetAllAsync();

                TotalDocumentos = documentos.Count();
                TotalEmpresas = empresas.Count();
                TotalSectores = empresas
                    .Where(e => !string.IsNullOrEmpty(e.Sector))
                    .Select(e => e.Sector)
                    .Distinct()
                    .Count();

                DocumentosAnioActual = documentos
                    .Where(d => d.FechaFirma.HasValue && d.FechaFirma.Value.Year == DateTime.Now.Year)
                    .Count();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar estadísticas generales: {ex.Message}");
            }
        }

        public async Task ActualizarDatos()
        {
            await CargarDatosGraficos();
            await CargarEstadisticasGenerales();
        }
    }

  
}

