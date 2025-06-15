using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Backend.Modelo;
using ProyectoDocumentales.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDocumentales.Servicios
{
    public class UsuarioServicio : ServicioGenerico<Usuario>
    {
       
        public Usuario usuLogin {  get; set; }

        public Documentales2Context _contexto;

        public UsuarioServicio(Documentales2Context context) : base(context)
        {
            _contexto = context;
        }

        //Metodo para obtener el usuario
        public async Task<Boolean> Login(String correo, String pass)
        {
            Boolean correcto = false;
            try
            {
                // Obtenemos el óbjeto usuario
                usuLogin = await GetUsuarioPorCorreo(correo);
            }
            catch (Exception e)
            {
                logger.Error("Login. Error al obtener el usuario" + e.InnerException);
                logger.Error(e.StackTrace);
            }
            // Coprobamos si el objeto es distinto de null y su
            // usuario y contraseña son iguales a los introducidos
            // entonces devolvemos true, en cualquier otro caso devolvemos false
            if (usuLogin != null && usuLogin.Correo.Equals(correo) && usuLogin.Contrasenia.Equals(pass))
            {
                correcto = true;
            }
            return correcto;
        }

        public async Task<Usuario> GetUsuarioPorCorreo(string correo)
        {
            IEnumerable<Usuario> usuarios;
            Usuario usu = null;
            usuarios = await FindAsync(u => u.Correo == correo);
            if (usuarios != null)
            {
                usu = usuarios.FirstOrDefault();
            }
            return usu;
        }

        public async Task<Usuario> GetUsuarioPorId(int IdUsuario)
        {
            Usuario usuario = null;
            try
            {
                //Busca directamente el usuario por su clave primaria
                usuario = await _contexto.Set<Usuario>().FirstOrDefaultAsync(u => u.IdUsuario == IdUsuario);
            }
            catch (Exception e)
            {
                logger.Error("GetUsuarioPorId. Error al obtener el usuario por ID: " + e.Message);
                logger.Error(e.StackTrace);
            }
            return usuario;
        }

        public async Task<IEnumerable<Usuario>> GetAllWithNavigationsAsync()
        {
            return await _contexto.Usuarios
             .Include(u => u.IdRolNavigation)       
             .Include(u => u.Documentos)           
             .ToListAsync();
        }
        public async Task<bool> VerificarContrasena(int usuarioId, string contrasena)
        {
            var usuario = await _contexto.Usuarios.FindAsync(usuarioId);
            return usuario != null && usuario.Contrasenia == contrasena;
        }

        public async Task<bool> VerificarEstado(string correoUsuario)
        {
            var usuario = await GetUsuarioPorCorreo(correoUsuario);
            return usuario?.FechaBaja.HasValue == true;
        }
    }
}
