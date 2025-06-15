using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Globalization;
using ProyectoDocumentales.Servicios;


namespace ProyectoDocumentales.Backend.Modelo.Utiles
{
    public class InformeResumenAnual : IDocument
    {
        private static readonly string ColorPrimario = "#1d4355";
        private static readonly string ColorSecundario = "#00796B";
        private static readonly string ColorTexto = "#333333";
        private static readonly string ColorFondo = "#F5F5F5";

        private readonly List<DocumentosPorAñoDto> _documentosPorAño;
        private readonly string _fechaGeneracion;

        public InformeResumenAnual(List<DocumentosPorAñoDto> documentosPorAño)
        {
            _documentosPorAño = documentosPorAño ?? new List<DocumentosPorAñoDto>();
            _fechaGeneracion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
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
                // Barra superior
                col.Item().Background(ColorPrimario).Padding(1).Height(8);

                col.Item().Padding(20).Row(row =>
                {
                    row.RelativeItem().Column(titleCol =>
                    {
                        titleCol.Item().Text("DocuMe").FontSize(22).Bold().FontColor(ColorPrimario);
                        titleCol.Item().Text("Informe Resumen Anual").FontSize(16).FontColor(ColorSecundario).Medium();
                    });

                    row.RelativeItem().AlignRight().Column(infoCol =>
                    {
                        infoCol.Item().Text($"Fecha: {_fechaGeneracion}").FontColor(ColorTexto);
                        infoCol.Item().Text($"Total años: {_documentosPorAño.Count}").Bold().FontColor(ColorPrimario);
                    });
                });

                col.Item().LineHorizontal(1).LineColor(ColorSecundario);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(col =>
            {
                // Resumen general
                col.Item().Text("RESUMEN POR AÑOS").FontSize(16).Bold().FontColor(ColorPrimario);
                col.Item().PaddingTop(10);

                // Tabla resumen anual
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1); // Año
                        columns.RelativeColumn(2); // Total documentos
                        columns.RelativeColumn(2); // Promedio mensual
                    });

                    // Encabezados
                    table.Header(header =>
                    {
                        header.Cell().Background(ColorSecundario).Padding(8)
                            .Text("Año").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorSecundario).Padding(8)
                            .Text("Total Documentos").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorSecundario).Padding(8)
                            .Text("Promedio/Mes").FontColor(Colors.White).Bold();
                    });

                    // Datos
                    foreach (var año in _documentosPorAño.OrderByDescending(x => x.Año))
                    {
                        var promedioMensual = año.CantidadDocumentos / 12.0;

                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .AlignCenter().Text(año.Año.ToString());
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .AlignCenter().Text(año.CantidadDocumentos.ToString());
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .AlignCenter().Text($"{promedioMensual:F1}");
                    }
                });

                col.Item().PaddingTop(30);

                // Detalle mensual por año
                col.Item().Text("DETALLE MENSUAL POR AÑO").FontSize(16).Bold().FontColor(ColorPrimario);
                col.Item().PaddingTop(10);

                foreach (var año in _documentosPorAño.OrderByDescending(x => x.Año))
                {
                    col.Item().PaddingTop(20).Column(añoCol =>
                    {
                        añoCol.Item().Background(ColorFondo).Padding(10).Row(row =>
                        {
                            row.RelativeItem().Text($"Año {año.Año}")
                                .FontSize(14).Bold().FontColor(ColorPrimario);
                            row.RelativeItem().AlignRight().Text($"Total: {año.CantidadDocumentos} documentos")
                                .FontSize(12).FontColor(ColorSecundario);
                        });

                        // Tabla mensual
                        añoCol.Item().PaddingTop(10).Table(monthTable =>
                        {
                            monthTable.ColumnsDefinition(columns =>
                            {
                                // 4 columnas para mostrar 3 meses por fila
                                for (int i = 0; i < 4; i++)
                                    columns.RelativeColumn();
                            });

                            // Generar datos mensuales completos (1-12)
                            var mesesCompletos = new List<DocumentosPorMesDto>();
                            for (int mes = 1; mes <= 12; mes++)
                            {
                                var mesData = año.DocumentosPorMes?.FirstOrDefault(m => m.Mes == mes);
                                mesesCompletos.Add(new DocumentosPorMesDto
                                {
                                    Mes = mes,
                                    CantidadDocumentos = mesData?.CantidadDocumentos ?? 0
                                });
                            }

                            // Mostrar en filas de 4 columnas
                            for (int i = 0; i < mesesCompletos.Count; i += 4)
                            {
                                for (int j = 0; j < 4 && (i + j) < mesesCompletos.Count; j++)
                                {
                                    var mes = mesesCompletos[i + j];
                                    var nombreMes = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mes.Mes);

                                    monthTable.Cell().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(5)
                                        .Column(mesCol =>
                                        {
                                            mesCol.Item().Text(nombreMes).FontSize(9).Bold();
                                            mesCol.Item().Text($"{mes.CantidadDocumentos} docs").FontSize(8).FontColor(ColorTexto);
                                        });
                                }
                            }
                        });
                    });
                }

                // Estadísticas adicionales
                if (_documentosPorAño.Any())
                {
                    col.Item().PaddingTop(30);
                    col.Item().Text("ESTADÍSTICAS GENERALES").FontSize(16).Bold().FontColor(ColorPrimario);
                    col.Item().PaddingTop(10);

                    var totalDocumentos = _documentosPorAño.Sum(x => x.CantidadDocumentos);
                    var añoConMasDocumentos = _documentosPorAño.OrderByDescending(x => x.CantidadDocumentos).First();
                    var promedioAnual = totalDocumentos / (double)_documentosPorAño.Count;

                    col.Item().Background(ColorFondo).Padding(15).Column(statsCol =>
                    {
                        statsCol.Item().Container().PaddingBottom(5).Text($"• Total de documentos: {totalDocumentos}")
                            .FontSize(12);

                        statsCol.Item().Container().PaddingBottom(5).Text($"• Año con más documentos: {añoConMasDocumentos.Año} ({añoConMasDocumentos.CantidadDocumentos} documentos)")
                           .FontSize(12);

                        statsCol.Item().Text($"• Promedio anual: {promedioAnual:F1} documentos/año")
                            .FontSize(12);
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
                    row.RelativeItem().Text($"Informe generado el {_fechaGeneracion}")
                        .FontSize(9).FontColor(ColorTexto);
                    row.RelativeItem().AlignRight().Text("Documentador 3000")
                        .FontSize(9).SemiBold().FontColor(ColorPrimario);
                });
                col.Item().PaddingTop(10).Background(ColorPrimario).Padding(1).Height(5);
            });
        }
    }

}
