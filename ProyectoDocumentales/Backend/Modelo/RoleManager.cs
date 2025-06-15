using ProyectoDocumentales.backend.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoDocumentales.Backend.Modelo
{
    public static class RoleManager
    {

        public static Usuario UsuarioActual { get; set; }

        public static List<string> Roles { get; set; } = new();

        public static bool HasPermission(string code) => Roles.Contains(code);
    }
}
