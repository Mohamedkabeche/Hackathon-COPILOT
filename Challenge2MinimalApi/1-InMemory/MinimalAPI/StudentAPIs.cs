namespace MinimalAPI;

// this is a crud API to create, read, update and delete a list of students
// /create - POST - to create a new student
// / - GET - to read all students
// /read - GET - to read a student by ID
// /update/{studentId} - PUT - to update a student
// /delete/{studentId} - DELETE - to delete a student
// ReSharper disable once InconsistentNaming
public static class StudentAPIs
{
    public static void MapAPI(this WebApplication app)
    {
        app.MapPost("/create",
                    async ([FromBody] StudentInfo student, SchoolContext context) =>
                    {
                        context.Students.Add(student);
                        await context.SaveChangesAsync();
                        return Results.Ok(student);
                    });

        app.MapGet("/",
                   async (SchoolContext context) =>
                   {
                       var students = await context.Students.ToListAsync();
                       return Results.Ok(students);
                   });

        app.MapGet("/read/{studentId:int}",
                   async (int studentId, SchoolContext context) =>
                   {
                       var student = await context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
                       return student != null ? Results.Ok(student) : Results.NotFound();
                   });

        app.MapPut("/update/{studentId:int}",
                   async (int studentId, [FromBody] StudentInfo student, SchoolContext context) =>
                   {
                       var existingStudent = await context.Students.FirstOrDefaultAsync(e => e.Id == studentId);
                       if (existingStudent == null) return Results.NotFound();

                       existingStudent.FirstName = student.FirstName;
                       existingStudent.LastName = student.LastName;

                       await context.SaveChangesAsync();

                       return Results.Ok(existingStudent);
                   });

        app.MapDelete("/delete/{studentId:int}",
                      async (int studentId, SchoolContext context) =>
                      {
                          var existingStudent = await context.Students.FirstOrDefaultAsync(s => s.Id == studentId);
                          if (existingStudent == null) return Results.NotFound();

                          context.Students.Remove(existingStudent);
                          await context.SaveChangesAsync();

                          return Results.Ok();
                      });
    }
}
