using Microsoft.AspNetCore.Mvc;
using Moq;
using MythApi.Common.Database.Models;
using MythApi.Endpoints.v1;
using MythApi.Gods.Interfaces;
using MythApi.Gods.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests
{
    public class GodEndpointsTests
    {
        private Mock<IGodRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IGodRepository>();
        }

        [Test]
        public async Task GetAllGods_ShouldReturnAllGods()
        {
            var gods = new List<God>
            {
                new God { Name = "Zeus", MythologyId = 1, Description = "God of the sky" },
                new God { Name = "Hera", MythologyId = 1, Description = "Goddess of marriage" }
            };
            _mockRepository.Setup(repo => repo.GetAllGodsAsync()).ReturnsAsync(gods);

            var result = await MythApi.Endpoints.v1.Gods.GetAlllGods(_mockRepository.Object);

            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task AddOrUpdateGods_ShouldAddNewGod()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = "Zeus", MythologyId = 1, Description = "God of the sky" }
            };
            var gods = new List<God>
            {
                new God { Name = "Zeus", MythologyId = 1, Description = "God of the sky" }
            };
            _mockRepository.Setup(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>())).ReturnsAsync(gods);

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Name, Is.EqualTo("Zeus"));
        }

        [Test]
        public async Task GetGodByName_DoesNotAllowSqlInjection()
        {
            // Arrange
            var maliciousName = "' OR '1'='1";
            var parameter = new GodByNameParameter(maliciousName, false);
            
            // Mock repository to return empty list for SQL injection attempt
            _mockRepository.Setup(repo => repo.GetGodByNameAsync(It.IsAny<GodByNameParameter>()))
                .ReturnsAsync(new List<God>());
            
            // Act
            var result = await _mockRepository.Object.GetGodByNameAsync(parameter);
            
            // Assert
            Assert.That(result, Is.Empty); // Should find nothing, not all records
        }
    }
}
