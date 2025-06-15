using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Backend.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDocumentales.Servicios
{
    public class ResponsableServicio : ServicioGenerico<Responsable>
    {

        Documentales2Context _context;
        public ResponsableServicio(Documentales2Context context) : base(context)
        {

            _context = context;

        }

        public async Task<List<Responsable>> GetResponsablesLibresAsync()
        {
            return await _context.Responsables
                .Where(c => c.Empresa == null)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }
    }
}
