﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDocumentales.Servicios
{
    /// <summary>
	/// Interfaz que nos muestra las principales operaciones 
	/// a realizar mediante objetos con la base de datos
	/// </summary>
	/// <typeparam name="T">Clase genérica</typeparam>
    public interface IServicioGenerico<T> where T : class
    {
        /// <summary>
		/// Inserta un objeto a la BD de forma asíncrona
		/// </summary>
		/// <param name="entity">Entidad que insertamos en la BD</param>
		/// <returns>Devuelve una tarea para implementar la asincronía</returns>
		Task<bool> AddAsync(T entity);
        /// <summary>
        /// Devuelve una lista con todos los elementos de una tabla de forma asíncrona
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
