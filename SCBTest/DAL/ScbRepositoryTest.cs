using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Refit;
using SCB.DAL;
using SCB.Interfaces;
using SCB.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SCBTest.DAL
{
    [TestClass]
    public class ScbRepositoryTest
    {
        private AutoMock _mock;
        private IScbRepository _scbRepository;

        [TestInitialize]
        public void TestInitialize()
        {
            _mock = AutoMock.GetLoose();
            _scbRepository = _mock.Create<ScbRepository>();
            _mock.VerifyAll = true;
        }

        [TestMethod]
        public async Task GetElectionTurnoutStatistics_ContentIsNull_ShouldThrowArgumentException()
        {
            // Arrange
            var refitSettings = new RefitSettings();

            var response = new ApiResponse<ScbRegionsResponseModel>(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null
            },
            new ScbRegionsResponseModel(),
            refitSettings
            );

            _mock.Mock<IScbApi>()
                .Setup(x => x.GetElectionTurnoutRegionsCodeAndYears(default))
                .ReturnsAsync(response);

            // Act & assert 
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => _scbRepository.GetElectionTurnoutStatistics(default));
        }

    }
}
