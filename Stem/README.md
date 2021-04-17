# Stem Project

## Setup

Put the following configuration to `Constants.cs`, this way we bake in credentials in build time.
It is more secure than keeping a plain file somewhere.

```c#
namespace Stem
{
    static class Constants
    {
        public const string DbName = "main";
        public const string DbHost = "127.0.0.1";
        public const string DbPort = "5433";
        public const string DbMasterUser = "postgres";
        public const string DbMasterPassword = "postgres";
        public const string DbUserPasswordPrefix = "prefix"
        public const string MigrationFolder = "C:\\Projects\\Stem\\Migrations123";
    }
}
```

## Database Credentials

Users access database as restricted role. They have following credentials:

```
role: pid
password: DbUserPasswordPrefix + random_uuid
```

This way credentials are half-baked into the database.
