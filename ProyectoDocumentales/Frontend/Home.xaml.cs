using ProyectoDocumentales.Frontend.ControlUsuario;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.MVVM;
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
using MahApps.Metro.Controls;
using ProyectoDocumentales.Frontend.Formularios;
using ProyectoDocumentales.Frontend.Charts;
using ProyectoDocumentales.Frontend.Informes;
using ProyectoDocumentales.Backend.Modelo;
using ProyectoDocumentales.Frontend.Login;
using NLog;
namespace ProyectoDocumentales.Frontend
{

    public partial class Home : MetroWindow
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private bool menuExpandido = false;

        private Documentales2Context _contexto;

        private MvDocumentos mvDocumentos;

        private MvUsuario mvUsuario;

        private MvUsuarios mvUsuarios;
        
        private Usuario _usuario;


        private MvCentrosEducativo _mvCentros;
      

        public Home(Documentales2Context contexto, Usuario usuario)
        {

            InitializeComponent();

            logger.Info("Inicializando ventana Home.");

            _contexto = contexto;
            _usuario = usuario;
            _mvCentros = new MvCentrosEducativo(_contexto, usuario);
            this.DataContext = _mvCentros;

            _ = Inicializa();

          

            // CargarUcHome();  

        }

       
      
        private async Task Inicializa()
        {
            logger.Info("Inicializando modelos y configurando menú.");

            mvDocumentos = new MvDocumentos(_contexto);
            await mvDocumentos.Inicializa();

            
            mvUsuario = new MvUsuario(_contexto,_usuario);
            await mvUsuario.Inicializa();

            await _mvCentros.Inicializar();

            ConfigurarVisibilidadMenu();

            //Selecciona el item de inicio
            var itemInicio = hamMenuPrincipal.Items
                .OfType<HamburgerMenuImageItem>()
                .FirstOrDefault(item => item.Label == "Inicio");

            if (itemInicio != null)
            {
                hamMenuPrincipal.SelectedItem = itemInicio;
                ProcesarMenuItem(itemInicio);
            }


        }

