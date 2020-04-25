using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SampleEfCore
{
    public abstract class AggregateRoot<TId> where TId : IComparable
    {
        public abstract TId Id { get; set; }
        
    }
    public class Course : AggregateRoot<Guid>
    {
        public override Guid Id { get; set; }
        public string Name { get; set; }
        public List<Student> Students { get; set; }
    }

    public class Student : AggregateRoot<int>
    {
        public override int Id { get; set; }
        public string Name { get; set; }
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
        
    }
}