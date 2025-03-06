﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using MaquinaEstado.DB.Model;
using Microsoft.EntityFrameworkCore;

namespace MaquinaEstado.DB.DataSet;

public partial class MaquinaEstadosDBContext : DbContext
{
    public MaquinaEstadosDBContext(DbContextOptions<MaquinaEstadosDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Estados> Estados { get; set; }

    public virtual DbSet<HistorialEstados> HistorialEstados { get; set; }

    public virtual DbSet<Solicitudes> Solicitudes { get; set; }

    public virtual DbSet<Transiciones> Transiciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estados>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Estados__3214EC07B394CCB3");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreadoPor)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Modificado).HasColumnType("datetime");
            entity.Property(e => e.ModificadoPor).HasMaxLength(100);
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<HistorialEstados>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Historia__3214EC07B4544810");

            entity.HasIndex(e => e.SolicitudId, "IX_HistorialEstados_SolicitudId").HasFilter("([Activo]=(1))");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreadoPor)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.FechaCambio)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Modificado).HasColumnType("datetime");
            entity.Property(e => e.ModificadoPor).HasMaxLength(100);
            entity.Property(e => e.Usuario)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.EstadoAnterior).WithMany(p => p.HistorialEstadosEstadoAnterior)
                .HasForeignKey(d => d.EstadoAnteriorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Estad__49C3F6B7");

            entity.HasOne(d => d.EstadoNuevo).WithMany(p => p.HistorialEstadosEstadoNuevo)
                .HasForeignKey(d => d.EstadoNuevoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Estad__4AB81AF0");

            entity.HasOne(d => d.Solicitud).WithMany(p => p.HistorialEstados)
                .HasForeignKey(d => d.SolicitudId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Historial__Solic__48CFD27E");
        });

        modelBuilder.Entity<Solicitudes>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Solicitu__3214EC0746A4E735");

            entity.HasIndex(e => e.EstadoId, "IX_Solicitudes_EstadoId").HasFilter("([Activo]=(1))");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreadoPor)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Descripcion)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.Modificado).HasColumnType("datetime");
            entity.Property(e => e.ModificadoPor).HasMaxLength(100);

            entity.HasOne(d => d.Estado).WithMany(p => p.Solicitudes)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Solicitud__Estad__4316F928");
        });

        modelBuilder.Entity<Transiciones>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transici__3214EC0744319D04");

            entity.HasIndex(e => e.EstadoDestinoId, "IX_Transiciones_Destino").HasFilter("([Activo]=(1))");

            entity.HasIndex(e => e.EstadoOrigenId, "IX_Transiciones_Origen").HasFilter("([Activo]=(1))");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Creado)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CreadoPor)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Modificado).HasColumnType("datetime");
            entity.Property(e => e.ModificadoPor).HasMaxLength(100);

            entity.HasOne(d => d.EstadoDestino).WithMany(p => p.TransicionesEstadoDestino)
                .HasForeignKey(d => d.EstadoDestinoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transicio__Estad__3E52440B");

            entity.HasOne(d => d.EstadoOrigen).WithMany(p => p.TransicionesEstadoOrigen)
                .HasForeignKey(d => d.EstadoOrigenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transicio__Estad__3D5E1FD2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}