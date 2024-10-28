using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Playmove.DAO.Models;

namespace Playmove.DAO.Data;

public partial class FornecedoresDbContext : IdentityDbContext
{
    public FornecedoresDbContext()
    {
    }

    public FornecedoresDbContext(DbContextOptions<FornecedoresDbContext> options)
        : base(options)
    {
    }

    
    public virtual DbSet<Fornecedor> Fornecedors { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        

        modelBuilder.Entity<Fornecedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Forneced__3214EC07D4125DF3");

            entity.ToTable("Fornecedor");

            entity.HasIndex(e => e.Cnpj, "UQ__Forneced__AA57D6B4F991F6A8").IsUnique();

            entity.Property(e => e.Cep)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cidade");
            entity.Property(e => e.Cnpj)
                .HasMaxLength(18)
                .IsUnicode(false)
                .HasColumnName("CNPJ");
            entity.Property(e => e.DataCadastro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("data_cadastro");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Endereco)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("endereco");
            entity.Property(e => e.Estado)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.Nome)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("NOME");
            entity.Property(e => e.Status).HasColumnName("STATUS");
            entity.Property(e => e.Telefone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("telefone");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuario__3214EC070FB43E59");

            entity.ToTable("Usuario");

            entity.Property(e => e.Nome)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("NOME");
            entity.Property(e => e.Sobrenome)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("SOBRENOME");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
