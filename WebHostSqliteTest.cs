using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Xunit;
using static Xmu.Crms.Insomnia.XUnitTest.Utils;

namespace Xmu.Crms.Insomnia.XUnitTest
{
    public class WebHostSqliteTest
    {
        [Fact]
        public async Task CanGetEntityById()
        {
            var basePath = GetProjectPath(Assembly.GetExecutingAssembly());
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            try
            {
                var server = (await connection.PopulateDbAsync(basePath)).MakeTestServer(basePath);
                using (var scope = server.Host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var db = services.GetRequiredService<CrmsContext>();
                    Assert.Single(await db.Attendences.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.ClassInfo.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.Course.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.CourseSelection.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.FixGroup.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.FixGroupMember.Where(a => a.Id == 1).ToListAsync());
                    Assert.True((await db.Location.Where(a => a.Id == 1).ToListAsync()).Count <= 1);
                    Assert.Single(await db.Seminar.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.SeminarGroup.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.SeminarGroupMember.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.SeminarGroupTopic.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.StudentScoreGroup.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.Topic.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.UserInfo.Where(a => a.Id == 1).ToListAsync());
                    Assert.Single(await db.UserInfo.Where(a => a.Id == 3).ToListAsync());
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task CanCallSeminarGroup()
        {
            var basePath = GetProjectPath(Assembly.GetExecutingAssembly());
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            try
            {
                var server = (await connection.PopulateDbAsync(basePath)).MakeTestServer(basePath);
                using (var scope = server.Host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var sg = services.GetRequiredService<ISeminarGroupService>();
                    Assert.NotNull(sg.GetSeminarGroupByGroupId(1));
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Fact]
        public async Task CanCallFixGroup()
        {
            var basePath = GetProjectPath(Assembly.GetExecutingAssembly());
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            try
            {
                var server = (await connection.PopulateDbAsync(basePath)).MakeTestServer(basePath);
                using (var scope = server.Host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var fg = services.GetRequiredService<IFixGroupService>();
                    fg.GetFixedGroupById(1, 1);
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}