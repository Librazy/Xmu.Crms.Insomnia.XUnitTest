using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Xmu.Crms.Shared.Models;
using Xunit;

namespace Xmu.Crms.Insomnia.XUnitTest
{
    public class WebHostTest
    {
        [Fact]
        public async Task CanRunHost()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            try
            {
                var webHostBuilder = Program.CreateWebHostBuilder(new string[0]);
                webHostBuilder
                    .ConfigureServices(service => service.UseCrmsSqlite(connection))
                    .UseEnvironment("Development")
                    .UseContentRoot(GetProjectPath(Assembly.GetExecutingAssembly()));
                var server = new TestServer(webHostBuilder);
                using (var scope = server.Host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var db = services.GetRequiredService<CrmsContext>();
                    await db.SaveChangesAsync();
                }
            }
            finally
            {
                connection.Close();
            }
        }

        private static string GetProjectPath(Assembly startupAssembly)
        {
            //Get name of the target project which we want to test
            var projectName = startupAssembly.GetName().Name;

            //Get currently executing test project path
            var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;

            //Find the folder which contains the solution file. We then use this information to find the 
            //target project which we want to test
            DirectoryInfo directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                if (directoryInfo.Name == startupAssembly.GetName().Name)
                {
                    return directoryInfo.FullName;
                }
                directoryInfo = directoryInfo.Parent;
            }
            while (directoryInfo?.Parent != null);

            throw new Exception($"Solution root could not be located using application root {applicationBasePath}");

        }
    }
}