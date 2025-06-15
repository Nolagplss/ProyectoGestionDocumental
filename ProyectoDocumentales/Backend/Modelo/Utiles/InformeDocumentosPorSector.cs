using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;



namespace ProyectoDocumentales.Backend.Modelo.Utiles
{
    public class InformeDocumentosPorSector : IDocument
    {

        private static readonly string ColorPrimario = "#1d4355";
        private static readonly string ColorSecundario = "#00796B";
        private static readonly string ColorTexto = "#333333";
        private static readonly string ColorFondo = "#F5F5F5";

        private readonly List<DocumentosPorSectorDto> _documentosPorSector;
        private readonly string _fechaGeneracion;

        public InformeDocumentosPorSector(List<DocumentosPorSectorDto> documentosPorSector)
        {
            _documentosPorSector = documentosPorSector;
            _fechaGeneracion = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        public DocumentMetadata GetMetadata() => new DocumentMetadata
        {
            Title = "Informe de Documentos por Sector",
            Author = "DocuMe"
        };

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
                //Barra superior
                col.Item().Background(ColorPrimario).Padding(1).Height(8);

                col.Item().Padding(20).Row(row =>
                {
                    row.RelativeItem().Column(titleCol =>
                    {
                        titleCol.Item().Text("DocuMe").FontSize(22).Bold().FontColor(ColorPrimario);
                        titleCol.Item().Text("Informe de Documentos por Sector").FontSize(16).FontColor(ColorSecundario).Medium();
                    });

                    row.RelativeItem().AlignRight().Column(infoCol =>
                    {
                        infoCol.Item().Text($"Fecha: {_fechaGeneracion}").FontColor(ColorTexto);
                        infoCol.Item().Text($"Total documentos: {_documentosPorSector.Sum(x => x.CantidadDocumentos)}").Bold().FontColor(ColorPrimario);
                    });
                });

                col.Item().LineHorizontal(1).LineColor(ColorSecundario);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(col =>
            {
                //Resumen por sectores
                col.Item().Text("RESUMEN POR SECTORES").FontSize(16).Bold().FontColor(ColorPrimario);
                col.Item().PaddingTop(10);

                //Tabla resumen
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3); //Sector
                        columns.RelativeColumn(1); //Cantidad
                        columns.RelativeColumn(2); //Porcentaje
                    });

                    //Encabezados
                    table.Header(header =>
                    {
                        header.Cell().Background(ColorSecundario).Padding(8)
                            .Text("Sector").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorSecundario).Padding(8)
                            .Text("Documentos").FontColor(Colors.White).Bold();
                        header.Cell().Background(ColorSecundario).Padding(8)
                            .Text("Porcentaje").FontColor(Colors.White).Bold();
                    });

                    //Datos
                    var totalDocumentos = _documentosPorSector.Sum(x => x.CantidadDocumentos);

                    foreach (var item in _documentosPorSector.OrderByDescending(x => x.CantidadDocumentos))
                    {
                        var porcentaje = totalDocumentos > 0 ? (item.CantidadDocumentos * 100.0 / totalDocumentos) : 0;

                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .Text(item.Sector ?? "Sin sector");
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .AlignCenter().Text(item.CantidadDocumentos.ToString());
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(8)
                            .AlignCenter().Text($"{porcentaje:F1}%");
                    }
                });

                col.Item().PaddingTop(30);

                //Detalle por sector
                col.Item().Text("DETALLE POR SECTOR").FontSize(16).Bold().FontColor(ColorPrimario);
                col.Item().PaddingTop(10);

                foreach (var sector in _documentosPorSector.OrderByDescending(x => x.CantidadDocumentos))
                {
                    col.Item().PaddingTop(15).Column(sectorCol =>
                    {
                        sectorCol.Item().Background(ColorFondo).Padding(10).Row(row =>
                        {
                            row.RelativeItem().Text($"Sector: {sector.Sector ?? "Sin sector"}")
                                .FontSize(14).Bold().FontColor(ColorPrimario);
                            row.RelativeItem().AlignRight().Text($"Total: {sector.CantidadDocumentos} documentos")
                                .FontSize(12).FontColor(ColorSecundario);
                        });

                        //Lista de documentos del sector
                        if (sector.Documentos?.Any() == true)
                        {
                            sectorCol.Item().PaddingTop(8).Column(docCol =>
                            {
                                foreach (var doc in sector.Documentos.Take(10)) //Limitar a 10 por pagina
                                {
                                    docCol.Item().PaddingVertical(2).Row(docRow =>
                                    {
                                        docRow.RelativeItem().Text($"• Nº {doc.NumeroConcierto}")
                                            .FontSize(10);
                                        docRow.RelativeItem().Text($"{doc.EmpresaNombre}")
                                            .FontSize(10);
                                        docRow.RelativeItem().AlignRight().Text($"{doc.FechaFirma:dd/MM/yyyy}")
                                            .FontSize(10).FontColor(ColorTexto);
                                    });
                                }

                                if (sector.Documentos.Count > 10)
                                {
                                    docCol.Item().PaddingTop(5).Text($"... y {sector.Documentos.Count - 10} documentos más")
                                        .FontSize(9).Italic().FontColor(ColorTexto);
                                }
                            });
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
                    row.RelativeItem().Text($"Informe generado el {_fechaGeneracion}")
                        .FontSize(9).FontColor(ColorTexto);
                    row.RelativeItem().AlignRight().Text("Documentador 3000")
                        .FontSize(9).SemiBold().FontColor(ColorPrimario);
                });
                col.Item().PaddingTop(10).Background(ColorPrimario).Padding(1).Height(5);
            });
        }
    }
    //DTO para agrupar datos
    public class DocumentosPorSectorDto
    {
        public string Sector { get; set; }
        public int CantidadDocumentos { get; set; }
        public List<DocumentoResumenDto> Documentos { get; set; } = new List<DocumentoResumenDto>();
    }

    public class DocumentoResumenDto
    {
        public string NumeroConcierto { get; set; }
        public string EmpresaNombre { get; set; }
        public DateTime FechaFirma { get; set; }
    }

}
