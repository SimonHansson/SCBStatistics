using SCB.Constants;
using SCB.Interfaces;
using SCB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCB.DAL
{
    /// <summary>
    /// Repository for handel trunout statistics from SCB api
    /// </summary>
    public class ScbRepository: IScbRepository
    {
        private string _responseTimeCode = "Tid";
        private readonly IScbApi _scbApi;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="scbApi">Scb Refit interface reference</param>
        /// <exception cref="ArgumentNullException">Thrown if any dependency injection parameter is null</exception>
        public ScbRepository(IScbApi scbApi)
        {
            _scbApi = scbApi ?? throw new ArgumentNullException(nameof(scbApi));
        }

        /// <summary>
        /// Get and handels election turnout statistics from SCB api
        /// </summary>
        /// <param name="cancellationToken">Token to cancel long-running operation</param>
        /// <returns>List of <see cref="ElectionTurnoutModel"/></returns>
        public async Task<List<ElectionTurnoutModel>> GetElectionTurnoutStatistics(CancellationToken cancellationToken)
        {
            // Get region names and code from scb api
            var getResponse = await _scbApi.GetElectionTurnoutRegionsCodeAndYears(cancellationToken);
            
            if (!getResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to retrieve election region code, name and election years from SCB");
                return null;
            }

            var regions = GetRegionsNameAndId(getResponse.Content);
            if (regions is null)
            {
                return null;
            }

            var electionYears = GetAllElectionYears(getResponse.Content);

            var requestModel = CreateRequestModel(regions);

            // Get turnout statistics for all regions and election from svb api
            var postResponse = await _scbApi.GetElectionTurnoutStatistics(requestModel, cancellationToken);

            if (!postResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to retrieve election statistics from SCB");
                return null;
            }

            if (postResponse.Content is null)
            {
				throw new ArgumentException($"'{nameof(postResponse.Content)}' cannot be null.");
            }

            if (postResponse?.Content.data is null)
            {
                throw new ArgumentException($"'{nameof(postResponse.Content.data)}' cannot be null.");
            }

            var electionTrunoutResults = new List<ElectionTurnoutModel>();
            foreach (var electionRes in postResponse.Content.data)
            {
                var electionTurnout = new ElectionTurnoutModel
                {
                    Identifier = electionRes.key[0],
                    Year = electionRes.key[1],
                    Turnout = electionRes.values[0]
                };
                electionTrunoutResults.Add(electionTurnout);
            }

            var highestTurnouts = new List<ElectionTurnoutModel>();

            foreach (var year in electionYears)
            {
                var electionTrunout = electionTrunoutResults.FindAll(y => y.Year == year).OrderByDescending(x => x.Turnout).FirstOrDefault();

                electionTrunout.RegionName = regions.Find(region => region.Identifier == electionTrunout.Identifier).Name;

                highestTurnouts.Add(electionTrunout);
            };

            return highestTurnouts;
        }

        /// <summary>
        /// Get regions and regions code.
        /// </summary>
        /// <param name="resModel">response model from scb request</param>
        /// <returns>List of <see cref="RegionModel"/></returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="resModel"/> is null</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="resModel.variables"/> is null</exception>
        public List<RegionModel> GetRegionsNameAndId(ScbRegionsResponseModel resModel)
        {
            if (resModel is null)
            {
                throw new ArgumentNullException($"'{nameof(resModel)}' cannot be null.");
            }

            if (resModel.variables is null)
            {
                throw new ArgumentException($"'{nameof(resModel.variables)}' cannot be null.");
            }

            var regions = new List<RegionModel>();

            var variable = resModel.variables.FirstOrDefault(v => String.Compare(v.code, "region", StringComparison.InvariantCultureIgnoreCase) == 0);

            if (variable.values.Count() != variable.valueTexts.Count())
            {
                Console.WriteLine($"'{nameof(variable.values)}' and '{nameof(variable.valueTexts)}' don't match, data recieved is incomplete.");
                return null;
            }

            // Create object with value and valuetext lists, they match by having same index. 
            var index = 0;
            foreach (var value in variable.values)
            {
                var region = new RegionModel
                {
                    Identifier = value,
                    Name = variable.valueTexts[index] 
                };
               index++;
               regions.Add(region);
            }

            regions.RemoveAt(0); // Remove first object due it's country, not region as default.

            return regions;
        }

        /// <summary>
        /// Create request model.
        /// </summary>
        /// <param name="regions"> List of regions <see cref="RegionModel"/></param>
        /// <returns><see cref="ScbElectionTurnoutStatisticRequestModel"/></returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="regions"/> is null</exception>
        private ScbElectionTurnoutStatisticRequestModel CreateRequestModel(List<RegionModel> regions)
        {
            if (regions is null)
            {
                throw new ArgumentNullException($"'{nameof(regions)}' cannot be null.");
            }

            var requestModel = new ScbElectionTurnoutStatisticRequestModel();
            requestModel.Query = new List<Query>();

            List<string> values = regions.Select(region => region.Identifier).ToList();

            var regionsSelection = new Selection
            {
                Filter = ScbRequestConstants.RegionFilter,
                Values = values,
            };

            var regionsQuary = new Query
            {
                Code = ScbRequestConstants.RegionCode,
                Selection = regionsSelection
            };

            requestModel.Query.Add(regionsQuary);

            var contentSelection = new Selection
            {
                Filter = ScbRequestConstants.ContentSelectionFilter,
                Values = new List<string>() { ScbRequestConstants.ContentSelectionValue }
            };

            var contentQuary = new Query
            {
                Code = ScbRequestConstants.ContentQueryCode,
                Selection = contentSelection
            };

            requestModel.Query.Add(contentQuary);

            var response = new Response
            {
                Format = ScbRequestConstants.ResponseFormatJson
            };

            requestModel.Response = response;

            return requestModel;
        }

        /// <summary>
        /// Gets all election years
        /// </summary>
        /// <param name="resModel"><see cref="ScbRegionsResponseModel"/></param>
        /// <returns>List of all election years as list of strings"/></returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="resModel"/> is null</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="resModel.varibles"/> is null</exception>
        private List<string> GetAllElectionYears(ScbRegionsResponseModel resModel)
        {
            if (resModel is null)
            {
                throw new ArgumentNullException($"'{nameof(resModel)}' cannot be null.");
            }

            if (resModel.variables is null)
            {
                throw new ArgumentException($"'{nameof(resModel.variables)}' cannot be null.");
            }

            var electionYears = resModel.variables.FirstOrDefault(v => String.Compare(v.code, _responseTimeCode, StringComparison.InvariantCultureIgnoreCase) == 0).values;

            return electionYears;
        }

    }
}
