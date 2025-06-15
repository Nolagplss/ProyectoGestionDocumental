    using ProyectoDocumentales.backend.Modelo;
    using ProyectoDocumentales.Backend.Modelo;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace ProyectoDocumentales.Servicios
    {
        public class RoleServicio : ServicioGenerico<Role>
        {

            Documentales2Context _context;
            public RoleServicio(Documentales2Context context) : base(context)
            {
                _context = context;

            }
        }
    }
