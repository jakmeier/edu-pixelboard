# Distributed Pixel Board

An educational project used in distributed systems classes.

The intended audience knows a bit of C# and ASP.NET Core.

Teams of students implement a client application to read and modify a shared
pixel board. Most of the course is done in local development, without the shared
state. But the end goal is to play an interactive mini game on the pixel board
where each team tries to score points by drawing specific shapes in their team's
color while preventing other teams to draw theirs. A functioning client is
essential to be competitive.

## Concepts

- REST API basics
- Local development practices with test and production environments
- Publish / subscribe concept
- OAuth 2.0 (code authorization)

## Tech Stack

For students:

- ASP.NET Core, specifically Razor Pages web apps
- Redis (client view only)

Additional tech for teachers:

- Hosting of Redis, Keycloack, and a ASP.NET Core based web server (docker-compose configuration provided)
- Configure Keycloak as the OIDC provider

## Quick Start (Local Dev Environment)

(WORK IN PROGRESS! Requires additional setup)

In the class, we explore components one by one and learn how to run them one by
one. Doing it manually takes more time but helps to understand what happens. But
if you just want to try out the final project and run it locally, do this:

```bash
docker-compose build
docker-compose up
```

This runs 4 docker containers (Keycloak, Postgres, Redis, ASP.NET API server)
which form the infrastructure to develop a client application against.

A sample client is in `./student_client`. To run it:

```bash
cd student_client
dotnet run
```

TODO: The currently described steps skips over
- Keycloak realm import
- Client secret extraction from Keycloak and storing it with `dotnet user-secrets set "Keycloak:ClientSecret" "SECRET" --id "1625b1eb-47fe-43d5-bdcd-c89d1e43e56b"`

TODO: Describe how to develop against an online test environment instead of running 4 docker containers
