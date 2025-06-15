using Microsoft.VisualBasic.ApplicationServices;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.MVVM.Base;
using ProyectoDocumentales.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoDocumentales.MVVM
{
    public class MvUsuario : MVBaseCRUD<Usuario>
    {

        //Contexto
        private Documentales2Context _contexto;

        //Usuario Servicio
        private UsuarioServicio usuarioServicio;

        //Usuario
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

        public MvUsuario(Documentales2Context contexto, Usuario usuario)
        {
            _contexto = contexto;
           
            _usuario = usuario;

           
        }

        //Inicializador
        public async Task Inicializa()
        {
            usuarioServicio = new UsuarioServicio(_contexto);

            //Asignamos al servicio el usuarioServicio
            servicio = usuarioServicio;


        }

        //Cambiar contraseña con validacion
        public async Task<bool> CambiarContrasena(string contrasenaActual, string nuevaContrasena)
        {
            try
            {
                //Verificamos que la contrasñea que introdujo es correcta
                bool contrasenaValida = await usuarioServicio.VerificarContrasena(usuario.IdUsuario, contrasenaActual);

                if (!contrasenaValida)
                {
                    return false; // Contraseña actual incorrecta
                }

                //Actualizamos contraseña del usuario
                usuario.Contrasenia = nuevaContrasena;

                //Guardamos en la bd
                return await Update(usuario);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar la contraseña: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }


        public async Task<bool> Actualiza()
        {
            try
            {
                return await Update(usuario);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar el usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

        }

    }
}
