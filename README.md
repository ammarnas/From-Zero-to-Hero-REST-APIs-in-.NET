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