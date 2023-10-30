using Microsoft.Extensions.DependencyInjection;

namespace Ri.Interview;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceProvider = ServiceRegistration.RegisterServices();

        var applicationRunner = serviceProvider.GetService<ApplicationRunner>();
        
        
        await applicationRunner.Run();
    }
}

