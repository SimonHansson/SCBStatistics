using Refit;
using SCB.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SCB.Interfaces
{
	/// <summary>
	/// Refit interface for Scb api
	/// </summary>
	[Headers("Content-Type: application/json")]
	public interface IScbApi
	{
		/// <summary>
		/// Get Scb electionTurnout regions and region code.
		/// </summary>
		/// <param name="cancellationToken">Token for cancelling long running task</param>
		/// <returns><see cref="IApiResponse{ScbResponseModel}"/></returns>
		[Get("/OV0104/v1/doris/sv/ssd/START/ME/ME0104/ME0104D/ME0104T4")]
		Task<IApiResponse<ScbRegionsResponseModel>> GetElectionTurnoutRegionsCodeAndYears(CancellationToken cancellationToken);

		/// <summary>
		/// Get Scb electionTurnout statistics.
		/// </summary>
		/// <param name="cancellationToken">Token for cancelling long running task</param>
		/// <returns><see cref="IApiResponse{ScbStatisticsResponseModel}"/></returns>
		[Post("/OV0104/v1/doris/sv/ssd/START/ME/ME0104/ME0104D/ME0104T4")]
        Task<IApiResponse<ScbStatisticsResponseModel>> GetElectionTurnoutStatistics([Body]ScbElectionTurnoutStatisticRequestModel requestModel, CancellationToken cancellationToken);
    }
}
