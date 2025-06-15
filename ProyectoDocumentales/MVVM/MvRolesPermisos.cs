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

namespace ProyectoDocumentales.MVVM
{
    public class MvRolesPermisos : MVBaseCRUD<Role>
    {
        private Documentales2Context _contexto;
        private RoleServicio _roleServicio;
        private PermisoServicio _permisoServicio;

        private Role _rolSeleccionado;
        public Role RolSeleccionado
        {
            get => _rolSeleccionado;
            set
            {
                _rolSeleccionado = value;
                OnPropertyChanged(nameof(RolSeleccionado));
            }
        }

        private ObservableCollection<Role> _lista;
        public ObservableCollection<Role> Lista
        {
            get => _lista;
            set
            {
                _lista = value;
                OnPropertyChanged(nameof(Lista));
            }
        }
        private List<Permiso> _todosLosPermisos;
        public List<Permiso> TodosLosPermisos
        {
            get => _todosLosPermisos;
            set
            {
                _todosLosPermisos = value;
                OnPropertyChanged(nameof(TodosLosPermisos));
            }
        }

        public MvRolesPermisos(Documentales2Context contexto)
        {
            _contexto = contexto;
            _roleServicio = new RoleServicio(contexto);
            _permisoServicio = new PermisoServicio(contexto);

            Lista = new ObservableCollection<Role>();

           
        }

        public async Task Inicializa()
        {
            await CargarRoles();
            await CargarPermisos();
        }

        public async Task CargarRoles()
        {
            try
            {
                var roles = await _contexto.Roles
                    .Include(r => r.IdPermisos)
                    .Where(r => !string.IsNullOrEmpty(r.NombreRol))
                    .ToListAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Lista.Clear();
                    foreach (var rol in roles)
                    {
                        Lista.Add(rol);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al cargar roles: {ex.Message}");
                });
            }
        }

        public async Task CargarPermisos()
        {
            try
            {
                var permisos = await _contexto.Permisos.ToListAsync();

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    TodosLosPermisos = permisos;
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Error al cargar permisos: {ex.Message}");
                });
            }
        }

        //Metodo para agregar un nuevo rol
        public async Task<bool> Add(Role rol)
        {
            try
            {
                await _roleServicio.AddAsync(rol);
                await CargarRoles();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar rol: {ex.Message}");
                return false;
            }
        }

        //Metodo para actualizar un rol existente
        public async Task<bool> Update(Role rol)
        {
            try
            {
                await _roleServicio.UpdateAsync(rol);
                await CargarRoles();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar rol: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar un rol
        public async Task<bool> Delete(Role rol)
        {
            try
            {
                await _roleServicio.DeleteAsync(rol);
                await CargarRoles();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al eliminar rol: {ex.Message}");
                return false;
            }
        }

        //Metodo para guardar cambios
        public async Task<bool> Save()
        {
            try
            {
                await _contexto.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar cambios: {ex.Message}");
                return false;
            }
        }

        //Metodo para agregar un permiso a un rol
        public async Task<bool> AgregarPermisoARole(int idRol, int idPermiso)
        {
            try
            {
                var rol = await _contexto.Roles
                    .Include(r => r.IdPermisos)
                    .FirstOrDefaultAsync(r => r.IdRol == idRol);

                var permiso = await _contexto.Permisos
                    .FirstOrDefaultAsync(p => p.IdPermiso == idPermiso);

                if (rol != null && permiso != null)
                {
                    if (!rol.IdPermisos.Contains(permiso))
                    {
                        rol.IdPermisos.Add(permiso);
                        await _contexto.SaveChangesAsync();
                        await CargarRoles();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar permiso al rol: {ex.Message}");
                return false;
            }
        }

        //Metodo para quitar un permiso de un rol
        public async Task<bool> QuitarPermisoDeRole(int idRol, int idPermiso)
        {
            try
            {
                var rol = await _contexto.Roles
                    .Include(r => r.IdPermisos)
                    .FirstOrDefaultAsync(r => r.IdRol == idRol);

                var permiso = rol?.IdPermisos.FirstOrDefault(p => p.IdPermiso == idPermiso);

                if (rol != null && permiso != null)
                {
                    rol.IdPermisos.Remove(permiso);
                    await _contexto.SaveChangesAsync();
                    await CargarRoles();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al quitar permiso del rol: {ex.Message}");
                return false;
            }
        }

    }
}
