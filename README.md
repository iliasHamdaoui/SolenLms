[![Web Api - Build](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/api_pull_request.yml/badge.svg)](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/api_pull_request.yml)
[![Web Api - Deploy](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/api_push_main.yml/badge.svg)](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/api_push_main.yml) \
[![Web Client - Build](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/webclient_pull_request.yml/badge.svg)](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/webclient_pull_request.yml)
[![Web Client - Deploy](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/webclient_push_main.yml/badge.svg)](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/webclient_push_main.yml) \
[![Idp - Build](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/idp_pull_request.yml/badge.svg)](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/idp_pull_request.yml)
[![Idp - Deploy](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/idp_push_main.yml/badge.svg)](https://github.com/iliasHamdaoui/SolenLms/actions/workflows/idp_push_main.yml)



The Solen LMS is an open source Learning Management System. It's built using the latest version of `ASP.NET Core`. The application was
designed and developed following the principles of the *Clean Architecture* as defined
by [Robert C. Martin (aka Uncle Bob)](http://cleancoder.com) in his
book [Clean Architecture](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164).

# Clean Architecture (Quick Overview)

## Introduction

> _The idea of Clean Architecture, is to put the Business Logic and Rules (aka `Policies`) at the centre of the design,
and put the `Infrastructure` (aka `mechanisms`) at the edges of this design._

<div style="text-align:center">
<img src="https://user-images.githubusercontent.com/52765247/92224033-92e75180-eea1-11ea-8d48-16d6eadb8b11.png" />
</div>

The Business Rules are divided between two layers: the `Domain` layer (aka _Entities_) and the `Use Cases` layer. These
two layers form what is called the `Core` of the system.

## The Dependency Rule

What makes this architecture work is that all dependencies must flow inwards. The `Core` of the system has no
dependencies on any outside layers. The other layers depend on `Core`, but not on one another.

> _Source code dependencies must point only inward, toward high-level policies._

This is the architectural application of the `Dependecy Inversion Principle`.

## Characteristics of a _Clean Architecture_

A _Clean Architecture_ produces systems that have the following characteristics :

- _Independent of frameworks_. `Core` should not be dependent on external frameworks such as `Entity Framework`.
- _Testable_. The business rules can be tested without the UI, database, web server, or any other external element.
- _Independent of the UI_. The UI can change easily. For example, we can swap out the `Web UI` for a `Console UI`,
  or `Angular` for `Blazor`. Logic is contained within `Core`, so changing the UI will not impact the system.
- _Independent of the database_. We can change `SQL Server` for `Oracle`, `Mongo`, or something else. `Core` is not
  bound to the database.
- _Independent of any external agency_. `Core` simply doesn't know anything about the outside world.

For further reading about _Clean Architecture_, I highly recommend
this [book](https://www.amazon.com/Clean-Architecture-Craftsmans-Software-Structure/dp/0134494164).

# Solen LMS Architecture

## Solution Structure

![](https://user-images.githubusercontent.com/52765247/211998294-73162ce0-3abc-434b-b85e-e20c48c6c1ad.png)

The Solen LMS solution is composed of three applications :

- the Api (aka Backend) contains the business rules and does the heavy lifting.
- the Web Client (aka Frontend) contains the UIs to interact with the users. It's built with `blazor Webassembly`.
- and the Identity Provider (implemented by [Identity Server](https://duendesoftware.com/products/identityserver)) responsible for creating, managing and authenticating the users.

I could of course separate those applications into different solutions and different repositories,
but in my opinion, as long as the team size working on the project remains small, it's more convenient to keep them
in the same repository and the same solution.

Since the most important code resides in the API application, we'll focus more on its structure.

## Screaming Architecture

Let's take a look at the content of the API structure :

![](https://user-images.githubusercontent.com/52765247/212002616-f1810d7b-5a63-4db3-8ebf-c6d82a27bb02.png)

When we look at the top-level directory structure, we can guess what the application does. By reading the names of
the sub-directories, we can tell the propose of creating this application which is a Learning Management System.\
A good architecture of a software application should, in the first place, *scream* about the use cases of the
application, not about frameworks and tools. This is what Uncle Bob calls in his book **Screaming Architecture**.

Each sub-directory of the API directory represent a module, an independent module of the application. Each module is
composed of four layers :

- Domain Layer
- Use Cases Layer
- Infrastructure Layer
- Presentation Layer

## Domain Layer

The `Domain` layer contains the business objects called *Entities*. Those objects encapsulate the domain concepts within the *context* of the module
(ex: Course, Instructor, Learner, Category..) and grouped into different *Aggregates*. An *Aggregate*, in Domain Driven-Design
terminology, is a cluster of domain objects that can be treated as a single unit.

<details><summary>Click here to see the aggregates in the Course Management module
</summary>
<p>

![](https://user-images.githubusercontent.com/52765247/212053999-9e610666-55af-45f9-8d82-c0303e4bb41d.png)

</p>
</details>

## Use Cases Layer

### Presentation

This layer encapsulates and implements all the use cases of the module. A *Use Case* is an object that handles the users requests and applies rules 
that govern the interaction between the users and the *Entities*.\
Each *Use Case* is independent of the others (`Single Responsibility Principle`). For example, in the Course Management module, 
modifying or deleting the *Use Case* `UpdateCourse` will have absolutely no effects on the `DeleteCourse` *Use Case*. \
The *Use Cases* are grouped by the same logic as the entities, **i.e** *Aggregates*.

<details><summary>Click here to see the Use Cases of Course Management Module
</summary>
<p>

![image](https://user-images.githubusercontent.com/52765247/212055150-542fa654-e5f9-4c07-bdf8-f1ac92bd94d0.png)

</p>
</details>

### CQRS Pattern

To tackle business complexity and keep *Use Cases" simple to read and maintain, the Use Cases Layer implements the
architectural pattern [CQRS](https://martinfowler.com/bliki/CQRS.html). Using this pattern means clear separation
between *Commands* (Write operations) and *Queries* (Read operations). This separation gives us, we developers, a clear
picture of what pieces of code change the state of the application.


### Mediator Pattern

To keep the application business rules out the external layers and to prevent this layers from knowing much about the
Business Logic, the Use Case Layer implements the [Mediator Pattern](https://en.wikipedia.org/wiki/Mediator_pattern).

### MediatR Library

To implement the `CQRS` Pattern and the `Mediator` Pattern easily, the Use Cases Layer uses an open source
.NET library called [MediatR](https://github.com/jbogard/MediatR). It allows in-process messaging and provides an
elegant and powerful approach for writing `CQRS` and sending events between layers and modules.


### Requests Validation

When exposing public APIs, it is important to validate each incoming request to ensure it meets all expected
pre-conditions. The system should process valid requests but return an error for any invalid requests. \
The validation process is part of the *Use Cases* business logic. Therefore, the responsibility for validating requests
does not belong within the `Web API` or `Console UI` or whatsoever external interfaces, but rather in the Use Cases Layer. \
To make the validation process easier, I use a popular .NET library for building validation rules
called [FLUENT VALIDATION](https://fluentvalidation.net/).\
The other advantage of using this library, is we can make use of `MediatR` pipeline behaviours to validate automatically
every request that requires validation before further processing occurs.

## Infrastructure layer

The Infrastructure Layer implements all interfaces (abstractions) used by the Use Cases Layer to persist data, access external systems.... 
It contains also all configurations related to the Dependency Injection Container.

## Presentation layer

The *Presentation* Layer is the entry point to the system from the user’s point of view. Its primary concerns are routing requests to the Use Cases Layer.
This layer exposes public Web APIs. All the concerns related to the `GUIs` are handled by the Web Client application.

### _Thin_ Controller vs _Fat_ Controller

In the "traditional" way to write controllers, we usually implement some business logic flow in like as Validation,
Mapping Objects, Return HTTP status code...

Example of a _Fat_ Controller :

```csharp
[HttpPost]
public async Task<IHttpActionResult> CreateCourse(CourseModel model) {
    if (!ModelState.IsValid) return BadRequest (ModelState);

    var course = new Course {
        Title = model.title
    };

    var result = await _coursesService.CreateCourse(course);

    if (!result.Succeeded) return GetErrorResult (result);

    return Ok ();
}
```

By implementing the _Clean Architecture_ (where all the business logic and rules are implemented in the *Core* Layer),
and with the help of a library like `Mediadtr`, we can write controllers with few lines of code :

![](https://user-images.githubusercontent.com/52765247/92463244-d9072280-f1cb-11ea-9eba-3ed9dea306a9.PNG)


# Contribution

For the moment, I will be the only contributor of the project. Nevertheless, you're welcome to report bugs or/and submit
features by creating issues.
