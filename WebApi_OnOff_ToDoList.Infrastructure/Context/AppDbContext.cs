using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApi_OnOff_ToDoList.Domain.Entities;

namespace WebApi_OnOff_ToDoList.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TblUser> TblUsers { get; set; }
        public DbSet<TblTask> TblTasks { get; set; }
        public DbSet<TblStatusTask> TblStatusTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TblUser>(user =>
            {
                user.ToTable("TBL_USER");
                user.HasKey(u => u.id);
                user.Property(u => u.fullName)
                    .IsRequired()
                    .HasMaxLength(100);
                user.Property(u => u.email)
                    .IsRequired()
                    .HasMaxLength(100);
                user.Property(u => u.passwordHash)
                    .IsRequired()
                    .HasMaxLength(255);
                user.Property(u => u.createdAt)
                    .HasDefaultValueSql("SYSDATETIME()");
                user.Property(u => u.isActive)
                    .HasDefaultValue(true);
                user.HasIndex(u => u.email)
                    .IsUnique(true);

                user.HasData(
                    new TblUser
                    {
                        id = 1,
                        fullName = "admin",
                        email = "admin@onoff.com",
                        passwordHash = "admin123",
                        createdAt = DateTime.Now,
                        isActive = true
                    }
                );
            });

            modelBuilder.Entity<TblStatusTask>(status =>
            {
                status.ToTable("TBL_STATUSTASK");
                status.HasKey(s => s.id);
                status.Property(s => s.name)
                    .IsRequired()
                    .HasMaxLength(50);
                status.Property(s => s.description)
                    .HasMaxLength(150);
                status.HasIndex(s => s.name)
                    .IsUnique(true);

                status.HasData(
                    new TblStatusTask { id = 1, name = "Bloqueado", description = "Tarea detenida por dependencia o error" },
                    new TblStatusTask { id = 2, name = "Por Hacer", description = "Pendiente de iniciar" },
                    new TblStatusTask { id = 3, name = "En Curso", description = "Actualmente en desarrollo" },
                    new TblStatusTask { id = 4, name = "QA", description = "En revisión de calidad" },
                    new TblStatusTask { id = 5, name = "Listo", description = "Finalizada y aprobada" }
                );
            });

            modelBuilder.Entity<TblTask>(task =>
            {
                task.ToTable("TBL_TASK");
                task.HasKey(t => t.id);
                task.Property(t => t.title)
                    .IsRequired()
                    .HasMaxLength(100);
                task.Property(t => t.description)
                    .HasMaxLength(250);
                task.Property(t => t.createdAt)
                    .HasDefaultValueSql("SYSDATETIME()");
                task.Property(t => t.updatedAt)
                    .IsRequired(false);

                task.HasOne(t => t.status)
                    .WithMany(s => s.tasks)
                    .HasForeignKey(t => t.idStatus)
                    .OnDelete(DeleteBehavior.Restrict);

                task.HasOne(t => t.user)
                    .WithMany(u => u.tasks)
                    .HasForeignKey(t => t.idUser)
                    .OnDelete(DeleteBehavior.Cascade);

                task.HasIndex(t => t.title);
            });
        }
    }
}
