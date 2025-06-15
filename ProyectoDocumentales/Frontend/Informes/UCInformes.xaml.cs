using NLog;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.MVVM;
using QuestPDF.Infrastructure;
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

namespace ProyectoDocumentales.Frontend.Informes
{


    /// <summary>
    /// Lógica de interacción para UCInformes.xaml
    /// </summary>
    public partial class UCInformes : UserControl
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private MvInformes _mvInformes;

        public UCInformes(MvInformes mvInformes)
        {
            InitializeComponent();

            _mvInformes = mvInformes; 

            DataContext = _mvInformes;
            logger.Info("UCInformes inicializado.");

        }



        private async void BtnInformePorSector_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.Info("Generación de informe por sector iniciada.");

                QuestPDF.Settings.License = LicenseType.Community;
                if (_mvInformes != null)
                {
                    //Mostramos barra de progreso
                    progressBarGeneracion.Visibility = Visibility.Visible;

                    //Deshabilitar el boton mientras se genera el informe
                    BtnInformePorSector.IsEnabled = false;
                    BtnInformeAnual.IsEnabled = false;

                    await _mvInformes.GenerarInformePorSector();
                    logger.Info("Informe por sector generado correctamente.");

                }
                else
                {
                    logger.Warn("Intento de generar informe por sector pero _mvInformes es null.");

                    MessageBox.Show("El sistema no está inicializado correctamente.",
                                   "Error",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error inesperado al generar el informe por sector.");

                MessageBox.Show($"Error inesperado al generar el informe por sector: {ex.Message}",
                               "Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
            finally
            {
                //Rehabilitar los botones
                BtnInformePorSector.IsEnabled = true;
                BtnInformeAnual.IsEnabled = true;

                //Ocultamos barra de progreso
                progressBarGeneracion.Visibility = Visibility.Collapsed;

            }
        }

        /// <summary>
        /// Maneja el clic del botón para generar informe anual
        /// </summary>
        private async void BtnInformeAnual_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Generación de informe anual iniciada.");

            try
            {
                QuestPDF.Settings.License = LicenseType.Community;
                if (_mvInformes != null)
                {
                    //Mostramos barra de progreso
                    progressBarGeneracion.Visibility = Visibility.Visible;

                    //Deshabilitar el boton mientras se genera el informe
                    BtnInformePorSector.IsEnabled = false;
                    BtnInformeAnual.IsEnabled = false;

                    await _mvInformes.GenerarInformeAnual();
                    logger.Info("Informe anual generado correctamente.");

                }
                else
                {
                    logger.Warn("Intento de generar informe anual pero _mvInformes es null.");

                    MessageBox.Show("El sistema no está inicializado correctamente.",
                                   "Error",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error inesperado al generar el informe anual.");

                MessageBox.Show($"Error inesperado al generar el informe anual: {ex.Message}",
                               "Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
            finally
            {
                // Rehabilitar los botones
                BtnInformePorSector.IsEnabled = true;
                BtnInformeAnual.IsEnabled = true;
                //Ocultar barra de progreso
                progressBarGeneracion.Visibility = Visibility.Collapsed;
            }
        }

        //Refresca las estadisticas
        private async void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Refrescando estadísticas iniciado.");

            try
            {
                if (_mvInformes != null)
                {
                    //Mostramos barra progreso
                    progressBarEstadisticas.Visibility = Visibility.Visible;

                    //Deshabilitar el boton mientras se cargan las estadísticas
                    BtnRefrescar.IsEnabled = false;

                    await _mvInformes.RefrescarEstadisticas();

                    logger.Info("Estadísticas refrescadas correctamente.");

                }
                else
                {
                    logger.Warn("Intento de refrescar estadísticas pero _mvInformes es null.");

                    MessageBox.Show("El sistema no está inicializado correctamente.",
                                   "Error",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al refrescar las estadísticas.");

                MessageBox.Show($"Error al refrescar las estadísticas: {ex.Message}",
                               "Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
            finally
            {
                //Rehabilitar el boton
                BtnRefrescar.IsEnabled = true;

                //Ocultamos barra progreso
                progressBarEstadisticas.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Limpia recursos cuando el UserControl se descarga
        /// </summary>
        private void UCInformes_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.Info("UCInformes descargado, limpiando recursos.");

                //Limpiar el DataContext para evitar memory leaks
                this.DataContext = null;
                _mvInformes = null;
            }
            catch (Exception ex)
            {
                //Logg error
                logger.Error(ex, "Error al limpiar recursos en UCInformes_Unloaded.");
            }
        }

      
        public async Task ActualizarEstadisticas()
        {
            logger.Info("Actualizando estadísticas (método público).");

            try
            {
                if (_mvInformes != null)
                {
                    await _mvInformes.RefrescarEstadisticas();
                    logger.Info("Estadísticas actualizadas correctamente.");

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al actualizar las estadísticas.");

                MessageBox.Show($"Error al actualizar las estadísticas: {ex.Message}",
                               "Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }
    }
}
