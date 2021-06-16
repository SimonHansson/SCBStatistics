using Autofac.Extras.Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCB.BL;
using SCB.Interfaces;
using System;
using System.Threading.Tasks;

namespace SCBTest.BL
{
    [TestClass]
    public class ElectionTurnoutServiceTest
    {
        private AutoMock _mock;
        private IElectionTurnoutService _electionTurnoutService;

        [TestInitialize]
        public void TestInitialize()
        {
            _mock = AutoMock.GetLoose();
            _electionTurnoutService = _mock.Create<ElectionTurnoutService>();
        }

        [TestMethod]
        public async Task DisplayElectionTurnout_ResponseIsNull_ShouldThrowInvalidOperationException()
        {
            // Act & assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => _electionTurnoutService.DisplayElectionTurnout());
        }

    }
}
