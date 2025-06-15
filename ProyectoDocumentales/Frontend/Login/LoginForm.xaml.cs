using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.EntityFrameworkCore;
using NLog;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Backend.Modelo;
using ProyectoDocumentales.Frontend.ControlUsuario;
using ProyectoDocumentales.MVVM;
using ProyectoDocumentales.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProyectoDocumentales.Frontend.Login
{
    /// <summary>
    /// Lógica de interacción para LoginForm.xaml
    /// </summary>
    public partial class LoginForm : MetroWindow
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private Usuario _usuario;

        private UsuarioServicio _usuarioServicio;


        private Documentales2Context _contexto;


        public LoginForm()
        {

            if (ConectarBD())
            {
                InitializeComponent();
                _contexto = new Documentales2Context();
                _usuario = new Usuario();
                _usuarioServicio = new UsuarioServicio(_contexto);
            }


        }


        // Conexion a la base de datos
        private bool ConectarBD()
        {
            bool correcto = true;
            _contexto = new Documentales2Context();

            try
            {
                _contexto.Database.OpenConnection();
              


            }
            catch (Exception ex)
            {
           
                correcto = false;
                MessageBox.Show("Conexion de la base de datos", "Ups!!!" + ex.Message);
            }
            return correcto;
        }


        public async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            logger.Info("Intento de login para el usuario: " + txtCorreoUsuario.Text);

            //Verificamos si el usuario existe pasandole el usuario que escribio
            if (await _usuarioServicio.Login(txtCorreoUsuario.Text, txtContraseniaUsuario.Password))
            {

                logger.Info($"Login exitoso para usuario {txtCorreoUsuario.Text}");


                if (await _usuarioServicio.VerificarEstado(txtCorreoUsuario.Text))
                {
                    await this.ShowMessageAsync("Inicio de sesion", "El usuario se encuentra inactivo, contacte al administrador del sistema para mas detalles.");
                    logger.Warn($"Usuario {txtCorreoUsuario.Text} inactivo intentó iniciar sesión");

                    return;
                }

                RoleManager.UsuarioActual = _usuarioServicio.usuLogin;

                //Cargamos los permisos del usuario
                await CargarPermisosUsuario(_usuarioServicio.usuLogin.IdUsuario);

                Home home = new Home(_contexto, _usuarioServicio.usuLogin);
                home.Show();
                this.Close();
            }
            else
            {
                await this.ShowMessageAsync("Inicio de sesion", "El usuario o contraseña no son correctos");
                logger.Warn($"Login fallido para usuario {txtCorreoUsuario.Text}");

            }



        }

        private async Task CargarPermisosUsuario(int usuarioId)
        {
            try
            {
                logger.Info($"Cargando permisos para usuario ID: {usuarioId}");

                //Obtener el usuario con su rol y permisos
                var usuario = await _contexto.Usuarios
                   .Include(u => u.IdRolNavigation)
                       .ThenInclude(r => r.IdPermisos)
                   .FirstOrDefaultAsync(u => u.IdUsuario == usuarioId);

                if (usuario?.IdRolNavigation?.IdPermisos != null)
                {
                    //Obtener los codigos de permisos del rol del usuario
                    var permisos = usuario.IdRolNavigation.IdPermisos
                        .Select(p => p.Codigo)
                        .ToList();

                    //Asignar los permisos al RoleManager
                    RoleManager.Roles = permisos;
                    logger.Info($"Permisos cargados correctamente para usuario ID: {usuarioId}");



                }
                else
                {
                    RoleManager.Roles = new List<string>();
                    logger.Warn($"Usuario {usuarioId} no tiene rol asignado o el rol no tiene permisos");

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al cargar permisos del usuario.");
                //Manejar error al cargar permisos
                MessageBox.Show($"Error al cargar permisos: {ex.Message}");
                RoleManager.Roles = new List<string>(); //Lista vacia por seguridad
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Si la ventana esta maximizada, se restaura a estado normal
                if (WindowState == WindowState.Maximized)
                {
                    // Obtenemos posicion del raton
                    Point mousePosition = e.GetPosition(this);

                    // Cambiar a estado normal
                    WindowState = WindowState.Normal;

                    // Ajustar la posicion para que el cursor quede sobre la barra de titulo
                    // Tambien evitamos que hayan saltos bruscos
                    double adjustedLeft = mousePosition.X - (this.Width * mousePosition.X / SystemParameters.WorkArea.Width);
                    this.Left = Mouse.GetPosition(null).X - adjustedLeft;
                    this.Top = Mouse.GetPosition(null).Y - mousePosition.Y;
                }

                // Con esto movemos la ventana
                this.DragMove();
            }
        }

        private void btnMinimizarApp_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

        }

        private void btnMaximizarMinimizar_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void btnCerrarApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();

        }
    }
}