        private void ConfigurarVisibilidadMenu()
        {
            logger.Info("Configurando visibilidad del menú.");
            try
            {
                //Crear nueva coleccion de elementos del menu
                var menuItems = new HamburgerMenuItemCollection();

                //Inicio siempre visible
                menuItems.Add(new HamburgerMenuImageItem
                {
                    Label = "Inicio",
                    ToolTip = "Volver al inicio",
                    Thumbnail = new BitmapImage(new Uri("/Recursos/Iconos/HomeWhite.png", UriKind.Relative))
                });


                //Usuarios
                if (RoleManager.HasPermission("USER_MANAGE"))
                {
                    menuItems.Add(new HamburgerMenuImageItem
                    {
                        Label = "Usuarios",
                        ToolTip = "Gestión de usuarios",
                        Thumbnail = new BitmapImage(new Uri("/Recursos/Iconos/Usuarios_White.png", UriKind.Relative))
                    });
                }

                //Responsables
                if (RoleManager.HasPermission("RESPONSABLE_MANAGE"))
                {
                    menuItems.Add(new HamburgerMenuImageItem
                    {
                        Label = "Responsables",
                        ToolTip = "Gestión de responsables",
                        Thumbnail = new BitmapImage(new Uri("/Recursos/Iconos/ResponsableWhite.png", UriKind.Relative))
                    });
                }

                //Informes
                if (RoleManager.HasPermission("REPORTS_CREATE"))
                {
                    menuItems.Add(new HamburgerMenuImageItem
                    {
                        Label = "Informes",
                        ToolTip = "Crear informes",
                        Thumbnail = new BitmapImage(new Uri("/Recursos/Iconos/informeWhite.png", UriKind.Relative))
                    });
                }

                //Graficos
                if (RoleManager.HasPermission("CHARTS_VIEW"))
                {
                    menuItems.Add(new HamburgerMenuImageItem
                    {
                        Label = "Gráficos",
                        ToolTip = "Visualizar gráficos",
                        Thumbnail = new BitmapImage(new Uri("/Recursos/Iconos/GraficoWhite.png", UriKind.Relative))
                    });
                }

                //Roles y permisos
                if (RoleManager.HasPermission("ROLES_MANAGE"))
                {
                    menuItems.Add(new HamburgerMenuImageItem
                    {
                        Label = "Roles",
                        ToolTip = "Roles y Permisos",
                        Thumbnail = new BitmapImage(new Uri("/Recursos/Iconos/RolesPermisosWhite.png", UriKind.Relative))
                    });
                }



                // Asignar la colección filtrada al menú
                hamMenuPrincipal.ItemsSource = menuItems;
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Error al configurar visibilidad del menú.");
                MessageBox.Show($"Error al configurar el menú: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           

           





        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCerrarApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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


        private async void ProcesarMenuItem(HamburgerMenuImageItem item)
        {
            logger.Info($"Procesando item de menú seleccionado");

            if (item == null) return;

            ContenidoBase.Children.Clear();

            switch (item.Label)
            {
                case "Inicio":
                    CargarUcHome();
                    break;
                

                case "Usuarios":
                    
                    var mvUsuarios = new MvUsuarios(_contexto);
                    await mvUsuarios.Inicializa();

                    UCUsuarios ucUsuarios = new UCUsuarios(mvUsuarios);
                    ContenidoBase.Children.Add(ucUsuarios);
                    break;

                case "Responsables":

                    var mvResponsables = new MvResponsables(_contexto);
                    await mvResponsables.Inicializa();

                    UCResponsables ucResponsables = new UCResponsables(mvResponsables);
                    ContenidoBase.Children.Add(ucResponsables);
                    break;

                case "Informes":

                    var mvInformes = new MvInformes(_contexto);
                    await mvInformes.Inicializar();

                    //Creamos el UserControl de gráficos y lo añadimos al contenido base
                    UCInformes ucInformes = new UCInformes(mvInformes);
                    ContenidoBase.Children.Add(ucInformes);
                    break;

                case "Gráficos":

                    var mvGraficos = new MvGraficos(_contexto);
                    await mvGraficos.Inicializa();

                    //Creamos el UserControl de gráficos y lo añadimos al contenido base
                    UCCharts ucCharts = new UCCharts(mvGraficos);
                    ContenidoBase.Children.Add(ucCharts);
                    break;

                case "Roles":

                    var mvRolesPermisos = new MvRolesPermisos(_contexto);
                    await mvRolesPermisos.Inicializa();

                    //Creamos el user control

                    RolesForm rolesForm = new RolesForm(mvRolesPermisos);
                    ContenidoBase.Children.Add(rolesForm);


                    break;



                default:
                    logger.Error("Error al procesar el item del menu");

                    //No hay
                    MessageBox.Show($"No se ha implementado la funcionalidad para el item: {item.Label}", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
        }


        private void hamMenuPrincipal_ItemClick(object sender, MahApps.Metro.Controls.ItemClickEventArgs e)
        {
           // var clickedItem = e.ClickedItem as HamburgerMenuImageItem;

            if (e.ClickedItem is HamburgerMenuImageItem item)
            {
                ProcesarMenuItem(item);
            }
        }

        private void CargarUcHome()
        {

            UCDocumentos uc = new UCDocumentos(mvDocumentos, _usuario, _contexto);
            ContenidoBase.Children.Clear();
            ContenidoBase.Children.Add(uc);
        }


        private void hamMenuPrincipal_OptionsItemClick(object sender, MahApps.Metro.Controls.ItemClickEventArgs args)
        {
            var clickedItem = args.ClickedItem as HamburgerMenuIconItem;
            if (clickedItem != null)
            {
                string selectedLabel = clickedItem.Label;


                //Si selecciona ayuda le envias un correo a samrad@alu.edu.gva.es
                if (selectedLabel == "Ayuda")
                {
                    try
                    {
                        //Crear una direccion de correo con el protocolo mailto:
                        var mailtoUri = new Uri("mailto:samrad3@alu.edu.gva.es?subject=Solicitud de Ayuda para DocuMe");

                        // brir la aplicacion de correo predeterminada
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = mailtoUri.AbsoluteUri,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"No se pudo abrir la aplicación de correo: {ex.Message}", "Error");
                    }
                }
                else if (selectedLabel == "Acerca de")
                {
                    // Codigo para mostrar la informacion Acerca de
                    MessageBox.Show("Programa de gestión de documentos para empresas y centros educativos desarrollado por Samuel Radu Dragomir.", "About");
                }
            }
        }

        private void btnClic_Click(object sender, RoutedEventArgs e)
        {
            UCDocumentos uc = new UCDocumentos(mvDocumentos, _usuario, _contexto);
            ContenidoBase.Children.Clear();
            ContenidoBase.Children.Add(uc);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void btnUser_Click(object sender, RoutedEventArgs e)
        {
            UserForm userForm = new UserForm(mvUsuario);
            userForm.ShowDialog();
        }

        private void btnCentroEducativo_Click(object sender, RoutedEventArgs e)
        {

            if(!RoleManager.HasPermission("CENTER_DATA"))
            {
                logger.Warn("Intento de acceso a Centro Educativo sin permiso.");

                return;
            }
            CentroEducativoForm centroEducativoForm = new CentroEducativoForm(_mvCentros);

            centroEducativoForm.ShowDialog();

        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}
