# Development Environment

## Setup

### Bookx Server

Requirements:

- .NET 8+
- [EF Core tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

The easiest way to pass environment variables is by using ``launchSettings.json`` and
``appsettings.json`` files. Following variables need to be set:

| Name                     | Description                                                                          |
|--------------------------|--------------------------------------------------------------------------------------|
|   ASPNETCORE_ENVIRONMENT | Enable debugging and development features of ASP.NET                                 |
| DB_CONNECTION_STRING     | Connection string for PostgreSQL instance                                            |
| PASSWORD_PEPPER          | Pepper that is used for password hashing                                             |
| JWT_SECURITY_KEY         | 384 bit key for JWT generation, you can generate keys [here](https://jwtsecret.com/generate) |
| JWT_ISSUER_DOMAIN        | Issuer of JWTs                                                                       |

Sample ``launchSettings.json`` in ``backend/BookxBackend/Properties`` path:

```json
{
  "profiles": {
    "Server": {
      "commandName": "Project",
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5001",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DB_CONNECTION_STRING": "User ID=YOUR_USER;Password=YOUR_PASSWORD;Host=localhost;Port=5432;Database=YOUR_DATABASE;Include Error Detail=true;",
        "PASSWORD_PEPPER": "YOUR_PEPPER",
        "JWT_SECURITY_KEY": "YOUR_KEY",
        "JWT_ISSUER_DOMAIN": "YOUR_DOMAIN"
      }
    }
  }
}
```

Sample ``appsettings.json`` (alternatively ``appsettings.Development.json`` in
``backend/BookxBackend`` path:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Kestrel": {
    "EndpointDefaults": {
      "Protocols": "Http1AndHttp2"
    }
  }
}
```

### PostgreSQL

You'll need a local PostgreSQL instance including a user with its own database schema. If you're
using an Arch based distro like me, you can follow these steps:

```sh
# install PostgreSQL
sudo pacman -S postgresql

# switch to postgres user
sudo su - postgres

# initialize database
initdb --locale en_US.UTF-8 -D /var/lib/postgres/data

# switch back to your user
exit

# start PostgreSQL service (add to autostart)
sudo systemctl enable --now postgresql

# enter psql mode as admin
sudo -u postgres psql

# create new database
create database bookx;

# create new user
create user bookx_user with encrypted password 'bookx_user';

# grant privileges
grant all privileges on database bookx to bookx_user;

# exit psql mode
\q
```

The connection string would look like this:

```sh
User ID=bookx_user;Password=bookx_user;Host=localhost;Port=5432;Database=bookx;
```

You can now apply migrations using the EF Core CLI tool:

```sh
dotnet ef database update
```

### Bookx Client

work in progress

## Debugging

### Manual

work in progress

### Tests

Create a ``default.env`` file with your environment variables for local development. I wasn't
able to pass variables directly by just running ``dotnet test``, so I created the little script
``run_tests.sh`` in the tests project. It simply sets all the variables in your current session
to then be used by the tests.

```env
ASPNETCORE_ENVIRONMENT="Development"
DB_CONNECTION_STRING="User ID=YOUR_USER;Password=YOUR_PASSWORD;Host=localhost;Port=5432;Database=YOUR_DATABASE;"
PASSWORD_PEPPER="YOUR_PEPPER"
JWT_SECURITY_KEY="YOUR_KEY"
JWT_ISSUER_DOMAIN="YOUR_DOMAIN"
```

Now just run ``./run_tests.sh``.
