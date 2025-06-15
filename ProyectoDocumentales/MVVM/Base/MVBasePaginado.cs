using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDocumentales.MVVM.Base
{
    /// <summary>
    /// Clase base para ViewModels que necesitan paginación
    /// </summary>
    public abstract class MVBasePaginado<T> : MVBaseCRUD<T> where T : class
    {

        //Propiedades de paginacion compartidas
        private int _paginaActual = 1;
        private int _tamanioPagina = 15;
        private int _totalPaginas;

        public int PaginaActual
        {
            get => _paginaActual;
            set
            {
                _paginaActual = value;
                OnPropertyChanged(nameof(PaginaActual));
            }
        }

        public int TamanioPagina
        {
            get => _tamanioPagina;
            set
            {
                _tamanioPagina = value;
                OnPropertyChanged(nameof(TamanioPagina));
            }
        }

        public int TotalPaginas
        {
            get => _totalPaginas;
            set
            {
                _totalPaginas = value;
                OnPropertyChanged(nameof(TotalPaginas));
            }
        }

        /// <summary>
        /// Método abstracto que cada clase hija debe implementar para cargar su página específica
        /// </summary>
        public abstract Task CargarPaginaAsync();

        /// <summary>
        /// Método común para ir a la página anterior
        /// </summary>
        public virtual async Task PaginaAnteriorAsync()
        {
            if (PaginaActual > 1)
            {
                PaginaActual--;
                await CargarPaginaAsync();
            }
        }

        /// <summary>
        /// Método común para ir a la página siguiente
        /// </summary>
        public virtual async Task PaginaSiguienteAsync()
        {
            if (PaginaActual < TotalPaginas)
            {
                PaginaActual++;
                await CargarPaginaAsync();
            }
        }

        /// <summary>
        /// Método común para ir a una página específica
        /// </summary>
        public virtual async Task IrAPaginaAsync(int numeroPagina)
        {
            if (numeroPagina >= 1 && numeroPagina <= TotalPaginas)
            {
                PaginaActual = numeroPagina;
                await CargarPaginaAsync();
            }
        }

        /// <summary>
        /// Método común para resetear a la primera página (útil al filtrar)
        /// </summary>
        protected virtual async Task ResetearPaginaAsync()
        {
            PaginaActual = 1;
            await CargarPaginaAsync();
        }

    }
}
