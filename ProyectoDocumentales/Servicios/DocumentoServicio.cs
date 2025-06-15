using ProyectoDocumentales.backend.Modelo;
using ProyectoDocumentales.Backend.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProyectoDocumentales.Servicios
{
    public class DocumentoServicio : ServicioGenerico<Documento>
    {

        private Documentales2Context _contexto;

        public DocumentoServicio(Documentales2Context contexto) : base(contexto) 
        {
            _contexto = contexto;
        }
        public async Task<IEnumerable<Documento>> GetAllWithNavigationsAsync()
        {
            return await _contexto.Documentos
                .Include(d => d.IdEmpresaNavigation)
                    .ThenInclude(e => e.IdResponsableNavigation)
                .Include(d => d.IdCentroEducativoNavigation)
                .Include(d => d.IdUsuarioNavigation)
                .Include(d => d.IdEmpresaNavigation.CentrosTrabajos)
                .ToListAsync();
        }

    }
}
