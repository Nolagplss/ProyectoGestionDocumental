using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.MVVM.Base;
using ProyectoDocumentales.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace ProyectoDocumentales.MVVM
{
    public class MvResponsables : MVBasePaginado<Responsable>
    {

        private Documentales2Context _contexto;
        private Responsable _responsableOriginal;

        private ResponsableServicio _responsableServicio;

        private bool _filtrosActivos = false;
        public bool FiltrosActivos
        {
            get => _filtrosActivos;
            set
            {
                _filtrosActivos = value;
                OnPropertyChanged(nameof(FiltrosActivos));
            }
        }

        //Guarda el Dni
        private String _txtDni;
        private Responsable _responsable;

        public Responsable responsable
        {
            get { return _responsable; }
            set
            {
                _responsable = value;
                OnPropertyChanged(nameof(responsable));
            }
        }

        private ListCollectionView _listaResponsablesLC;
        public ListCollectionView ListaResponsablesLC
        {
            get => _listaResponsablesLC;
            private set
            {
                if (_listaResponsablesLC != value)
                {
                    _listaResponsablesLC = value;
                    OnPropertyChanged(nameof(ListaResponsablesLC));
                }
            }
        }

        //Filtro txtDni
        public String txtDni
        {
            get => _txtDni;
            set
            {
                _txtDni = value;
                OnPropertyChanged(nameof(txtDni));
            }
        }

        public MvResponsables(Documentales2Context contexto)
        {
            _contexto = contexto;
            responsable = new Responsable();
        }



        public async Task Inicializa()
        {
            _responsableServicio = new ResponsableServicio(_contexto);
            servicio = _responsableServicio;
            
            //Cargamos la pagina
            await CargarPaginaAsync();
          
        }

        public override async Task CargarPaginaAsync()
        {
            var query = _contexto.Responsables.AsQueryable();

            if (FiltrosActivos)
            {
                var filtro = txtDni?.ToLower();
                if (!string.IsNullOrWhiteSpace(filtro))
                    query = query.Where(r => r.Dni.ToLower().StartsWith(filtro));
            }

            int totalRegistros = await query.CountAsync();
            TotalPaginas = (int)Math.Ceiling((double)totalRegistros / TamanioPagina);

            var responsablesPagina = await query
                .OrderBy(r => r.IdResponsable)
                .Skip((PaginaActual - 1) * TamanioPagina)
                .Take(TamanioPagina)
                .ToListAsync();

            ListaResponsablesLC = new ListCollectionView(responsablesPagina);
        }


       

        public async void Filtrar()
        {
            FiltrosActivos = true;
            PaginaActual = 1;
            await CargarPaginaAsync();
        }

      

        
        public async Task ActualizarListaResponsables()
        {
            try
            {
                await CargarPaginaAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la lista de responsables: {ex.Message}",
                              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<bool> ComprobadorCamposUnicos()
        {
            //Verificar si otro responsable tiene el mismo DNI
            var dniExistente = await _responsableServicio.FindAsync(r =>
                r.Dni == responsable.Dni && r.IdResponsable != responsable.IdResponsable);
            if (dniExistente.Any())
            {
                MessageBox.Show("El DNI ya está registrado. Introduce uno diferente.",
                              "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            //Verificar si otro responsable tiene el mismo correo
            var correoExistente = await _responsableServicio.FindAsync(r =>
                r.Correo == responsable.Correo && r.IdResponsable != responsable.IdResponsable);
            if (correoExistente.Any())
            {
                MessageBox.Show("El Correo ya le pertenece a un responsable. Introduce uno diferente.",
                              "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public async Task<bool> GuardarResponsable()
        {
            try
            {
                if (responsable != null)
                {
                    //Comprobamos campos unicos
                    if (!await ComprobadorCamposUnicos()) return false;

                    await _responsableServicio.AddAsync(responsable);

                    //Limpiar el responsable antes de guardarlo
                    responsable = new Responsable();

                    //Recargar la lista de responsables
                    await ActualizarListaResponsables();

                    return true;
                }
            }
            catch (Exception ex)
            {
                //Manejo de errores en el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al guardar responsable: {ex.Message}");
                });
            }

            return false;
        }

        public async Task<bool> ActualizarResponsable()
        {
            try
            {
                if (responsable != null)
                {
                    //Comprobamos campos unicos
                    if (!await ComprobadorCamposUnicos()) return false;

                    await _responsableServicio.UpdateAsync(responsable);

                    //Recargar la lista de responsables
                    await ActualizarListaResponsables();

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores en el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al actualizar responsable: {ex.Message}");
                });
            }

            return false;
        }


        public void CrearCopiaSeguridad()
        {
            if (responsable != null)
            {
                _responsableOriginal = new Responsable
                {
                    IdResponsable = responsable.IdResponsable,
                    Nombre = responsable.Nombre,
                    Apellidos = responsable.Apellidos,
                    Dni = responsable.Dni,
                    Correo = responsable.Correo,
                };
            }
        }

        public void RestaurarResponsable()
        {
            if (_responsableOriginal != null)
            {
                responsable.IdResponsable = _responsableOriginal.IdResponsable;
                responsable.Nombre = _responsableOriginal.Nombre;
                responsable.Apellidos = _responsableOriginal.Apellidos;
                responsable.Dni = _responsableOriginal.Dni;
                responsable.Correo = _responsableOriginal.Correo;
               

                OnPropertyChanged(nameof(responsable));
            }
        }
    }
}
