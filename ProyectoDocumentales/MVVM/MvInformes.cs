using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.MVVM.Base;
using ProyectoDocumentales.Servicios;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoDocumentales.MVVM
{
    public class MvInformes : MVBase
    {
        private readonly Documentales2Context _contexto;
        private readonly InformeServicio _informeServicio;

        //Propiedades para estadisticas
        private int _totalDocumentos;
        public int TotalDocumentos
        {
            get => _totalDocumentos;
            set
            {
                _totalDocumentos = value;
                OnPropertyChanged(nameof(TotalDocumentos));
            }
        }

        private int _documentosEsteAño;
        public int DocumentosEsteAño
        {
            get => _documentosEsteAño;
            set
            {
                _documentosEsteAño = value;
                OnPropertyChanged(nameof(DocumentosEsteAño));
            }
        }

        private string _sectorMasDocumentos;
        public string SectorMasDocumentos
        {
            get => _sectorMasDocumentos;
            set
            {
                _sectorMasDocumentos = value;
                OnPropertyChanged(nameof(SectorMasDocumentos));
            }
        }

        private bool _cargandoEstadisticas;
        public bool CargandoEstadisticas
        {
            get => _cargandoEstadisticas;
            set
            {
                _cargandoEstadisticas = value;
                OnPropertyChanged(nameof(CargandoEstadisticas));
            }
        }

        private bool _generandoInforme;
        public bool GenerandoInforme
        {
            get => _generandoInforme;
            set
            {
                _generandoInforme = value;
                OnPropertyChanged(nameof(GenerandoInforme));
            }
        }

        public MvInformes(Documentales2Context contexto)
        {
            _contexto = contexto;
            _informeServicio = new InformeServicio(_contexto);
        }

        public async Task Inicializar()
        {
            await CargarEstadisticas();
        }

        public async Task CargarEstadisticas()
        {
            try
            {
                CargandoEstadisticas = true;

                //Guardamos en variable lo que devuelven las estadisticas
                var estadisticas = await _informeServicio.ObtenerEstadisticasGenerales();

                //Asignamos a los atributos su variable obtenida de las estadisticas
                TotalDocumentos = estadisticas.TotalDocumentos;
                DocumentosEsteAño = estadisticas.DocumentosEsteAño;

                //Obtener el sector con mas documentos
                if (estadisticas.Top5SectoresPorDocumentos.Count > 0)
                {
                    var sectorTop = estadisticas.Top5SectoresPorDocumentos.First();
                    SectorMasDocumentos = $"{sectorTop.Key} ({sectorTop.Value} docs)";
                }
                else
                {
                    SectorMasDocumentos = "Sin datos";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar estadísticas: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CargandoEstadisticas = false;
            }
        }

        public async Task GenerarInformePorSector()
        {
            try
            {
                GenerandoInforme = true;
                var rutaArchivo = await _informeServicio.GenerarInformeDocumentosPorSector();

                var resultado = MessageBox.Show(
                    $"Informe generado correctamente.\n\nUbicación: {rutaArchivo}\n\n¿Desea abrirlo ahora?",
                    "Informe Generado",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (resultado == MessageBoxResult.Yes)
                {
                    AbrirArchivo(rutaArchivo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el informe por sector: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                GenerandoInforme = false;
            }
        }

        public async Task GenerarInformeAnual()
        {
            try
            {
                GenerandoInforme = true;
                var rutaArchivo = await _informeServicio.GenerarInformeResumenAnual();

                var resultado = MessageBox.Show(
                    $"Informe generado correctamente.\n\nUbicación: {rutaArchivo}\n\n¿Desea abrirlo ahora?",
                    "Informe Generado",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (resultado == MessageBoxResult.Yes)
                {
                    AbrirArchivo(rutaArchivo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el informe anual: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                GenerandoInforme = false;
            }
        }

        private void AbrirArchivo(string rutaArchivo)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = rutaArchivo,
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public async Task RefrescarEstadisticas()
        {
            await CargarEstadisticas();
        }
    }
}
