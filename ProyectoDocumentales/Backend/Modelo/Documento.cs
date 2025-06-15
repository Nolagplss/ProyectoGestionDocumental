using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.MVVM.Base;

namespace ProyectoDocumentales.backend.Modelo;

[Table("documentos")]
[Index("IdCentroEducativo", Name = "id_centro_educativo")]
[Index("IdEmpresa", Name = "id_empresa")]
[Index("IdUsuario", Name = "id_usuario")]
[Index("NumeroConcierto", Name = "numero_concierto", IsUnique = true)]
public partial class Documento : PropertyChangedDataError
{
    [Key]
    [Column("id_documento")]
    public int IdDocumento { get; set; }

    [Column("numero_concierto")]
    [StringLength(50)]
    [Required(ErrorMessage = "Numero de concierto requerido.")]
    public string NumeroConcierto { get; set; }

    [Column("fecha_firma", TypeName = "date")]
    public DateTime? FechaFirma { get; set; }

    [Column("id_centro_educativo")]
    public int? IdCentroEducativo { get; set; }

    [Column("id_empresa")]
    public int? IdEmpresa { get; set; }

    [Column("id_usuario")]
    public int? IdUsuario { get; set; }

    [Column("ruta")]
    public string Ruta { get; set; }

    [ForeignKey("IdCentroEducativo")]
    [InverseProperty("Documentos")]
    [Required(ErrorMessage = "Centro educativo requerido.")]
    public virtual CentrosEducativo IdCentroEducativoNavigation { get; set; }

    [ForeignKey("IdEmpresa")]
    [InverseProperty("Documentos")]
    [Required(ErrorMessage = "Empresa requerida requerido.")]
    public virtual Empresa IdEmpresaNavigation { get; set; }

    [ForeignKey("IdUsuario")]
    [InverseProperty("Documentos")]
    public virtual Usuario IdUsuarioNavigation { get; set; }
}
