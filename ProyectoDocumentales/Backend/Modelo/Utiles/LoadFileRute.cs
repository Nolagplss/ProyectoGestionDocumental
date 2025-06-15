using ProyectoDocumentales.MVVM;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;

namespace ProyectoDocumentales.Backend.Modelo.Utiles
{
    public static class LoadFileRute
    {
        public static void LoadFile(TextBox txtRuta, MvDocumentos _mvDocumentos)

        {

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos PDF|*.pdf",
                Title = "Selecciona un archivo PDF",
                //Ruta inicial relativa al ejecutable/proyecto
                InitialDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Rutas")
            };
            //Ruta base relativa al ejecutable
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string rutaInicial = Path.Combine(basePath, "Rutas");

            //Verificamos que la carpeta exista, si no, la creamos
            if (!Directory.Exists(rutaInicial))
            {
                Directory.CreateDirectory(rutaInicial);
            }

            openFileDialog.InitialDirectory = rutaInicial;

            if (openFileDialog.ShowDialog() == true)
            {
                //Ruta del archivo
                string rutaArchivo = openFileDialog.FileName;

                //Nombre del archivo
                string nombreArchivo = System.IO.Path.GetFileName(rutaArchivo);
                txtRuta.Text = rutaArchivo;

                //Guardamos en el viewModel la ruta del archivo
                _mvDocumentos.ArchivoSeleccionado = rutaArchivo;


            }
        }
    }
}
