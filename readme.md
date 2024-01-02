# Distributed Pixel Board

An educational project used in distributed systems classes.

The intended audience knows a bit of C# and ASP.NET Core.
General programming basics are a requirement, too.

Teams of students implement a client application to read and modify a shared
pixel board. Most of the course is done in local development, without the shared
state. But the end goal is to play an interactive mini game on the pixel board
where each team tries to score points by drawing specific shapes in their team's
color while preventing other teams to draw theirs. A functioning client is
essential to be competitive.

## Concepts

- REST API basics
- Local development practices
- Publish / subscribe
- OAuth

## Tech Stack

For students:

- ASP.NET Core
- Redis (client view only)

Additional tech teachers must have familiarity:

- Keycloak as the OAuth provider
- Redis read-only replica for direct
- Manage (public) hosting of shared resources on your platform of choice
