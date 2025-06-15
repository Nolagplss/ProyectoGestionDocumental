using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Backend.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoDocumentales.Servicios
{
    public class CentroEducativoServicio : ServicioGenerico<CentrosEducativo>
    {
        private readonly Documentales2Context _context;


        public CentroEducativoServicio(Documentales2Context context) : base(context) 
        {
            _context = context;

        }
       
        

    }
}
