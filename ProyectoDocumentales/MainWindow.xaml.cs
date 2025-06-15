using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Backend.Modelo;
using ProyectoDocumentales.Frontend.ControlUsuario;
using ProyectoDocumentales.MVVM;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProyectoDocumentales
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Documentales2Context _contexto;

        private MvDocumentos mvDocumentos;

        private Usuario _usuario;

        //Le pasamos el contexto y el usuario
        public MainWindow(Documentales2Context contexto, Usuario usuario)
        {

            InitializeComponent();
            _contexto = contexto;
            _usuario = usuario;
            _ = Inicializa();
        }

     

        private async Task Inicializa()
        {
       
            mvDocumentos = new MvDocumentos(_contexto);
            await mvDocumentos.Inicializa();

        }

        
        private void aparecerDocumentos_Click(object sender, RoutedEventArgs e)
        {

            UCDocumentos uc = new UCDocumentos(mvDocumentos, _usuario, _contexto);
            panelPrincipal.Children.Clear();
            panelPrincipal.Children.Add(uc);
        }
    }
}