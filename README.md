# epr-packaging-data-api

Core delivery C# ASP.NET backend template.

* [Install MongoDB](#install-mongodb)
* [Inspect MongoDB](#inspect-mongodb)
* [Testing](#testing)
* [Running](#running)
* [Dependabot](#dependabot)


### Docker Compose

A Docker Compose template is in [compose.yml](compose.yml).

A local environment with:

- Localstack for AWS services (S3, SQS)
- Redis
- MongoDB
- This service.
- A commented out frontend example.

```bash
docker compose up --build -d
```

A more extensive setup is available in [github.com/DEFRA/cdp-local-environment](https://github.com/DEFRA/cdp-local-environment)

### MongoDB

#### MongoDB via Docker

See above.

```
docker compose up -d mongodb
```

#### MongoDB locally

Alternatively install MongoDB locally:

- Install [MongoDB](https://www.mongodb.com/docs/manual/tutorial/#installation) on your local machine
- Start MongoDB:
```bash
sudo mongod --dbpath ~/mongodb-cdp
```

#### MongoDB in CDP environments

In CDP environments a MongoDB instance is already set up
and the credentials exposed as enviromment variables.


### Inspect MongoDB

To inspect the Database and Collections locally:
```bash
mongosh
```

You can use the CDP Terminal to access the environments' MongoDB.

### Testing

Tests run a full `WebApplication` through `WebApplicationFactory`, so routing, validation and serialisation
are exercised for real. Persistence is substituted with an NSubstitute mock, so the tests do not need a
running MongoDB.

```bash
dotnet test
```

Run a single test class or method with a filter:

```bash
dotnet test --filter "FullyQualifiedName~ExampleEndpointsTest"
dotnet test --filter "FullyQualifiedName~Health_endpoint_is_available"
```

### Running

Run the application:
```bash
dotnet run --project EprPackagingDataApi
```

This uses the `EprPackagingDataApi` launch profile, listens on http://localhost:8085, and expects MongoDB
on `127.0.0.1:27017`. Start one with `docker compose up mongodb -d` if you do not have a local install.

### NuGet sources

[NuGet.config](NuGet.config) pins restore to nuget.org and clears inherited sources. If you have DEFRA's
private Azure DevOps feed configured at machine level for the Azure-hosted EPR services, leaving it in scope
makes `dotnet restore` fail with `NU1301` after several minutes, because that feed returns 401 without a PAT.
This service takes no dependency on it.

### SonarCloud

Example SonarCloud configuration are available in the GitHub Action workflows.

### Dependabot

We have added an example dependabot configuration file to the repository. You can enable it by renaming
the [.github/example.dependabot.yml](.github/example.dependabot.yml) to `.github/dependabot.yml`


### About the licence

The Open Government Licence (OGL) was developed by the Controller of Her Majesty's Stationery Office (HMSO) to enable
information providers in the public sector to license the use and re-use of their information under a common open
licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.
