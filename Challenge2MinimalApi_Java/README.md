# Challenge instructions for MinimalAPI

You are a developer working on an existing legacy project.
The project is a RESTful API that provides CRUD operations for a list of students.
The API is built using Spring Boot
The project have tests for the API endpoints.
All tests are passing.

## Step 1: Integrate Entity Framework Core to use a SQLite database for storing the student list

See instructions in the 1-InMemory/README.md

## Step 2: Fix various bugs in the API and ensure all tests are passing

See instructions in the 2-Database/README.md

# API Information

## Student class

```
public class Student
{
  private long id;
  private String firstName;
  private String lastName;
    ...
}
```

## API Endpoints

### Get All Students

- URL: `/api/students`
- Method: `POST`
- Request Body:

```json
{
    "firstName": "John Doe",
    "lastName": 25
}
```

### Read Student

- URL: `/api/students/{id}`
- Method: `GET`

- Response Body:

```json
{
    "id": 1,
    "firstName": "John Doe",
    "lastName": 25
}
```

### Create Student

- URL: `/api/students`
- Method: `POST`
- Request Body:

```json
{
    "firstName": "John Doe",
    "lastName": 25
}
```

- Response Body:

```json
{
    "id": 1,
    "firstName": "John Doe",
    "lastName": 25
}
```


### Update Student

- URL: `/api/students/{id}`
- Method: `PUT`
- Request Body:

```json
{
    "firstName": "John Doe",
    "lastName": 25
}
```

- Response Body:

```json
{
    "id": 1,
    "firstName": "John Doe",
    "lastName": 25
}
```

### Delete Student

- URL: `/api/students/{id}`
- Method: `DELETE`