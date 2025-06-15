using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProyectoDocumentales.Backend.Modelo;

namespace ProyectoDocumentales.backend.Modelo;

public partial class Documentales2Context : DbContext
{
    public Documentales2Context()
    {
    }

    public Documentales2Context(DbContextOptions<Documentales2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<CentrosEducativo> CentrosEducativos { get; set; }

    public virtual DbSet<CentrosTrabajo> CentrosTrabajos { get; set; }

    public virtual DbSet<Documento> Documentos { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Responsable> Responsables { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

  

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseLazyLoadingProxies().UseMySQL("server=localhost;port=3306;database=gestiondocumentalbd;user=root;password=mysql");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CentrosEducativo>(entity =>
        {
            entity.HasKey(e => e.IdCentroEducativo).HasName("PRIMARY");
        });

        modelBuilder.Entity<CentrosTrabajo>(entity =>
        {
            entity.HasKey(e => e.IdCentroTrabajo).HasName("PRIMARY");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.CentrosTrabajos).HasConstraintName("centros_trabajo_ibfk_1");
        });

        modelBuilder.Entity<Documento>(entity =>
        {
            entity.HasKey(e => e.IdDocumento).HasName("PRIMARY");

            entity.HasOne(d => d.IdCentroEducativoNavigation).WithMany(p => p.Documentos).HasConstraintName("documentos_ibfk_1");

            entity.HasOne(d => d.IdEmpresaNavigation).WithMany(p => p.Documentos).HasConstraintName("documentos_ibfk_2");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Documentos).HasConstraintName("documentos_ibfk_4");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.IdEmpresa).HasName("PRIMARY");

            entity.HasOne(d => d.IdResponsableNavigation).WithOne(p => p.Empresa).HasConstraintName("id_responsable");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.IdPermiso).HasName("PRIMARY");
        });

        modelBuilder.Entity<Responsable>(entity =>
        {
            entity.HasKey(e => e.IdResponsable).HasName("PRIMARY");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PRIMARY");

            entity.HasMany(d => d.IdPermisos).WithMany(p => p.IdRols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolesPermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("IdPermiso")
                        .HasConstraintName("roles_permisos_ibfk_2"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("IdRol")
                        .HasConstraintName("roles_permisos_ibfk_1"),
                    j =>
                    {
                        j.HasKey("IdRol", "IdPermiso").HasName("PRIMARY");
                        j.ToTable("roles_permisos");
                        j.HasIndex(new[] { "IdPermiso" }, "id_permiso");
                        j.IndexerProperty<int>("IdRol").HasColumnName("id_rol");
                        j.IndexerProperty<int>("IdPermiso").HasColumnName("id_permiso");
                    });
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PRIMARY");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios).HasConstraintName("usuarios_ibfk_1");

            entity.HasOne(d => d.IdCentroEducativoNavigation).WithMany(p => p.Usuarios).HasConstraintName("fk_usuario_centro");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
