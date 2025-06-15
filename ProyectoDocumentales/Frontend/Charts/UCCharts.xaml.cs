using LiveCharts.Wpf;
using LiveCharts;
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
using ProyectoDocumentales.Backend.Modelo.Utiles;
using ProyectoDocumentales.Frontend.Modelo;
using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using MediaColor = System.Windows.Media.Color;
using PdfColor = QuestPDF.Infrastructure.Color;
using ProyectoDocumentales.Backend.Modelo;

namespace ProyectoDocumentales.Frontend.Charts
{
   
    public partial class UCCharts : UserControl
    {
        private MvGraficos _mvGraficos;

        public UCCharts(MvGraficos mvGraficos)
        {
            InitializeComponent();

            _mvGraficos = mvGraficos;

            DataContext = _mvGraficos;
        }

        private void comboTipoGrafico_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mvGraficos == null) return;

            ComboBox combo = sender as ComboBox;
            if (combo?.SelectedItem is ComboBoxItem selectedItem)
            {
                string tipoGrafico = selectedItem.Content.ToString();
                CrearGrafico(tipoGrafico);

                //Habilitar el boto de exportar grafico actual
                if (btnExportarGraficoActual != null)
                {
                    btnExportarGraficoActual.IsEnabled = true;
                }
            }
        }

        private void CrearGrafico(string tipoGrafico)
        {
            gridGrafico.Children.Clear();

            switch (tipoGrafico)
            {
                case "Número de empresas por sector":
                    CrearGraficoEmpresasPorSector();
                    break;
                case "Número de documentos por año":
                    CrearGraficoDocumentosPorAnio();
                    break;
            }
        }

        private void CrearGraficoEmpresasPorSector()
        {
            var datos = _mvGraficos.DatosEmpresasPorSector;

            var chart = new PieChart
            {
                LegendLocation = LegendLocation.Right,
                Series = new SeriesCollection(),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
            };

            //Colores para el grafico de sectores
            var colores = new[]
            {
                Brushes.DodgerBlue,
                Brushes.LimeGreen,
                Brushes.Orange,
                Brushes.Red,
                Brushes.Purple,
                Brushes.Teal,
                Brushes.Gold,
                Brushes.DeepPink
            };

            int colorIndex = 0;
            foreach (var dato in datos)
            {
                chart.Series.Add(new PieSeries
                {
                    Title = dato.Sector,
                    Values = new ChartValues<int> { dato.Cantidad },
                    DataLabelsTemplate = CreateDataLabelTemplate(),
                    Fill = colores[colorIndex % colores.Length]
                });
                colorIndex++;
            }

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var titulo = new TextBlock
            {
                Text = "Número de Empresas por Sector",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20),
                Foreground = new SolidColorBrush(MediaColor.FromRgb(29, 67, 85))
            };

            Grid.SetRow(titulo, 0);
            Grid.SetRow(chart, 1);

            grid.Children.Add(titulo);
            grid.Children.Add(chart);

            gridGrafico.Children.Add(grid);
        }

        private void CrearGraficoDocumentosPorAnio()
        {
            var datos = _mvGraficos.DatosDocumentosPorAnio;

            var chart = new CartesianChart
            {
                Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Documentos",
                        Values = new ChartValues<int>(datos.Select(d => d.Cantidad)),
                        Fill = new SolidColorBrush(MediaColor.FromRgb(0, 121, 107)),
                        DataLabels = true
                    }
                },
                AxisX = new AxesCollection
                {
                    new Axis
                    {
                        Title = "Año",
                        Labels = datos.Select(d => d.Anio.ToString()).ToArray(),
                        Separator = new LiveCharts.Wpf.Separator
                        {
                            StrokeThickness = 1,
                            StrokeDashArray = new DoubleCollection(new double[] { 2 }),
                            Stroke = new SolidColorBrush(MediaColor.FromRgb(64, 79, 86))
                        }
                    }
                },
                AxisY = new AxesCollection
                {
                    new Axis
                    {
                        Title = "Cantidad de Documentos",
                        LabelFormatter = value => value.ToString("N0"),
                        Separator = new LiveCharts.Wpf.Separator
                        {
                            StrokeThickness = 1,
                            StrokeDashArray = new DoubleCollection(new double[] { 2 }),
                            Stroke = new SolidColorBrush(MediaColor.FromRgb(64, 79, 86))
                        }
                    }
                },
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var titulo = new TextBlock
            {
                Text = "Número de Documentos por Año",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20),
                Foreground = new SolidColorBrush(MediaColor.FromRgb(29, 67, 85))
            };

            Grid.SetRow(titulo, 0);
            Grid.SetRow(chart, 1);

            grid.Children.Add(titulo);
            grid.Children.Add(chart);

            gridGrafico.Children.Add(grid);
        }

        private DataTemplate CreateDataLabelTemplate()
        {
            var template = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Value"));
            factory.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            factory.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            template.VisualTree = factory;
            return template;
        }

        private void btnExportarGraficoActual_Click(object sender, RoutedEventArgs e)
        {

            ExportarGraficoActualPDF();
        }


        //Metodo alternativo para capturar grafico especifico
        private byte[] CapturarGraficoActual()
        {
            try
            {
                if (gridGrafico.Children.Count > 0)
                {
                    var elemento = gridGrafico.Children[0] as FrameworkElement;
                    if (elemento != null)
                    {
                        return GraficoImagenHelper.CapturarGraficoComoImagen(elemento);
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        //Exportar solo el grafico actual
        public void ExportarGraficoActualPDF()
        {
            try
            {
              
                QuestPDF.Settings.License = LicenseType.Community;

                if (gridGrafico.Children.Count == 0)
                {
                    MessageBox.Show("No hay ningún gráfico para exportar.",
                        "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                byte[] imagenGrafico = CapturarGraficoActual();

                //Determinar que tipo de grafico es el actual
                var comboItem = comboTipoGrafico.SelectedItem as ComboBoxItem;
                string tipoGrafico = comboItem?.Content?.ToString() ?? "Gráfico";

                List<DatoGrafico> datosEmpresasPorSector = null;
                List<DatoGraficoAnio> datosDocumentosPorAnio = null;

                if (tipoGrafico == "Número de empresas por sector")
                {
                    datosEmpresasPorSector = _mvGraficos.DatosEmpresasPorSector;
                }
                else if (tipoGrafico == "Número de documentos por año")
                {
                    datosDocumentosPorAnio = _mvGraficos.DatosDocumentosPorAnio;
                }

                var graficoPdf = new GraficoPDF(
                    datosEmpresasPorSector ?? new List<DatoGrafico>(),
                    datosDocumentosPorAnio ?? new List<DatoGraficoAnio>(),
                    _mvGraficos.TotalDocumentos,
                    _mvGraficos.TotalEmpresas,
                    _mvGraficos.TotalSectores,
                    _mvGraficos.DocumentosAnioActual,
                    tipoGrafico == "Número de empresas por sector" ? imagenGrafico : null,
                    tipoGrafico == "Número de documentos por año" ? imagenGrafico : null
                );

                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    DefaultExt = "pdf",
                    FileName = $"{tipoGrafico.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    graficoPdf.GeneratePdf(saveFileDialog.FileName);

                    MessageBox.Show($"PDF generado exitosamente en:\n{saveFileDialog.FileName}",
                        "Exportación Exitosa", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el PDF: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }


}
