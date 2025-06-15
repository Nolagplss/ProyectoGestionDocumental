using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProyectoDocumentales.backend.Modelo;

[Table("permisos")]
public partial class Permiso
{
    [Key]
    [Column("id_permiso")]
    public int IdPermiso { get; set; }

    [Column("codigo")]
    public string Codigo { get; set; } = null!;

    [Column("descripcion", TypeName = "text")]
    public string Descripcion { get; set; }

    [ForeignKey("IdPermiso")]
    [InverseProperty("IdPermisos")]
    public virtual ICollection<Role> IdRols { get; set; } = new List<Role>();
}
