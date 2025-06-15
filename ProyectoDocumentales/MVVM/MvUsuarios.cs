using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.MVVM.Base;
using ProyectoDocumentales.Servicios;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace ProyectoDocumentales.MVVM
{
    public class MvUsuarios : MVBasePaginado<Usuario>
    {

        private Documentales2Context _contexto;

        private Usuario _usuarioOriginal;

        private UsuarioServicio _usuarioServicio;
        private RoleServicio _roleServicio;


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

        // Propiedades existentes
        private String _txtDni;
        private Usuario _usuario;

        public Usuario usuario
        {
            get { return _usuario; }
            set
            {
                _usuario = value;
                OnPropertyChanged(nameof(usuario));
            }
        }

        private ListCollectionView _listaUsuariosLC;
        public ListCollectionView ListaUsuariosLC
        {
            get => _listaUsuariosLC;
            private set
            {
                if (_listaUsuariosLC != value)
                {
                    _listaUsuariosLC = value;
                    OnPropertyChanged(nameof(ListaUsuariosLC));
                }
            }
        }

        //Lista roles para combobox
        private IEnumerable<Role> _listaRole;
        public IEnumerable<Role> ListaRoles
        {
            get { return _listaRole; }
            set
            {
                _listaRole = value;
                OnPropertyChanged(nameof(ListaRoles));
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

        //Rol Seleccionado
        private Role _roleSeleccionado;
        public Role RoleSeleccionado
        {
            get => _roleSeleccionado;
            set
            {
                _roleSeleccionado = value;
                OnPropertyChanged(nameof(RoleSeleccionado));
            }
        }

        public MvUsuarios(Documentales2Context contexto)
        {
            _contexto = contexto;
            usuario = new Usuario();
        }

        public async Task Inicializa()
        {
            _usuarioServicio = new UsuarioServicio(_contexto);
            _roleServicio = new RoleServicio(_contexto);
            servicio = _usuarioServicio;

            // Cargar roles
            ListaRoles = await _roleServicio.GetAllAsync();

            //Cargar primera pagina
            await CargarPaginaAsync();
        }

        public override async Task CargarPaginaAsync()
        {
            var query = _contexto.Usuarios
               .Include(u => u.IdRolNavigation)
               .AsQueryable();

            if (FiltrosActivos)
            {
                var filtroDni = txtDni?.ToLower();
                var filtroRol = RoleSeleccionado?.IdRol;

                if (!string.IsNullOrWhiteSpace(filtroDni))
                    query = query.Where(u => u.Dni.ToLower().StartsWith(filtroDni));

                if (filtroRol.HasValue)
                    query = query.Where(u => u.IdRol == filtroRol.Value);
            }

            int totalRegistros = await query.CountAsync();
            TotalPaginas = (int)Math.Ceiling((double)totalRegistros / TamanioPagina);

            var usuariosPagina = await query
                .OrderBy(u => u.IdUsuario)
                .Skip((PaginaActual - 1) * TamanioPagina)
                .Take(TamanioPagina)
                .ToListAsync();

            ListaUsuariosLC = new ListCollectionView(usuariosPagina);
        }

        public async void Filtrar()
        {
            FiltrosActivos = true;
            PaginaActual = 1;
            await CargarPaginaAsync();
        }

        public async Task ActualizarListaUsuarios()
        {
            try
            {

                await CargarPaginaAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la lista de usuarios: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<bool> ComprobadorCamposUnicos()
        {
            //Verifica si otro usuario tiene el mismo DNI
            var dniExistente = await _usuarioServicio.FindAsync(u => u.Dni == usuario.Dni && u.IdUsuario != usuario.IdUsuario);
            if (dniExistente.Any())
            {
                MessageBox.Show("El DNI ya está registrado. Introduce uno diferente.", "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            //Verifica si otro usuario tiene el mismo correo
            var correoExistente = await _usuarioServicio.FindAsync(u => u.Correo == usuario.Correo && u.IdUsuario != usuario.IdUsuario);
            if (correoExistente.Any())
            {
                MessageBox.Show("El Correo ya le pertenece a un usuario. Introduce uno diferente.", "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public async Task<bool> GuardarUsuario()
        {
            try
            {
                if (usuario != null)
                {
                    //Comprobamos campos unicos
                    if (!await ComprobadorCamposUnicos()) return false;

                    //Asignamos la fecha actual de alta
                    usuario.FechaAlta = DateTime.Now;
                    await _usuarioServicio.AddAsync(usuario);

                    //Limpiar el usuario antes de guardarlo
                    usuario = new Usuario();

                    //Recargar la lista de usuarios
                    await ActualizarListaUsuarios();
                    return true;
                }
            }
            catch (Exception ex)
            {
                //Manejo de errores en el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al guardar usuario: {ex.Message}");
                });
            }
            return false;
        }

        public async Task<bool> ActualizarUsuario()
        {
            try
            {
                if (usuario != null)
                {
                    //Comprobamos campos unicos
                    if (!await ComprobadorCamposUnicos()) return false;

                    await _usuarioServicio.UpdateAsync(usuario);

                    //Recargar la lista de usuarios
                    await ActualizarListaUsuarios();
                    return true;
                }
            }
            catch (Exception ex)
            {
                //Manejo de errores en el hilo principal
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al actualizar usuario: {ex.Message}");
                });
            }
            return false;
        }

        public void CrearCopiaSeguridad()
        {
            if (usuario != null)
            {
                _usuarioOriginal = new Usuario
                {
                    IdUsuario = usuario.IdUsuario,
                    Nombre = usuario.Nombre,
                    Apellidos = usuario.Apellidos,
                    Dni = usuario.Dni,
                    Correo = usuario.Correo,
                    Contrasenia = usuario.Contrasenia,
                    IdRol = usuario.IdRol,
                    IdRolNavigation = usuario.IdRolNavigation,
                    FechaAlta = usuario.FechaAlta,
                    Observaciones = usuario.Observaciones
                };
            }
        }

        public void RestaurarUsuario()
        {
            if (_usuarioOriginal != null)
            {
                usuario.IdUsuario = _usuarioOriginal.IdUsuario;
                usuario.Nombre = _usuarioOriginal.Nombre;
                usuario.Apellidos = _usuarioOriginal.Apellidos;
                usuario.Dni = _usuarioOriginal.Dni;
                usuario.Correo = _usuarioOriginal.Correo;
                usuario.Contrasenia = _usuarioOriginal.Contrasenia;
                usuario.IdRol = _usuarioOriginal.IdRol;
                usuario.IdRolNavigation = _usuarioOriginal.IdRolNavigation;
                usuario.FechaAlta = _usuarioOriginal.FechaAlta;
                usuario.Observaciones = _usuarioOriginal.Observaciones;


                if (_usuarioOriginal.IdRolNavigation != null)
                {
                    RoleSeleccionado = _usuarioOriginal.IdRolNavigation;
                }


                OnPropertyChanged(nameof(usuario));
            }
        }
    }
}
