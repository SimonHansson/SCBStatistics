using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SCB.BL;
using SCB.Constants;
using SCB.DAL;
using SCB.Extensions;
using SCB.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCB
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var cancelTokenSource = new CancellationTokenSource();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var serviceCollection = new ServiceCollection();

            ConfigureServices(serviceCollection, configuration);

            serviceCollection.RegisterRefitApi<IScbApi>(configuration[ConfigurationKeys.ScbApiEndpoint]);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            await MainMenu(serviceProvider, cancelTokenSource.Token);
        }

        private static async Task MainMenu(ServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            Console.WriteLine();
            Console.WriteLine("SCB statistics Application.");
            Console.WriteLine("============================");
            Console.WriteLine();
            Console.WriteLine("Press any key to show statistics");
            Console.ReadKey();
            Console.WriteLine("============================");
            Console.WriteLine();

            var electionTurnoutService = serviceProvider.GetRequiredService<IElectionTurnoutService>();

            await electionTurnoutService.DisplayElectionTurnout(cancellationToken);

            Console.WriteLine("============================");
            Console.ReadLine();
        }

        private static void ConfigureServices(ServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton<IElectionTurnoutService, ElectionTurnoutService>()
                .AddScoped<IScbRepository, ScbRepository>();
        }

    }
}
