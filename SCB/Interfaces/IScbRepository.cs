using SCB.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCB.Interfaces
{
    /// <summary>
    /// Interface describing ScbRepository.
    /// </summary>
    public interface IScbRepository
    {
        /// <summary>
        /// Gets election turnaout statistics
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>List of <see cref="ElectionTurnoutModel"/> object</returns>
        Task<List<ElectionTurnoutModel>> GetElectionTurnoutStatistics(CancellationToken cancellationToken);
    }
}
