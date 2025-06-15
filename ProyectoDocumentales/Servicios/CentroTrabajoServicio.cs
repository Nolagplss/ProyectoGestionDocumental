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
    public class CentroTrabajoServicio : ServicioGenerico<CentrosTrabajo>
    {

        private Documentales2Context _context;
        public CentroTrabajoServicio(Documentales2Context context) : base(context)
        {
            _context = context;
        }

        public async Task<List<CentrosTrabajo>> GetCentrosLibresAsync()
        {
            return await _context.CentrosTrabajos
                .Where(c => c.IdEmpresa == null)
                .OrderBy(c => c.Direccion)
                .ToListAsync();
        }

        public async Task<bool> AsignarCentroAEmpresaAsync(int idCentro, int idEmpresa)
        {
            try
            {
                var centro = await _context.CentrosTrabajos.FindAsync(idCentro);
                if (centro != null && centro.IdEmpresa == null)
                {
                    centro.IdEmpresa = idEmpresa;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
            
        public async Task<bool> LiberarCentroAsync(int idCentro)
        {
            try
            {
                var centro = await _context.CentrosTrabajos.FindAsync(idCentro);
                if (centro != null)
                {
                    centro.IdEmpresa = null;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
