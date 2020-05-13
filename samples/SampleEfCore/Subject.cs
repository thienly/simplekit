using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SampleEfCore
{
    public enum State
    {
        UnChanged,
        Added,
        Modified,
        Deleted
    }

    public interface IState
    {
        State State { get; set; }
    }

    public abstract class AggregateRoot<TId> where TId : IComparable
    {
        public abstract TId Id { get; set; }
    }

    public class Course : AggregateRoot<Guid>, IState
    {
        public override Guid Id { get; set; }
        public string Name { get; set; }
        public List<Student> Students { get; set; }
        [NotMapped] public State State { get; set; }

        public void RemoveStudent()
        {
            var student = Students.First();
            student.State = State.Deleted;
        }
    }

    public class Student : AggregateRoot<int>, IState
    {
        public override int Id { get; set; }
        public string Name { get; set; }
        [NotMapped] public State State { get; set; }
    }

    public class CourseEntityTypeConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Course");
            builder.Property(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.HasMany(x => x.Students);
        }
    }

    public class StudentEntityTypeConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Student");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
        }
    }

    public class SampleDbContext : DbContext
    {
        public SampleDbContext()
        {
        }

        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CourseEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StudentEntityTypeConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=SampleDb;User Id=sa;Password=Test!234");
            }
        }

        public override EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
        {
            var entityEntry = base.Attach(entity);
            entityEntry.State = EntityState.Detached;
            return entityEntry;
        }

        public void Target(EntityEntryGraphNode n)
        {
            n.Entry.State = EntityState.Detached;
            var entity = (IState) n.Entry.Entity;
            
            n.Entry.State = entity.State == State.Added
                ? EntityState.Added
                : entity.State == State.Modified
                    ? EntityState.Modified
                    : entity.State == State.Deleted
                        ? EntityState.Deleted
                        : EntityState.Unchanged;
            int x = 1;
        }
    }
}