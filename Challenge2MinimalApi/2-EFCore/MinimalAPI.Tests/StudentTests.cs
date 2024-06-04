using System.Net;
using System.Text;
using System.Text.Json;

using MinimalAPI.Tests.Helpers;

namespace MinimalAPI.Tests;

public class StudentTests(TestWebApplicationFactory<Program> factory) : IClassFixture<TestWebApplicationFactory<Program>>
{
    readonly HttpClient _client = factory.CreateClient();
    readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, };

    #region Create

    [Fact]
    public async Task CreateStudent_ReturnsCreatedStudent()
    {
        // Arrange
        var student = new StudentDto(1, "John", "Doe", DateTime.Today);
        var content = new StringContent(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/create", content);

        // Assert
        response.EnsureSuccessStatusCode();

        var returnedStudent = JsonSerializer.Deserialize<Student>(await response.Content.ReadAsStringAsync(), _jsonSerializerOptions);
        Assert.Equal(student.Id, returnedStudent?.Id);
        Assert.Equal(student.FirstName, returnedStudent?.FirstName);
        Assert.Equal(student.LastName, returnedStudent?.LastName);
        Assert.Equal(student.BirthDate, returnedStudent?.BirthDate);
    }

    [Fact]
    public async Task CreateStudent_ReturnsBadRequest_WhenStudentIsNull()
    {
        // Arrange
        StringContent content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/create", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Update

    [Fact]
    public async Task UpdateStudent_ReturnsUpdatedStudent()
    {
        // Arrange
        var student = new StudentDto(1, "John", "Doe", DateTime.Today);
        var content = new StringContent(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json");

        var createdResult = await _client.PostAsync("/create", content);
        var createdStudent = JsonSerializer.Deserialize<Student>(await createdResult.Content.ReadAsStringAsync(), _jsonSerializerOptions);

        // Act
        student.FirstName = "Jane";
        content = new(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/update/{createdStudent?.Id}", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var returnedStudent = JsonSerializer.Deserialize<Student>(await response.Content.ReadAsStringAsync(), _jsonSerializerOptions);
        Assert.Equal(student.FirstName, returnedStudent?.FirstName);
        Assert.Equal(student.LastName, returnedStudent?.LastName);
    }

    // test for updating a non-existent student
    [Fact]
    public async Task UpdateStudent_ReturnsNotFound_WhenStudentDoesNotExist()
    {
        // Arrange
        var student = new StudentDto(1, "John", "Doe", DateTime.Today);
        var content = new StringContent(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/update/999", content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Delete

    [Fact]
    public async Task DeleteStudent_ReturnsOk_WhenStudentExists()
    {
        // Arrange
        var student = new StudentDto(1, "John", "Doe", DateTime.Today);
        var content = new StringContent(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json");
        await _client.PostAsync("/create", content);

        // Act
        var response = await _client.DeleteAsync("/delete/1");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    // test for deleting a non-existent student
    [Fact]
    public async Task DeleteStudent_ReturnsNotFound_WhenStudentDoesNotExist()
    {
        // Arrange
        var student = new StudentDto(1, "John", "Doe", DateTime.Today);
        var content = new StringContent(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json");
        await _client.PostAsync("/create", content);

        // Act
        var response = await _client.DeleteAsync("/delete/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Read

    [Fact]
    public async Task Reads_EndpointReturnsSuccess()
    {
        // arrange
        var student1 = new { Id = 1, FirstName = "John", LastName = "Doe" };
        var student2 = new { Id = 2, FirstName = "Jane", LastName = "Doe" };

        var student1Json = JsonSerializer.Serialize(student1);
        var student2Json = JsonSerializer.Serialize(student2);
        await _client.PostAsync("/create", new StringContent(student1Json, Encoding.UTF8, "application/json"));
        await _client.PostAsync("/create", new StringContent(student2Json, Encoding.UTF8, "application/json"));

        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        var students = JsonSerializer.Deserialize<List<Student>>(responseString, _jsonSerializerOptions);

        Assert.NotNull(students);
        Assert.Contains(students, s => s.Id == 1);
        Assert.Contains(students, s => s.Id == 2);
    }

    [Fact]
    public async Task Read_EndpointReturnsSuccess()
    {
        // arrange
        var student = new StudentDto(1, "John", "Doe", DateTime.Today);
        var data = new StringContent(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json");
        await _client.PostAsync("/create", data);

        // Act
        var response = await _client.GetAsync("/read/1");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        var responseString = await response.Content.ReadAsStringAsync();
        var studentResponse = JsonSerializer.Deserialize<Student>(responseString, _jsonSerializerOptions);

        Assert.NotNull(studentResponse);
        Assert.Equal(1, studentResponse.Id);
        Assert.Equal("John", studentResponse.FirstName);
        Assert.Equal("Doe", studentResponse.LastName);
    }

    [Fact]
    public async Task Read_StudentNotFound_ReturnsNotFound()
    {
        // Arrange
        var nonExistentStudentId = 999; // Assuming this ID does not exist in the database

        // Act
        var response = await _client.GetAsync($"/read/{nonExistentStudentId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Business

    [Fact]
    public void StudentDto_CalculatesAgeCorrectly()
    {
        // Arrange
        var birthDate = new DateTime(2000, 1, 1);
        var studentDto = new StudentDto(1, "John", "Doe", birthDate);

        // Act
        Assert.Equal(studentDto.Age, 24);
    }
    #endregion

    #region Read Born After

    [Fact]
    public async Task ReadBornAfter_ReturnsStudentsBornAfterGivenDate()
    {
        // Arrange
        var student1 = new StudentDto(1, "John", "Doe", new DateTime(2000, 1, 1));
        var student2 = new StudentDto(2, "Jane", "Doe", new DateTime(2002, 1, 1));
        var content1 = new StringContent(JsonSerializer.Serialize(student1), Encoding.UTF8, "application/json");
        var content2 = new StringContent(JsonSerializer.Serialize(student2), Encoding.UTF8, "application/json");

        // Clean up the database before each test
        await _client.DeleteAsync("/delete/1");
        await _client.DeleteAsync("/delete/2");

        // Create the students
        await _client.PostAsync("/create", content1);
        await _client.PostAsync("/create", content2);

        // Act
        var response = await _client.GetAsync("/read/born-after/2001-01-01");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        var responseString = await response.Content.ReadAsStringAsync();
        var students = JsonSerializer.Deserialize<List<Student>>(responseString, _jsonSerializerOptions);

        Assert.NotNull(students);
        Assert.Single(students);
        Assert.Equal(2, students[0].Id);
    }

    [Fact]
    public async Task ReadBornAfter_ReturnsEmptyList_WhenNoStudentsBornAfterGivenDate()
    {
        // Arrange
        var student1 = new StudentDto(1, "John", "Doe", new DateTime(2000, 1, 1));
        var student2 = new StudentDto(2, "Jane", "Doe", new DateTime(2002, 1, 1));
        var content1 = new StringContent(JsonSerializer.Serialize(student1), Encoding.UTF8, "application/json");
        var content2 = new StringContent(JsonSerializer.Serialize(student2), Encoding.UTF8, "application/json");
        await _client.PostAsync("/create", content1);
        await _client.PostAsync("/create", content2);

        // Act
        var response = await _client.GetAsync("/read/born-after/2003-01-01");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299

        var responseString = await response.Content.ReadAsStringAsync();
        var students = JsonSerializer.Deserialize<List<Student>>(responseString, _jsonSerializerOptions);

        Assert.NotNull(students);
        Assert.Empty(students);
    }

    [Fact]
    public async Task ReadBornAfter_ReturnsBadRequest_WhenDateIsInFuture()
    {
        // Arrange
        var futureDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

        // Act
        var response = await _client.GetAsync($"/read/born-after/{futureDate}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion


}
