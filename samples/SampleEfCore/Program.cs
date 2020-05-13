using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace SampleEfCore
{
    public class Data
    {
        public DateTime DateTime { get; set; }
        public decimal Value { get; set; }
        public Category Type { get; set; }
    }

    public enum Category
    {
        Sales,
        Refund,
        ChargeBack
    }

    public class Model
    {
        public DateTime Date { get; set; }
        public decimal  Sale{ get; set; }
        public decimal Refund { get; set; }
    }
    class Program
    {
        
        static void Main(string[] args)
        {
            var datas = new List<Data>();
            datas.Add(new Data()
            {
                DateTime = DateTime.Now.AddDays(-1),
                Type = Category.Sales,
                Value = 1
            });
            datas.Add(new Data()
            {
                DateTime = DateTime.Now.AddDays(-1),
                Type = Category.Refund,
                Value = 2
            });
            datas.Add(new Data()
            {
                DateTime = DateTime.Now.AddDays(-1),
                Type = Category.ChargeBack,
                Value = 3
            });

            var models = datas.GroupBy(x => x.DateTime).Select(y => new Model()
            {
                Date = y.Key,
                Refund = y.FirstOrDefault(x => x.Type == Category.Refund)?.Value ?? 0,
                Sale = y.FirstOrDefault(x => x.Type == Category.Sales)?.Value ?? 0
            }).ToList();
            
            
            
            var serializeObject = JsonConvert.SerializeObject(models);
            Console.WriteLine(serializeObject);
            
            //var dbContext = new SampleDbContext();
            // var course = new Course()
            // {
            //     
            //     Name = "English Level 1",
            //     Students = new List<Student>()
            //     {
            //         new Student()
            //         {
            //             
            //             Name = "StudentA"
            //         },
            //         new Student()
            //         {
            //             Name = "StudentB"
            //         }
            //     }
            // };
            // dbContext.Set<Course>().Add(course);
            // dbContext.SaveChanges();
            // dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            //
            // var course = dbContext.Set<Course>().Include(x=>x.Students).First(x => x.Id == Guid.Parse("2e00a509-53cd-4338-b1b5-08d7e83d7e88"));
            // course.RemoveStudent();
            // dbContext.ChangeTracker.TrackGraph(course,dbContext.Target);
            //
            // foreach (var entityEntry in dbContext.ChangeTracker.Entries())
            // {
            //     Console.WriteLine(entityEntry.Entity.GetType().ToString() + ' ' +entityEntry.State.ToString());
            // }
            // dbContext.SaveChanges();
            // Console.WriteLine(course);
        }

        public class Repository
        {
            private readonly SampleDbContext _dbContext;

            public Repository(SampleDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public void Add(Course course)
            {
                // Add everything and save changes to get new id 
            }

            public void Update(Course course)
            {
                // Update parent and reference objects.
                
            }

            public void Delete(Course course)
            {
                // Delete parent and reference objects.
            }
        }
    }
}