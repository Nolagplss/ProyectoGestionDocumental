﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDocumentales.MVVM.Base
{
    public class PropertyChangedDataError : INotifyPropertyChanged, IDataErrorInfo
    {
        // Implementa la interfaz INotifyPropertyChanged
        // Permite tener sincronizados los valores de una propiedad con el lelemento correspondiente de la interfaz
        #region Property Changed
        /// <summary>
        /// evento que se activa al modificar una propiedad de la clase
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Manejador del evento que se activa al modificar una propiedad
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad que se modifica</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Para los errores
        protected void ValidateProperty(string propertyName)
        {
            //Fuerza el acceso al validador (el indexador lo hace internamente)
            var dummy = this[propertyName];
            //Notifica a la vista para que se actualicen posibles errores visuales
            OnPropertyChanged(propertyName);
        }

        #endregion
        // Implementa la interfaz IDataErrorInfo
        // Nos permite realizar comprobaciones de los errores que pueda tener la información introducida por el usuario
        #region DataErrorInfo

        // Mensaje del error
        public string Error { get { return null; } }

        // Comprueba los errores que pueda tener una propiedad
        public string this[string columnName]
        {
            get
            {
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                if (Validator.TryValidateProperty(
                        GetType().GetProperty(columnName).GetValue(this)
                        , new ValidationContext(this)
                        {
                            MemberName = columnName
                        }
                        , validationResults))
                    return null;

                return validationResults.First().ErrorMessage;
            }
        }

        #endregion
    }


}
