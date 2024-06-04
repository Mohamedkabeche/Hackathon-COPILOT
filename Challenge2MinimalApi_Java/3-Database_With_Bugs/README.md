# Spring Boot 3 + OpenAPI 3.0

## Challenge bugs

We improve our Student class to match our new requirement
- We want to use the functional `businessId` instead of the technical `id` in all request (find, update, delete)
- We add the birthdate and the calculated age
But unfortunately the project compile but when test the api we found functional bugs

Also we want a new endpoint GET `/search` to search for students by `BusinessId` and `FirstName` and `LastName`. 

## Launch Project

mvn compile    
mvn spring-boot:run

## Swagger

Navigate to: <http://localhost:42069/swagger-ui/index.html>

## Tasks

Your goal is to fix different found bugs using copilott
- Create request must create student with non null unique businessId
- Find/Update/Delete request id parameter must be the businessId
- Returned Age is incorrect
- Ensure to add unit tests for all this case
- Add the `/search` api

### Additinal Tasks

1. Use comment prompt to add an unique `Email` property in student
2. Use comment prompt to add `Email` to the property in the search api
3. Use comment prompt to add a new endpoint  `/promotions` to get students grouped by birth year and add this property

### Use copilot to generate tests for the new search endpoint using the comment prompt