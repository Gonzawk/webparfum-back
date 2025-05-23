﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebParfum.API.Data;

#nullable disable

namespace WebParfum.API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250320110150_CreatePasswordResetCodeTable")]
    partial class CreatePasswordResetCodeTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Usuario", b =>
                {
                    b.Property<int>("UsuarioId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UsuarioId"));

                    b.Property<string>("CodigoVerificacion")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool?>("EmailVerificado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("FechaRegistro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("NombreCompleto")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("UsuarioId")
                        .HasName("PK__Usuarios__2B3DE7B89ECB4BFE");

                    b.HasIndex(new[] { "Email" }, "UQ__Usuarios__A9D105341CB2F241")
                        .IsUnique();

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("UsuarioRole", b =>
                {
                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("UsuarioId", "RoleId")
                        .HasName("PK__UsuarioR__93924B5921C5AA97");

                    b.HasIndex("RoleId");

                    b.ToTable("UsuarioRoles", (string)null);
                });

            modelBuilder.Entity("WebParfum.API.Models.Marca", b =>
                {
                    b.Property<int>("MarcaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MarcaId"));

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("MarcaId")
                        .HasName("PK__Marcas__D5B1CD8B8EFE0BCC");

                    b.HasIndex(new[] { "Nombre" }, "UQ__Marcas__75E3EFCFADEE606F")
                        .IsUnique();

                    b.ToTable("Marcas");
                });

            modelBuilder.Entity("WebParfum.API.Models.Perfume", b =>
                {
                    b.Property<int>("PerfumeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PerfumeId"));

                    b.Property<string>("Descripcion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Genero")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("ImagenUrl")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("ImagenURL");

                    b.Property<int>("MarcaId")
                        .HasColumnType("int");

                    b.Property<string>("Modelo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("PrecioMayorista")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal>("PrecioMinorista")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<int>("Volumen")
                        .HasColumnType("int");

                    b.HasKey("PerfumeId")
                        .HasName("PK__Perfumes__A6B3A2EF8099A953");

                    b.HasIndex(new[] { "MarcaId" }, "IX_Perfumes_MarcaId");

                    b.ToTable("Perfumes");
                });

            modelBuilder.Entity("WebParfum.API.Models.Recuperar_clave.PasswordResetCode", b =>
                {
                    b.Property<int>("ResetCodeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ResetCodeId"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("ResetCodeId");

                    b.HasIndex("UsuarioId");

                    b.ToTable("PasswordResetCodes");
                });

            modelBuilder.Entity("WebParfum.API.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("RoleId")
                        .HasName("PK__Roles__8AFACE1A00370981");

                    b.HasIndex(new[] { "RoleName" }, "UQ__Roles__8A2B61609645DBC6")
                        .IsUnique();

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("WebParfum.API.Models.Ventas.Venta", b =>
                {
                    b.Property<int>("VentaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VentaId"));

                    b.Property<int>("AdminId")
                        .HasColumnType("int");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("FechaCompra")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("VentaId")
                        .HasName("PK__Ventas__5B4150AC5DC0094F");

                    b.HasIndex(new[] { "AdminId" }, "IX_Ventas_AdminId");

                    b.HasIndex(new[] { "UsuarioId" }, "IX_Ventas_UsuarioId");

                    b.ToTable("Ventas");
                });

            modelBuilder.Entity("WebParfum.API.Models.Ventas.VentaDetalle", b =>
                {
                    b.Property<int>("VentaDetalleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VentaDetalleId"));

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.Property<int>("PerfumeId")
                        .HasColumnType("int");

                    b.Property<decimal>("PrecioUnitario")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<decimal?>("Subtotal")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("decimal(21, 2)")
                        .HasComputedColumnSql("([Cantidad]*[PrecioUnitario])", true);

                    b.Property<int>("VentaId")
                        .HasColumnType("int");

                    b.HasKey("VentaDetalleId")
                        .HasName("PK__VentaDet__2DF62C37A45809AB");

                    b.HasIndex("PerfumeId");

                    b.HasIndex("VentaId");

                    b.ToTable("VentaDetalles");
                });

            modelBuilder.Entity("UsuarioRole", b =>
                {
                    b.HasOne("WebParfum.API.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .IsRequired()
                        .HasConstraintName("FK_UsuarioRoles_Roles");

                    b.HasOne("Usuario", null)
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .IsRequired()
                        .HasConstraintName("FK_UsuarioRoles_Usuarios");
                });

            modelBuilder.Entity("WebParfum.API.Models.Perfume", b =>
                {
                    b.HasOne("WebParfum.API.Models.Marca", "Marca")
                        .WithMany("Perfumes")
                        .HasForeignKey("MarcaId")
                        .IsRequired()
                        .HasConstraintName("FK_Perfumes_Marcas");

                    b.Navigation("Marca");
                });

            modelBuilder.Entity("WebParfum.API.Models.Recuperar_clave.PasswordResetCode", b =>
                {
                    b.HasOne("Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("UsuarioId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("WebParfum.API.Models.Ventas.Venta", b =>
                {
                    b.HasOne("Usuario", "Admin")
                        .WithMany("VentaAdmins")
                        .HasForeignKey("AdminId")
                        .IsRequired()
                        .HasConstraintName("FK_Ventas_Admins");

                    b.HasOne("Usuario", "Usuario")
                        .WithMany("VentaUsuarios")
                        .HasForeignKey("UsuarioId")
                        .IsRequired()
                        .HasConstraintName("FK_Ventas_Usuarios");

                    b.Navigation("Admin");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("WebParfum.API.Models.Ventas.VentaDetalle", b =>
                {
                    b.HasOne("WebParfum.API.Models.Perfume", "Perfume")
                        .WithMany("VentaDetalles")
                        .HasForeignKey("PerfumeId")
                        .IsRequired()
                        .HasConstraintName("FK_VentaDetalles_Perfumes");

                    b.HasOne("WebParfum.API.Models.Ventas.Venta", "Venta")
                        .WithMany("VentaDetalles")
                        .HasForeignKey("VentaId")
                        .IsRequired()
                        .HasConstraintName("FK_VentaDetalles_Ventas");

                    b.Navigation("Perfume");

                    b.Navigation("Venta");
                });

            modelBuilder.Entity("Usuario", b =>
                {
                    b.Navigation("VentaAdmins");

                    b.Navigation("VentaUsuarios");
                });

            modelBuilder.Entity("WebParfum.API.Models.Marca", b =>
                {
                    b.Navigation("Perfumes");
                });

            modelBuilder.Entity("WebParfum.API.Models.Perfume", b =>
                {
                    b.Navigation("VentaDetalles");
                });

            modelBuilder.Entity("WebParfum.API.Models.Ventas.Venta", b =>
                {
                    b.Navigation("VentaDetalles");
                });
#pragma warning restore 612, 618
        }
    }
}
