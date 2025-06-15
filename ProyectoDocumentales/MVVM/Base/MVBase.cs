using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace ProyectoDocumentales.MVVM.Base
{
    public class MVBase : PropertyChangedDataError
    {
        /// <summary>
        /// Botón del formulario que queremos que se active/desactive en función
        /// de si hay errores en la validación de los campos
        /// </summary>
        public Button btnGuardar { get; set; }

        public Button btnGenerarDocumento { get; set; }

      

        /// <summary>
        /// Variable que llev la cuenta de los errores que hay en el formulario
        /// </summary>
        private int errorCount;

        /// <summary>
        /// Diccionario para manejar múltiples secciones con sus propios botones y contadores de errores
        /// </summary>
        private Dictionary<string, ButtonValidationInfo> _validationSections;
        //Constructor
        public MVBase()
        {
            _validationSections = new Dictionary<string, ButtonValidationInfo>();

        }
      
        /// <summary>
        /// Registra una nueva sección de validación con su botón correspondiente
        /// </summary>
        /// <param name="sectionName">Nombre de la sección</param>
        /// <param name="button">Botón asociado a esta sección</param>
        /// <param name="fieldNames">Nombres de los campos que pertenecen a esta sección</param>
        public void RegisterValidationSection(string sectionName, Button button, params string[] fieldNames)
        {
            _validationSections[sectionName] = new ButtonValidationInfo
            {
                Button = button,
                ErrorCount = 0,
                FieldNames = new HashSet<string>(fieldNames)
            };
        }


        // Añade métodos que nos ayudan en la validación -------------------------------------------------------------------------------
        /// <summary>
        /// Método que nos permite saber si hay errores en algún formulario
        /// </summary>
        /// <param name="obj">Ventana o panel que contiene los controles del formulario que queremos comprobar</param>
        /// <returns>n caso de que haya errores devolverá el valor de falso y en caso de que no haya devolverá verdadero</returns>
        public bool IsValid(DependencyObject obj)
        {
            // The dependency object is valid if it has no errors and all
            // of its children (that are dependency objects) are error-free.
            return !Validation.GetHasError(obj) &&
            LogicalTreeHelper.GetChildren(obj)
            .OfType<DependencyObject>()
            .All(IsValid);
        }

        /// <summary>
        /// Manejador de eventos de validación que puede manejar múltiples secciones
        /// </summary>
        public void OnErrorEvent(object sender, RoutedEventArgs e)
        {
            var validationEventArgs = e as ValidationErrorEventArgs;
            if (validationEventArgs == null)
                throw new Exception("Argumentos inesperados");

            // Obtener el nombre del campo que produjo el error
            string fieldName = GetFieldName(validationEventArgs.OriginalSource);

            switch (validationEventArgs.Action)
            {
                case ValidationErrorEventAction.Added:
                    errorCount++;
                    UpdateSectionErrorCount(fieldName, 1);
                    break;
                case ValidationErrorEventAction.Removed:
                    errorCount--;
                    UpdateSectionErrorCount(fieldName, -1);
                    break;
                default:
                    throw new Exception("Acción desconocida");
            }

            // Actualizar botón principal
            if (btnGuardar != null)
                btnGuardar.IsEnabled = errorCount == 0;

            if (btnGenerarDocumento != null)
                btnGenerarDocumento.IsEnabled = errorCount == 0;

            // Actualizar botones de secciones
            UpdateSectionButtons();
        }

        /// <summary>
        /// Obtiene el nombre del campo desde el elemento que produjo el error
        /// </summary>
        private string GetFieldName(object source)
        {
            if (source is FrameworkElement element)
            {
                // Intentar obtener el nombre del binding
                var binding = element.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    return binding.ParentBinding.Path.Path;
                }
                // Si no hay binding, usar el nombre del control
                return element.Name ?? "Unknown";
            }
            return "Unknown";
        }

        /// <summary>
        /// Actualiza el contador de errores para las secciones que contienen el campo especificado
        /// </summary>
        private void UpdateSectionErrorCount(string fieldName, int delta)
        {
            foreach (var section in _validationSections.Values)
            {
                if (section.FieldNames.Contains(fieldName))
                {
                    section.ErrorCount += delta;
                    section.ErrorCount = Math.Max(0, section.ErrorCount); // Asegurar que no sea negativo
                }
            }
        }

        /// <summary>
        /// Actualiza el estado de todos los botones de sección
        /// </summary>
        private void UpdateSectionButtons()
        {
            foreach (var section in _validationSections.Values)
            {
                if (section.Button != null)
                {
                    section.Button.IsEnabled = section.ErrorCount == 0;
                }
            }
        }

        /// <summary>
        /// Clase interna para almacenar información de validación por sección
        /// </summary>
        private class ButtonValidationInfo
        {
            public Button Button { get; set; }
            public int ErrorCount { get; set; }
            public HashSet<string> FieldNames { get; set; }
        }

        /*
        /// <summary>
        /// Deshabilita el botón de guardar si hay errores en el formulario
        /// Si no hay errores habilitará el botón.
        /// se trata de un manejador de eventos. El framework, cada vez que se produce un error de validación
        /// en el formulario lanza un evento que manejamos en este procedimiento
        /// </summary>
        /// <param name="sender">Control que produce el error de validación</param>
        /// <param name="e">Parámetros del error</param>
        public void OnErrorEvent(object sender, RoutedEventArgs e)
        {
            var validationEventArgs = e as ValidationErrorEventArgs;
            if (validationEventArgs == null)
                throw new Exception("Argumentos inesperados");
            switch (validationEventArgs.Action)
            {
                case ValidationErrorEventAction.Added:
                    {
                        errorCount++; break;
                    }
                case ValidationErrorEventAction.Removed:
                    {
                        errorCount--; break;
                    }
                default:
                    {
                        throw new Exception("Acción desconocida");
                    }
            }

            btnGuardar.IsEnabled = errorCount == 0;
            if(btnGenerarDocumento != null)
            {
                btnGenerarDocumento.IsEnabled = errorCount == 0;

            }


        }
        */



    }
}
