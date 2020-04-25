using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SampleEfCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbContext = new SampleDbContext();
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
            var course = dbContext.Set<Course>().Include(x => x.Students).AsNoTracking()
                .First(x => x.Id == Guid.Parse("2e00a509-53cd-4338-b1b5-08d7e83d7e88"));
            course.Students.RemoveAt(0);
            dbContext.Set<Course>().Update(course);
            dbContext.SaveChanges();
            Console.WriteLine(course);
        }
    }
}