using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Backend.Modelo.Utiles;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDocumentales.Servicios
{
    public class InformeServicio
    {
        private readonly Documentales2Context _contexto;

        public InformeServicio(Documentales2Context contexto)
        {
            _contexto = contexto;
        }

        /// <summary>
        /// Genera el informe de documentos agrupados por sector
        /// </summary>
        public async Task<string> GenerarInformeDocumentosPorSector()
        {
            try
            {
                // Obtener documentos con sus relaciones
                var documentos = await _contexto.Documentos
                    .Include(d => d.IdEmpresaNavigation)
                    .ThenInclude(e => e.IdResponsableNavigation)
                    .Where(d => d.IdEmpresaNavigation != null && d.FechaFirma.HasValue)
                    .ToListAsync();

                // Agrupar por sector
                var documentosPorSector = documentos
                    .GroupBy(d => d.IdEmpresaNavigation.Sector ?? "Sin sector")
                    .Select(g => new DocumentosPorSectorDto
                    {
                        Sector = g.Key,
                        CantidadDocumentos = g.Count(),
                        Documentos = g.Select(d => new DocumentoResumenDto
                        {
                            NumeroConcierto = d.NumeroConcierto,
                            EmpresaNombre = d.IdEmpresaNavigation.RazonSocial,
                            FechaFirma = d.FechaFirma.Value // Ya validamos que tiene valor
                        }).ToList()
                    })
                    .OrderByDescending(x => x.CantidadDocumentos)
                    .ToList();

                // Generar PDF
                var documento = new InformeDocumentosPorSector(documentosPorSector);

                // Crear directorio si no existe
                var directorioInformes = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Informes");
                Directory.CreateDirectory(directorioInformes);

                // Generar nombre del archivo
                var nombreArchivo = $"Informe_Documentos_Por_Sector_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var rutaCompleta = Path.Combine(directorioInformes, nombreArchivo);

                // Generar el PDF
                documento.GeneratePdf(rutaCompleta);

                return rutaCompleta;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar el informe por sector: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Genera el informe resumen anual
        /// </summary>
        public async Task<string> GenerarInformeResumenAnual()
        {
            try
            {
                // Obtener todos los documentos con fecha válida
                var documentos = await _contexto.Documentos
                    .Where(d => d.FechaFirma.HasValue)
                    .Select(d => new { d.FechaFirma })
                    .ToListAsync();

                // Procesar agrupación en memoria
                var documentosPorAño = documentos
                    .GroupBy(d => d.FechaFirma.Value.Year)
                    .Select(g => new DocumentosPorAñoDto
                    {
                        Año = g.Key,
                        CantidadDocumentos = g.Count(),
                        DocumentosPorMes = g.GroupBy(d => d.FechaFirma.Value.Month)
                            .Select(mg => new DocumentosPorMesDto
                            {
                                Mes = mg.Key,
                                CantidadDocumentos = mg.Count()
                            }).ToList()
                    })
                    .OrderByDescending(x => x.Año)
                    .ToList();

                var documento = new InformeResumenAnual(documentosPorAño);

                var directorioInformes = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Informes");
                Directory.CreateDirectory(directorioInformes);

                var nombreArchivo = $"Informe_Resumen_Anual_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var rutaCompleta = Path.Combine(directorioInformes, nombreArchivo);

                documento.GeneratePdf(rutaCompleta);

                return rutaCompleta;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar el informe anual: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene estadísticas básicas para mostrar en pantalla
        /// </summary>
        public async Task<EstadisticasGeneralesDto> ObtenerEstadisticasGenerales()
        {
            try
            {
                //Total de documentos
                var totalDocumentos = await _contexto.Documentos.CountAsync();

                // Documentos de este año - Calculamos las fechas antes de la consulta
                var inicioAño = new DateTime(DateTime.Now.Year, 1, 1);
                var finAño = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);

                var documentosEsteAño = await _contexto.Documentos
                    .Where(d => d.FechaFirma.HasValue &&
                               d.FechaFirma.Value >= inicioAño &&
                               d.FechaFirma.Value <= finAño)
                    .CountAsync();

                // Documentos por sector - Traer datos y procesar en memoria
                var documentosConSector = await _contexto.Documentos
                    .Include(d => d.IdEmpresaNavigation)
                    .Where(d => d.IdEmpresaNavigation != null)
                    .Select(d => new { Sector = d.IdEmpresaNavigation.Sector ?? "Sin sector" })
                    .ToListAsync();

                var documentosPorSector = documentosConSector
                    .GroupBy(d => d.Sector)
                    .Select(g => new { Sector = g.Key, Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad)
                    .Take(5)
                    .ToList();

                return new EstadisticasGeneralesDto
                {
                    TotalDocumentos = totalDocumentos,
                    DocumentosEsteAño = documentosEsteAño,
                    Top5SectoresPorDocumentos = documentosPorSector.ToDictionary(x => x.Sector, x => x.Cantidad)
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas: {ex.Message}", ex);
            }
        }

    }

    // DTOs adicionales
    public class DocumentosPorAñoDto
    {
        public int Año { get; set; }
        public int CantidadDocumentos { get; set; }
        public List<DocumentosPorMesDto> DocumentosPorMes { get; set; } = new List<DocumentosPorMesDto>();
    }

    public class DocumentosPorMesDto
    {
        public int Mes { get; set; }
        public int CantidadDocumentos { get; set; }
        public string NombreMes => new DateTime(2000, Mes, 1).ToString("MMMM");
    }

    public class EstadisticasGeneralesDto
    {
        public int TotalDocumentos { get; set; }
        public int DocumentosEsteAño { get; set; }
        public Dictionary<string, int> Top5SectoresPorDocumentos { get; set; } = new Dictionary<string, int>();
    }
}
