using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Helpers;

namespace ProyectoDocumentales.Backend.Modelo.Utiles
{
    public class DocumentoPDF : IDocument
    {
        //Colores personalizados
        private static readonly string ColorPrimario = "#1d4355";
        private static readonly string ColorSecundario = "#00796B";
        private static readonly string ColorTexto = "#333333";
        private static readonly string ColorFondo = "#F5F5F5";
        private static readonly string ColorBordeFirma = "#A0C4D3";

        private readonly string _numero;
        private readonly string _fecha;
        private readonly string _centro;
        private readonly string _direccionCentro;
        private readonly string _cifCentro;
        private readonly string _directorCentro;
        private readonly string _empresa;
        private readonly string _sectorEmpresa;
        private readonly string _cifEmpresa;
        private readonly string _responsableEmpresa;

        public DocumentoPDF(
            string numero,
            string fecha,
            string centro,
            string direccionCentro,
            string cifCentro,
            string directorCentro,
            string empresa,
            string sectorEmpresa,
            string cifEmpresa,
            string responsableEmpresa)
        {
            _numero = numero;
            _fecha = fecha;
            _centro = centro;
            _direccionCentro = direccionCentro;
            _cifCentro = cifCentro;
            _directorCentro = directorCentro;
            _empresa = empresa;
            _sectorEmpresa = sectorEmpresa;
            _cifEmpresa = cifEmpresa;
            _responsableEmpresa = responsableEmpresa;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11).FontColor(ColorTexto));

                //Encabezado
                page.Header().Element(ComposeHeader);

                //Contenido
                page.Content().Element(ComposeContent);

                //Pie de pagina
                page.Footer().Element(ComposeFooter);
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                //Barra de color superior
                col.Item().Background(ColorPrimario).Padding(1).Height(8);

                col.Item().Padding(20).Row(row =>
                {
                    //Logo o título
                    row.RelativeItem().Column(titleCol =>
                    {
                        titleCol.Item().Text("DocuMe").FontSize(22).Bold().FontColor(ColorPrimario);
                        titleCol.Item().Text("Documento de Concierto").FontSize(16).FontColor(ColorSecundario).Medium();
                    });

                    //Informacion del documento
                    row.RelativeItem().AlignRight().Column(infoCol =>
                    {
                        infoCol.Item().Text($"Nº DE CONCIERTO: {_numero}").Bold().FontColor(ColorPrimario);
                        infoCol.Item().Text($"Fecha: {_fecha}").FontColor(ColorTexto);
                    });
                });

                //Linea separadora
                col.Item().LineHorizontal(1).LineColor(ColorSecundario);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(col =>
            {
                //Seccion de Centro Educativo
                col.Item().Background(ColorFondo).Padding(10).Column(sectionCol =>
                {
                    sectionCol.Item().Text("INFORMACIÓN DEL CENTRO EDUCATIVO").FontSize(14).Bold().FontColor(ColorPrimario);
                    sectionCol.Item().PaddingTop(5).Grid(grid =>
                    {
                        grid.Columns(2);
                        grid.Item().Text("Centro:").SemiBold();
                        grid.Item().Text(_centro);
                        grid.Item().Text("Dirección:").SemiBold();
                        grid.Item().Text(_direccionCentro);
                        grid.Item().Text("CIF:").SemiBold();
                        grid.Item().Text(_cifCentro);
                        grid.Item().Text("Director:").SemiBold();
                        grid.Item().Text(_directorCentro);
                    });
                });

                //Espacio
                col.Item().Height(20);

                //Seccion de Empresa
                col.Item().Background(ColorFondo).Padding(10).Column(sectionCol =>
                {
                    sectionCol.Item().Text("INFORMACIÓN DE LA EMPRESA").FontSize(14).Bold().FontColor(ColorPrimario);
                    sectionCol.Item().PaddingTop(5).Grid(grid =>
                    {
                        grid.Columns(2);
                        grid.Item().Text("Empresa:").SemiBold();
                        grid.Item().Text(_empresa);
                        grid.Item().Text("Sector:").SemiBold();
                        grid.Item().Text(_sectorEmpresa);
                        grid.Item().Text("CIF:").SemiBold();
                        grid.Item().Text(_cifEmpresa);
                        grid.Item().Text("Responsable:").SemiBold();
                        grid.Item().Text(_responsableEmpresa);
                    });
                });

                //Espacio para firmas
                col.Item().Height(40);

                //Seccion de firma del responsable
                col.Item().AlignCenter().PaddingTop(20).Column(signCol =>
                {
                    signCol.Item().Text("FIRMA DEL RESPONSABLE").FontSize(12).SemiBold().FontColor(ColorPrimario);

                    //Espacio para la firma
                    signCol.Item().PaddingVertical(10).Border(1).BorderColor(ColorBordeFirma).Height(60);

                    //Nombre del responsable
                    signCol.Item().PaddingTop(5).Text(_responsableEmpresa).SemiBold();

                    //Fecha de firma
                    signCol.Item().Text($"Fecha: {_fecha}");
                });
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                //Linea separadora
                col.Item().LineHorizontal(1).LineColor(ColorSecundario);

                col.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Text($"Documento generado el {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(9)
                        .FontColor(ColorTexto);

                    row.RelativeItem().AlignRight().Text("DocuMe")
                        .FontSize(9)
                        .SemiBold()
                        .FontColor(ColorPrimario);
                });

                //Barra de color inferior
                col.Item().PaddingTop(10).Background(ColorPrimario).Padding(1).Height(5);
            });
        }
    }
}
