using ProyectoDocumentales.Frontend.Modelo;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using QuestPDF.Helpers;

namespace ProyectoDocumentales.Backend.Modelo.Utiles
{
    public class GraficoPDF : IDocument
    {

        //Colores personalizados
        private static readonly string ColorPrimario = "#1d4355";
        private static readonly string ColorSecundario = "#00796B";
        private static readonly string ColorTexto = "#333333";
        private static readonly string ColorFondo = "#F5F5F5";

        private readonly List<DatoGrafico> _datosEmpresasPorSector;
        private readonly List<DatoGraficoAnio> _datosDocumentosPorAnio;
        private readonly int _totalDocumentos;
        private readonly int _totalEmpresas;
        private readonly int _totalSectores;
        private readonly int _documentosAnioActual;
        private readonly byte[] _imagenGrafico1;
        private readonly byte[] _imagenGrafico2;

        public GraficoPDF(
            List<DatoGrafico> datosEmpresasPorSector,
            List<DatoGraficoAnio> datosDocumentosPorAnio,
            int totalDocumentos,
            int totalEmpresas,
            int totalSectores,
            int documentosAnioActual,
            byte[] imagenGrafico1 = null,
            byte[] imagenGrafico2 = null)
        {
            _datosEmpresasPorSector = datosEmpresasPorSector;
            _datosDocumentosPorAnio = datosDocumentosPorAnio;
            _totalDocumentos = totalDocumentos;
            _totalEmpresas = totalEmpresas;
            _totalSectores = totalSectores;
            _documentosAnioActual = documentosAnioActual;
            _imagenGrafico1 = imagenGrafico1;
            _imagenGrafico2 = imagenGrafico2;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11).FontColor(ColorTexto));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Background(ColorPrimario).Padding(1).Height(8);

                col.Item().Padding(20).Row(row =>
                {
                    row.RelativeItem().Column(titleCol =>
                    {
                        titleCol.Item().Text("DocuMe").FontSize(22).Bold().FontColor(ColorPrimario);
                        titleCol.Item().Text("Reporte de Gráficos y Estadísticas").FontSize(16).FontColor(ColorSecundario).Medium();
                    });

                    row.RelativeItem().AlignRight().Column(infoCol =>
                    {
                        infoCol.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}").FontColor(ColorTexto);
                        infoCol.Item().Text($"Hora: {DateTime.Now:HH:mm}").FontColor(ColorTexto);
                    });
                });

                col.Item().LineHorizontal(1).LineColor(ColorSecundario);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(col =>
            {
                // Estadísticas Generales
                col.Item().Text("ESTADÍSTICAS GENERALES").FontSize(16).Bold().FontColor(ColorPrimario);

                col.Item().PaddingTop(10).Background(ColorFondo).Padding(15).Grid(grid =>
                {
                    grid.Columns(4);

                    // Total Documentos
                    grid.Item().Column(statCol =>
                    {
                        statCol.Item().AlignCenter().Text("Total Documentos").FontSize(12).SemiBold().FontColor(ColorSecundario);
                        statCol.Item().AlignCenter().Text(_totalDocumentos.ToString()).FontSize(20).Bold().FontColor(ColorPrimario);
                    });

                    // Total Empresas
                    grid.Item().Column(statCol =>
                    {
                        statCol.Item().AlignCenter().Text("Total Empresas").FontSize(12).SemiBold().FontColor(ColorSecundario);
                        statCol.Item().AlignCenter().Text(_totalEmpresas.ToString()).FontSize(20).Bold().FontColor(ColorPrimario);
                    });

                    // Total Sectores
                    grid.Item().Column(statCol =>
                    {
                        statCol.Item().AlignCenter().Text("Total Sectores").FontSize(12).SemiBold().FontColor(ColorSecundario);
                        statCol.Item().AlignCenter().Text(_totalSectores.ToString()).FontSize(20).Bold().FontColor(ColorPrimario);
                    });

                    // Documentos Año Actual
                    grid.Item().Column(statCol =>
                    {
                        statCol.Item().AlignCenter().Text("Docs. Año Actual").FontSize(12).SemiBold().FontColor(ColorSecundario);
                        statCol.Item().AlignCenter().Text(_documentosAnioActual.ToString()).FontSize(20).Bold().FontColor(ColorPrimario);
                    });
                });

                col.Item().Height(30);

                //Grafico de Empresas por Sector
                if (_datosEmpresasPorSector?.Count > 0)
                {
                    col.Item().Text("EMPRESAS POR SECTOR").FontSize(14).Bold().FontColor(ColorPrimario);

                    if (_imagenGrafico1 != null)
                    {
                        col.Item().PaddingTop(10).AlignCenter().Image(_imagenGrafico1).FitWidth();
                    }

                    col.Item().PaddingTop(10).Background(ColorFondo).Padding(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(ColorSecundario).Padding(5).Text("Sector").FontColor(QuestPDF.Helpers.Colors.White).SemiBold();
                            header.Cell().Background(ColorSecundario).Padding(5).Text("Cantidad").FontColor(QuestPDF.Helpers.Colors.White).SemiBold();
                        });

                        foreach (var dato in _datosEmpresasPorSector)
                        {
                            table.Cell().Border(0.5f).Padding(5).Text(dato.Sector);
                            table.Cell().Border(0.5f).Padding(5).AlignCenter().Text(dato.Cantidad.ToString());
                        }
                    });
                }

                col.Item().Height(30);

                //Grafico de Documentos por Año
                if (_datosDocumentosPorAnio?.Count > 0)
                {
                    col.Item().Text("DOCUMENTOS POR AÑO").FontSize(14).Bold().FontColor(ColorPrimario);

                    if (_imagenGrafico2 != null)
                    {
                        col.Item().PaddingTop(10).AlignCenter().Image(_imagenGrafico2).FitWidth();
                    }

                    col.Item().PaddingTop(10).Background(ColorFondo).Padding(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(ColorSecundario).Padding(5).Text("Año").FontColor(QuestPDF.Helpers.Colors.White).SemiBold();
                            header.Cell().Background(ColorSecundario).Padding(5).Text("Cantidad").FontColor(QuestPDF.Helpers.Colors.White).SemiBold();
                        });

                        foreach (var dato in _datosDocumentosPorAnio)
                        {
                            table.Cell().Border(0.5f).Padding(5).AlignCenter().Text(dato.Anio.ToString());
                            table.Cell().Border(0.5f).Padding(5).AlignCenter().Text(dato.Cantidad.ToString());
                        }
                    });
                }
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(ColorSecundario);

                col.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Text($"Reporte generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(9)
                        .FontColor(ColorTexto);

                    row.RelativeItem().AlignRight().Text("DocuMe")
                        .FontSize(9)
                        .SemiBold()
                        .FontColor(ColorPrimario);
                });

                col.Item().PaddingTop(10).Background(ColorPrimario).Padding(1).Height(5);
            });
        }
    }

    //Clase auxiliar para capturar graficos como imagen
    public static class GraficoImagenHelper
    {
        public static byte[] CapturarGraficoComoImagen(FrameworkElement elemento)
        {
            try
            {
                var renderTargetBitmap = new RenderTargetBitmap(
                    (int)elemento.ActualWidth,
                    (int)elemento.ActualHeight,
                    96, 96,
                    PixelFormats.Pbgra32);

                renderTargetBitmap.Render(elemento);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    return stream.ToArray();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
