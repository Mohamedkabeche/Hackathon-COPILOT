using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MinimalAPI;

// this is a crud API to create, read, update and delete a list of students
// /create - POST - to create a new student
// /read - GET - to read all students
// /update/{studentId} - PUT - to update a student
// /delete/{studentId} - DELETE - to delete a student
// ReSharper disable once InconsistentNaming
public static class StudentAPIs
{
    public static void MapAPI(this WebApplication app)
    {
        app.MapGet("/",
                   async ([FromServices] StudentContext dbContext) =>
                   {
                       var students = await dbContext.Students.ToListAsync();
                       return Results.Ok(students.Select(Map));
                   });

        app.MapGet("/read/{studentId:int}",
                   async (int studentId, [FromServices] StudentContext context) =>
                   {
                       var student = await context.Students.FirstOrDefaultAsync(s => s.BusinessId == studentId);
                       if (student == null)
                       {
                           return Results.NotFound();
                       }
                       return Results.Ok(Map(student));
                   });

        // add an endpoint to search students born after a certain date
  app.MapGet("/read/born-after/{birthDate:datetime}",
            async (DateTime birthDate, [FromServices] StudentContext context) =>
            {
                if (birthDate > DateTime.Now)
                {
                    return Results.BadRequest("La date de naissance ne peut pas �tre dans le futur");
                }
                var students = await context.Students
                                            .Where(s => s.BirthDate > birthDate)
                                            .OrderByDescending(s => s.BirthDate)
                                            .ToListAsync();
                return Results.Ok(students.Select(Map));
            });

        app.MapPost("/create",
            async ([FromServices] StudentContext context,
                   [FromBody] StudentDto student) =>
            {
                context.Students.Add(Map(student));
                await context.SaveChangesAsync();
                return Results.Ok(student);
            });

        app.MapPut("/update/{studentId:int}",
                   async (int studentId,
                          [FromBody] StudentDto student,
                          [FromServices] StudentContext context) =>
                   {
                       var existingStudent = await context.Students.FirstOrDefaultAsync(e => e.BusinessId == studentId);
                       if (existingStudent == null)
                       {
                           return Results.NotFound();
                       }
                       existingStudent.BusinessId = studentId;
                       existingStudent.FirstName = student.FirstName;
                       existingStudent.LastName = student.LastName;
                       existingStudent.BirthDate = student.BirthDate;
                       await context.SaveChangesAsync();
                       return Results.Ok(Map(existingStudent));
                   });

        app.MapDelete("/delete/{studentId:int}",
            async (int studentId, [FromServices] StudentContext context) =>
            {
                var existingStudent = await context.Students.FirstOrDefaultAsync(s => s.BusinessId == studentId);
                if (existingStudent == null)
                {
                    return Results.NotFound();
                }

                context.Students.Remove(existingStudent);
                await context.SaveChangesAsync();
                return Results.Ok();
            });
    }

    static StudentDto Map(Student student) => new(student.BusinessId, student.FirstName, student.LastName, student.BirthDate);

    static Student Map(StudentDto student) => new()
    {
        BusinessId = student.Id,
        FirstName = student.FirstName,
        LastName = student.LastName,
        BirthDate = student.BirthDate,
    };
}
