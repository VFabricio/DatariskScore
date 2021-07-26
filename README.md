# Platforms supported

There is a Docker Compose file to spin up the development environment. This is
supported in any environment with Docker and Docker Compose.

The database migration scripts required bash, other standard POSIX utilities and
psql. It was tested in the latest Arch Linux, but should work on most Linux
distributions and MacOS, provided psql is installed. Windows is unsupported.
It might be possible to run the scripts in Windows with WSL, but this has not
been tested.

To develop the application in bare-metal, the .NET 5.0 SDK is required

# Commands

To spin up the development environment (server plus database) run
```
docker-compose up
```
The application will start listening to HTTP traffic in port 5000.
At this point it is alreadly possible to send it requests, but if they require
communication with the database they will fail with status code 5000, because
the the database and score table have not yet been created. To create them,
run
```
DatariskScore/Scripts/initialize-database.sh
```

# ToDo

There several improvements that I would've liked to implement but havent't had
the time yet. In order of priority they are:
- the database access information is hardcoded and has to be kept manually
  in sync in three places: the application itself, docker-compose.yaml and the
  migration script. The correct solution would be to handle this with environment
  variables. Not only would this improve mantainability, it would also make it
  easier to connect the application with a production database. Right now doing
  it would require editing application code, which is a bad practice.
- refactor the Score.Controller module. Since this is a very simple application,
  there is little domain logic and most of the complexity is in marshalling data
  from HTTP-specific constructs to our domain model and back again. This makes
  this module the largest one in the system. It could be organized better and
  functions could be given better names.
- implement CPF hashing
- our integration tests run against the actual development database. To ensure
  isolation, each one of them test creates a new database. This is working well
  and runs fast enough, but our volume gets littered with lots of near empty
  databases that will never be used again. There should be an automated way of
  cleaning this up.
- move the database migrations to F#. F# is a decent enough scripting language
  and it would be better to use it than required bash.
