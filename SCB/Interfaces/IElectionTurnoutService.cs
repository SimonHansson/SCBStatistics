using System.Threading;
using System.Threading.Tasks;

namespace SCB.Interfaces
{
    /// <summary>
    /// Interface describing Election turnout service.
    /// </summary>
    public interface IElectionTurnoutService
    {
        /// <summary>
        /// Displays election turnout on console
        /// </summary>
        /// <param name="cancellationToken">Token to cancel long-running operation</param>
        Task DisplayElectionTurnout(CancellationToken cancellationToken = default);
    }
}
