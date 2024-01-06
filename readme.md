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
