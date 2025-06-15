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

# Perfecting the movie creation endpoint
## CreatedAtRoute
-	**Purpose:** Returns a 201 Created response with a Location header pointing to a resource, using a named route.

-	**Usage:** **Use when you want to reference a route by its name** (e.g., [Route("api/movies/{id}", Name = "GetMovieById")]).

-	**Parameters:** Typically takes the route name, **route values** (such as the new resource's ID), and the response body.

-	**Example:**

```
return CreatedAtRoute(
        routeName: "GetMovieById",
        routeValues: new { id = movie.Id }, // parameters of route
        value: movieResponse);
```

## CreatedAtAction
- **Purpose:** Returns a 201 Created response with a Location header pointing to a resource, **using a controller action name**.

- **Usage:** Use when you want to reference a specific action method (e.g., GetById) in the same or another controller.

- **Parameters:** Takes the action name, route values, and the response body.

- **Example:**
```
    return CreatedAtAction(
        actionName: nameof(GetById),
        routeValues: new { id = movie.Id },// parameters of route
        value: movieResponse
    );
```

# Implementing movie update
- Add update endpoint.
- Add mapping from update request to movie.
- Add update route to ApiEndpoint.

# Implementing movie deletion
- Add delete endpoint.

# Implementing slug-based retrieval

- Rename Endpoint `GetById` to `Get`
- Update the route
- Implement slug-based retrieval and update related endpoints
- Implement GetBySlugAsync
- Add Slug to movie model
- generate slug
- add slug to movie response

- ## Using Regex.Replace Directly

    ```
    private string GenerateSlug()
    {
        var sluggedTitle = Regex.Replace(Title, "[^a-zA-Z0-9 _-]", string.Empty)
            .ToLower()
            .Replace(" ", "-");

        return $"{sluggedTitle}-{YearOfRelease}";
    }
    ```
    
    - **How it works:**

        Every time **GenerateSlug()** is called, a new **Regex** object is created with the pattern **[^a-zA-Z0-9 _-]** and used to replace unwanted characters.
    -	**Performance:**

        Less efficient, especially if called frequently, because the regex is parsed and compiled each time.
    -	**Simplicity:**

        Straightforward and easy to read, but not optimal for repeated use.

- ## Using a Generated Regex Method
    ```
    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex().Replace(Title, string.Empty)
            .ToLower()
            .Replace(" ", "-");

        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^a-zA-Z0-9 _-]", RegexOptions.NonBacktracking, 5)]
    private static partial Regex SlugRegex();
    ```

    - **How it works:**

        The **[GeneratedRegex]** attribute (C# 11/.NET 7+) generates a static, compiled regex method at build time. **SlugRegex()** returns a cached, precompiled Regex instance.
    - **Performance:**

        More efficient, as the regex is compiled once and reused, reducing overhead.
    - **Modernity:**

        Uses newer C#/.NET features for better performance and maintainability.

- ## Error: No route matches the supplied values.
    This error happen when the route or parameters value of `CreatedAtRoute` or `CreatedAtAction` is wrong.

# Moving to a real database
[Create PostgreSQL in Docker and connect with DBeaver](https://www.youtube.com/watch?v=27tNXzioVGI&ab_channel=MostlyCode)

# Adding the database infrastructure code
- Install **Dapper** to Application layer.
- Install **Npgsql** to Application layer.
- Move the docker-compose to Application layer.
- Create Interface for db operation and create db connection using postgres.
- Add the db connection service using extension method `AddDatabase`.
- Register db connection service in `program.cs`.
- Create DbInitializer to init database
- Register DbInitializer in `AddDatabase`
- initialize the db in `program.cs` after map controllers.
- I Update the db connection string in the database to use `Host` instead of `Server` to work.

# Removing the old in-memory database
- Remove all code from `MovieRepository` of in memory db.
- Create `genres` Table in `DbInitializer`.
## Important Methods in Dapper
1. **BeginTransaction**

    **Purpose:** Starts a new database transaction on the current connection.

    **Usage:**

    ```  using var transaction = connection.BeginTransaction();```

    **Effect:** 
    All subsequent commands on this connection can be committed or rolled   back as a single unit, ensuring atomicity.

2. **ExecuteAsync**

    **Purpose:** Executes a SQL command asynchronously (e.g., INSERT, UPDATE, DELETE).

    **Usage:**

    ```  await connection.ExecuteAsync(new CommandDefinition("SQL", parameters));```

    **Returns:** The number of rows affected.

3. **QueryAsync**
    **Purpose:** Executes a SQL query asynchronously and returns the result as a collection of dynamic objects or a specified type.

    **Usage:**

    ```  var result = await connection.QueryAsync("SQL", parameters);```

    **Returns:** An **IEnumerable<T>** or **IEnumerable<dynamic>** with the query results.

4. **QuerySingleOrDefaultAsync**
    **Purpose:** Executes a SQL query asynchronously and returns a single result or a default value if no rows are found.

    **Usage:**

    ```  var movie = await connection.QuerySingleOrDefaultAsync<Movie>("SQL", parameters);```

    **Returns:** A single object of type **T** or **null** if no result is found. Throws if more than one row is returned.

5. **ExecuteScalarAsync**

    **Purpose:** Executes a SQL query asynchronously and returns the first column of the first row in the result set.

    **Usage:**

    ```  var exists = await connection.ExecuteScalarAsync<bool>("SQL", parameters);```

    **Returns:** A single value (e.g., a count, a boolean, etc.).

# Adding a business logic layer
- Add `MovieService` to separate the business layer inside and keep `MovieRepository` responsible for database connection 
- replace all usage of `MovieRepository` in controller with `MovieService`
- Register `MovieService` in application services

# Implementing validation
- Add `FluentValidation.DependencyInjectionExtensions` Package that has FluentValidation and Dependency Injection Extensions Methods 
- Create `MovieValidator`
- Register the `FluentValidation` in AddApplication with singleton life time
- We can add empty interface to mark where is the assembly of project
- Add `ValidationMappingMiddleware` to handle the validation errors register this middleware in `program.cs`

# Cancellation token passing
- Add cancellation token to controller, service, repository, dbConnection

# Authentication and Authorization in REST APIs

- **Authentication:** is the process of verifying who the user is.

- **Authorization:** is the process of verifying what the user can do.

- Add `Identity.Api` project