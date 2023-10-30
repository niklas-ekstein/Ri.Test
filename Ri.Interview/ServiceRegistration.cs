using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Ri.Interview.Data;
using Ri.Interview.Data.Repositories;
using Ri.Interview.Interfaces;
using Ri.Interview.Services;

namespace Ri.Interview;

public class ServiceRegistration
{
        public static ServiceProvider RegisterServices()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            
            return new ServiceCollection()
                .AddTransient<IAccountService, AccountService>()
                .AddTransient<IConsoleWrapperService, ConsoleWrapperService>()
                .AddTransient<ILoginService, LoginService>()
                .AddTransient<IProjectService, ProjectService>()
                .AddTransient<ITeamService, TeamService>()
                .AddTransient<IProjectRepository, ProjectRepository>()
                .AddTransient<ApplicationRunner>()
                .Configure<FormioSettings>(configuration.GetSection("FormioSettings"))
                .AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("Default")))
                .BuildServiceProvider();
        }
}