using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.MVVM.Base;

namespace ProyectoDocumentales.backend.Modelo;

[Table("responsables")]
[Index("Dni", Name = "dni", IsUnique = true)]
public partial class Responsable : PropertyChangedDataError
{
    [Key]
    [Column("id_responsable")]
    public int IdResponsable { get; set; }

    [Column("nombre")]
    [StringLength(100)]
    [Required(ErrorMessage = "Nombre requerido.")]
    public string Nombre { get; set; }

    [Column("apellidos")]
    [StringLength(100)]
    [Required(ErrorMessage = "Apellidos requeridos.")]
    public string Apellidos { get; set; }

    [Column("dni")]
    [StringLength(20)]
    [Required(ErrorMessage = "Dni requerido.")]
    public string Dni { get; set; }

    [Column("correo")]
    [StringLength(100)]
    [Required(ErrorMessage = "Correo requerido.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Formato de correo inválido.")]
    public string Correo { get; set; }

    [InverseProperty("IdResponsableNavigation")]
    public virtual Empresa Empresa { get; set; }
}
