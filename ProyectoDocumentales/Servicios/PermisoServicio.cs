using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Backend.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDocumentales.Servicios
{
    public class PermisoServicio : ServicioGenerico<Permiso>
    {
   

        public PermisoServicio(Documentales2Context context) : base(context)
        {
            

        }

    }
}
