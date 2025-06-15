using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.MVVM.Base;

namespace ProyectoDocumentales.backend.Modelo;

[Table("empresas")]
[Index("Cif", Name = "cif", IsUnique = true)]
[Index("IdResponsable", Name = "id_responsable", IsUnique = true)]
public partial class Empresa : PropertyChangedDataError
{
    [Key]
    [Column("id_empresa")]
    public int IdEmpresa { get; set; }

    [Column("razon_social")]
    [StringLength(255)]
    [Required(ErrorMessage = "Razon Social requerida.")]

    public string RazonSocial { get; set; }

    [Column("cif")]
    [StringLength(20)]
    [Required(ErrorMessage = "Cif requerido.")]
    public string Cif { get; set; }

    [Column("direccion")]
    [Required(ErrorMessage = "Direccion requerida.")]
    [StringLength(255)]
    public string Direccion { get; set; }

    [Column("telefono")]
    [StringLength(20)]
    [Required(ErrorMessage = "Teléfono requerido.")]
    [RegularExpression(@"^(\+?\d{1,4})?\d{9}$", ErrorMessage = "(9 - 15) caracteres sin prefijo o con")]
    public string Telefono { get; set; }

    [Column("localidad")]
    [StringLength(100)]
    [Required(ErrorMessage = "Localidad requerida.")]
    public string Localidad { get; set; }

    [Column("provincia")]
    [StringLength(100)]
    [Required(ErrorMessage = "Provincia requerida.")]
    public string Provincia { get; set; }

    [Column("fax")]
    [StringLength(20)]
    public string Fax { get; set; }

    [Column("codigo_postal")]
    [StringLength(10)]
    [Required(ErrorMessage = "Codigo Postal requerido.")]
    public string CodigoPostal { get; set; }

    [Column("sector")]
    [StringLength(100)]
    [Required(ErrorMessage = "Sector requerido.")]
    public string Sector { get; set; }

    [Column("id_responsable")]
    public int? IdResponsable { get; set; }

    [InverseProperty("IdEmpresaNavigation")]
    [Required(ErrorMessage = "Centro de trabajo requerido.")]
    public virtual ICollection<CentrosTrabajo> CentrosTrabajos { get; set; } = new List<CentrosTrabajo>();

    [InverseProperty("IdEmpresaNavigation")]
    public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();

    [ForeignKey("IdResponsable")]
    [InverseProperty("Empresa")]
    [Required(ErrorMessage = "Responsable Requerido.")]
    public virtual Responsable IdResponsableNavigation { get; set; }
}
