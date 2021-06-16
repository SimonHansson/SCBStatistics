using SCB.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCB.BL
{
    /// <summary>
    /// Service for displaying election turnout result
    /// </summary>
    public class ElectionTurnoutService: IElectionTurnoutService
    {
        private readonly IScbRepository _scbRepository;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="scbRepository">scb repository reference</param>
        /// <exception cref="ArgumentNullException">Thrown if any dependency injection parameter is null</exception>
        public ElectionTurnoutService(IScbRepository scbRepository)
        {
            _scbRepository = scbRepository ?? throw new ArgumentNullException(nameof(scbRepository)); ;
        }

        /// <summary>
        /// Get election result from repository and show it on console
        /// </summary>
        /// <param name="cancellationToken">Token to cancel long-running operation</param>
        /// <exception cref="InvalidOperationException">Thrown if repository returns null.</exception>
        public async Task DisplayElectionTurnout(CancellationToken cancellationToken)
        {
            var electionTurnouts = await _scbRepository.GetElectionTurnoutStatistics(cancellationToken);

            if (electionTurnouts is null)
            {
                throw new InvalidOperationException($"Could not get election turnouts from repository.");

            }

            foreach (var turnout in electionTurnouts)
            {
                Console.WriteLine($"{turnout.Year} {turnout.RegionName} {turnout.Turnout}%");
            }
        }

    }
}
