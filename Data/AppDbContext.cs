using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebParfum.API.Models;
using WebParfum.API.Models.Decant;
using WebParfum.API.Models.Recuperar_clave;
using WebParfum.API.Models.Ventas;

namespace WebParfum.API.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Marca> Marcas { get; set; }
        public virtual DbSet<Perfume> Perfumes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Venta> Ventas { get; set; }
        public virtual DbSet<VentaDetalle> VentaDetalles { get; set; }
        public virtual DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

        public virtual DbSet<Decant> Decants { get; set; }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // La cadena de conexión se configurará vía DI desde appsettings.json
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Marca>(entity =>
            {
                entity.HasKey(e => e.MarcaId).HasName("PK__Marcas__D5B1CD8B8EFE0BCC");
                entity.HasIndex(e => e.Nombre, "UQ__Marcas__75E3EFCFADEE606F").IsUnique();
                entity.Property(e => e.Nombre).HasMaxLength(100);
            });

            modelBuilder.Entity<Perfume>(entity =>
            {
                entity.HasKey(e => e.PerfumeId).HasName("PK__Perfumes__A6B3A2EF8099A953");
                entity.HasIndex(e => e.MarcaId, "IX_Perfumes_MarcaId");
                entity.Property(e => e.Genero).HasMaxLength(50);
                entity.Property(e => e.ImagenUrl)
                    .HasMaxLength(255)
                    .HasColumnName("ImagenURL");
                entity.Property(e => e.Modelo).HasMaxLength(100);
                entity.Property(e => e.PrecioMayorista).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PrecioMinorista).HasColumnType("decimal(10, 2)");
                entity.HasOne(d => d.Marca).WithMany(p => p.Perfumes)
                    .HasForeignKey(d => d.MarcaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Perfumes_Marcas");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A00370981");
                entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B61609645DBC6").IsUnique();
                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE7B89ECB4BFE");
                entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D105341CB2F241").IsUnique();
                entity.Property(e => e.CodigoVerificacion).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.EmailVerificado).HasDefaultValue(false);
                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.NombreCompleto).HasMaxLength(100);
                entity.Property(e => e.PasswordHash).HasMaxLength(256);
                entity.HasMany(d => d.Roles).WithMany(p => p.Usuarios)
                    .UsingEntity<Dictionary<string, object>>(
                        "UsuarioRole",
                        r => r.HasOne<Role>().WithMany()
                            .HasForeignKey("RoleId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_UsuarioRoles_Roles"),
                        l => l.HasOne<Usuario>().WithMany()
                            .HasForeignKey("UsuarioId")
                            .OnDelete(DeleteBehavior.ClientSetNull)
                            .HasConstraintName("FK_UsuarioRoles_Usuarios"),
                        j =>
                        {
                            j.HasKey("UsuarioId", "RoleId").HasName("PK__UsuarioR__93924B5921C5AA97");
                            j.ToTable("UsuarioRoles");
                        });
            });

            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(e => e.VentaId).HasName("PK__Ventas__5B4150AC5DC0094F");
                entity.HasIndex(e => e.AdminId, "IX_Ventas_AdminId");
                entity.HasIndex(e => e.UsuarioId, "IX_Ventas_UsuarioId");
                entity.Property(e => e.Estado).HasMaxLength(50);
                entity.Property(e => e.FechaCompra)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");
                entity.HasOne(d => d.Admin).WithMany(p => p.VentaAdmins)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ventas_Admins");
                entity.HasOne(d => d.Usuario).WithMany(p => p.VentaUsuarios)
                    .HasForeignKey(d => d.UsuarioId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ventas_Usuarios");
            });

            modelBuilder.Entity<VentaDetalle>(entity =>
            {
                entity.HasKey(e => e.VentaDetalleId).HasName("PK__VentaDet__2DF62C37A45809AB");
                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Subtotal)
                    .HasComputedColumnSql("([Cantidad]*[PrecioUnitario])", true)
                    .HasColumnType("decimal(21, 2)");
                entity.HasOne(d => d.Perfume).WithMany(p => p.VentaDetalles)
                    .HasForeignKey(d => d.PerfumeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VentaDetalles_Perfumes");
                entity.HasOne(d => d.Venta).WithMany(p => p.VentaDetalles)
                    .HasForeignKey(d => d.VentaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VentaDetalles_Ventas");
            });

            modelBuilder.Entity<Decant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.ToTable("Decants");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                      .HasColumnName("Id")
                      .ValueGeneratedOnAdd();  // para INT IDENTITY

                entity.Property(e => e.Nombre)
                      .HasColumnName("Nombre")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.CodigoQR)
                      .HasColumnName("CodigoQR")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.CantidadDisponible)
                      .HasColumnName("CantidadDisponible")
                      .IsRequired();

                entity.Property(e => e.UrlImagen)
                      .HasColumnName("UrlImagen")
                      .HasMaxLength(200);

                entity.Property(e => e.Estado)
                      .HasColumnName("Estado")
                      .IsRequired();

                entity.Property(e => e.FechaCreacion)
                      .HasColumnName("FechaCreacion")
                      .HasDefaultValueSql("SYSDATETIMEOFFSET()");
            });


            // Configuración para PasswordResetCode (ya no se usará Fluent API para clave si se tiene [Key])
            modelBuilder.Entity<PasswordResetCode>(entity =>
            {
                entity.HasKey(e => e.ResetCodeId); // Asegura la clave primaria
                entity.Property(e => e.Code)
                      .IsRequired()
                      .HasMaxLength(10);
                entity.Property(e => e.ExpiryDate)
                      .IsRequired();
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("(getdate())");

               
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
