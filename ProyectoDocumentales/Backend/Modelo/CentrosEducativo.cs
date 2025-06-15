using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.MVVM.Base;

namespace ProyectoDocumentales.backend.Modelo;

[Table("centros_educativos")]
[Index("Cif", Name = "cif", IsUnique = true)]
public partial class CentrosEducativo : PropertyChangedDataError
{
    [Key]
    [Column("id_centro_educativo")]
    public int IdCentroEducativo { get; set; }

    [Column("nombre")]
    [StringLength(255)]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }

    [Column("direccion")]
    [StringLength(255)]
    [Required(ErrorMessage = "La dirección es obligatoria.")]
    public string Direccion { get; set; }

    [Column("cif")]
    [StringLength(20)]
    [Required(ErrorMessage = "El cif es obligatorio.")]
    public string Cif { get; set; }

    [Column("telefono")]
    [Required(ErrorMessage = "Teléfono requerido.")]
    [RegularExpression(@"^(\+?\d{1,4})?\d{9}$", ErrorMessage = "(9 - 15) caracteres sin prefijo o con")]
    [StringLength(20)]
    public string Telefono { get; set; }

    [Column("fax")]
    [StringLength(20)]
    public string Fax { get; set; }

    [Column("codigo_postal")]
    [StringLength(10)]
    [Required(ErrorMessage = "El código postal es obligatorio.")]
    public string CodigoPostal { get; set; }

    [Column("director")]
    [StringLength(100)]
    public string Director { get; set; }

    [InverseProperty("IdCentroEducativoNavigation")]
    public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();

    [InverseProperty("IdCentroEducativoNavigation")]
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
