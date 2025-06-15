using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.MVVM.Base;

namespace ProyectoDocumentales.backend.Modelo;

[Table("usuarios")]
[Index("Correo", Name = "correo", IsUnique = true)]
[Index("Dni", Name = "dni", IsUnique = true)]
[Index("IdRol", Name = "id_rol")]
[Index("IdCentroEducativo", Name = "idx_usuario_centro")]
public partial class Usuario : PropertyChangedDataError
{
    [Key]
    [Column("id_usuario")]
    public int IdUsuario { get; set; }

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

    [Column("contrasenia")]
    [StringLength(255)]
    [Required(ErrorMessage = "Contraseña requerida.")]
    public string Contrasenia { get; set; }

    [Column("fecha_alta", TypeName = "date")]
    public DateTime? FechaAlta { get; set; }

    [Column("fecha_baja", TypeName = "date")]
    public DateTime? FechaBaja { get; set; }

    [Column("observaciones", TypeName = "text")]
    public string Observaciones { get; set; }

    [Column("id_rol")]
    [Required(ErrorMessage = "Rol requerido.")]
    public int? IdRol { get; set; }

    [Column("id_centro_educativo")]
    public int? IdCentroEducativo { get; set; }

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();

    [ForeignKey("IdRol")]
    [InverseProperty("Usuarios")]
    public virtual Role IdRolNavigation { get; set; }

    [ForeignKey("IdCentroEducativo")]
    [InverseProperty("Usuarios")]
    public virtual CentrosEducativo IdCentroEducativoNavigation { get; set; }
}
