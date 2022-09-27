
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
var connectionString = configuration.GetConnectionString("ConnectionString");