using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.MVVM.Base;

namespace ProyectoDocumentales.backend.Modelo;

[Table("centros_trabajo")]
[Index("IdEmpresa", Name = "id_empresa")]
public partial class CentrosTrabajo : PropertyChangedDataError
{
    [Key]
    [Column("id_centro_trabajo")]
    public int IdCentroTrabajo { get; set; }

    [Column("direccion")]
    [StringLength(255)]
    [Required(ErrorMessage = "Dirección requerida.")]
    public string Direccion { get; set; }

    [Column("telefono")]
    [Required(ErrorMessage = "Teléfono requerido.")]
    [RegularExpression(@"^(\+?\d{1,4})?\d{9}$", ErrorMessage = "(9 - 15) caracteres sin prefijo o con")]
    [StringLength(15)]
    public string Telefono { get; set; }

    [Column("id_empresa")]
    public int? IdEmpresa { get; set; }

    [ForeignKey("IdEmpresa")]
    [InverseProperty("CentrosTrabajos")]
    public virtual Empresa IdEmpresaNavigation { get; set; }
}
