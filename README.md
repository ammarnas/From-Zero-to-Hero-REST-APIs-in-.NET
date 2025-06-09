# From-Zero-to-Hero-REST-APIs-in-.NET

## The system we will build an API for
Add projects and postman requests and add .gitignore

## From Zero to Hero: REST APIs in .NET
- Create the movie repository
- ### Register Service:
    - we can register services in ```program.cs``` directly but it is not good because the benefit of ```Application``` Layer is to encapsulate the logic inside it.


    - instead of that I create Extension method ```AddApplication()``` in the application layer to register service then I use it in the ```program.cs```.


    - to do that we need to install ```Microsoft.Extensions.DependencyInjection.Abstractions``` package in the Application layer to do register the services.

# Creating the movies controller
Create the movie controller.

# Implementing movie creation

# Introducing mapping
Create mapping from CreateMovieRequest to Movie ```MapToMovie```  by create my own mapping class in ```ContractMapping```

# Keeping track of the endpoints
- ApiEndpoints class is a static utility class that centralizes the definition of API endpoint route strings for your application.

- this is a good practice for small to medium-sized projects. It improves maintainability and reduces duplication. 

## What is the benefit of it?
-	**Centralization:** All API route strings are defined in one place, making them easy to manage and update.
-	**Consistency:** Reduces the risk of typos or inconsistencies in route strings across controllers, services, and clients.
-	**Refactoring:** If you need to change a route, you only update it in one location.

-	**Discoverability:** Developers can quickly see all available endpoints and their structure.
## 	Alternative
### use a shared library for route definitions, but keep it DRY and in sync.
A **shared library** is a separate project (usually a class library) that contains code or constants used by multiple projects. For route definitions, this means putting your API endpoint strings (like "api/movies") in a shared library that both your API project and any client projects (like a frontend or integration tests) can reference.

**How to Keep it DRY and in Sync**
1. Create a Shared Project:
For example, Movies.Shared or Movies.Contracts.
2.	Define Route Constants/Methods.
Place your endpoint definitions in a static class in the shared project.
3.	Reference the Shared Library:
Add a project reference to Movies.Shared from both your API and client projects.
4.	Use the Shared Routes Everywhere.


# Implementing movie retrieval
- Add Get and GetAll Endpoint
- Add Map to movieResponse and map To MoviesResponse
- Fix MoviesResponse type