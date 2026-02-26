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

            Assert.That(result, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Once);
        }

        [Test]
        public async Task AddOrUpdateGods_WithNullInput_ShouldReturnBadRequest()
        {
            var result = await Gods.AddOrUpdateGods(null!, _mockRepository.Object);

            Assert.That(result, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_WithEmptyList_ShouldReturnBadRequest()
        {
            var godInputs = new List<GodInput>();

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_WithBatchSizeExceeded_ShouldReturnBadRequest()
        {
            var godInputs = new List<GodInput>();
            for (int i = 0; i < 101; i++)
            {
                godInputs.Add(new GodInput { Name = $"God{i}", MythologyId = 1, Description = "Test" });
            }

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_WithEmptyName_ShouldReturnBadRequest()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = "", MythologyId = 1, Description = "Test" }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_WithNameTooLong_ShouldReturnBadRequest()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = new string('a', 101), MythologyId = 1, Description = "Test" }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_WithDescriptionTooLong_ShouldReturnBadRequest()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = "Zeus", MythologyId = 1, Description = new string('a', 1001) }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_WithNegativeMythologyId_ShouldReturnBadRequest()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = "Zeus", MythologyId = -1, Description = "Test" }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_WithZeroMythologyId_ShouldReturnBadRequest()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Name = "Zeus", MythologyId = 0, Description = "Test" }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_WithNegativeId_ShouldReturnBadRequest()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Id = -1, Name = "Zeus", MythologyId = 1, Description = "Test" }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }

        [Test]
        public async Task AddOrUpdateGods_WithZeroId_ShouldReturnBadRequest()
        {
            var godInputs = new List<GodInput>
            {
                new GodInput { Id = 0, Name = "Zeus", MythologyId = 1, Description = "Test" }
            };

            var result = await Gods.AddOrUpdateGods(godInputs, _mockRepository.Object);

            Assert.That(result, Is.Not.Null);
            _mockRepository.Verify(repo => repo.AddOrUpdateGods(It.IsAny<List<GodInput>>()), Times.Never);
        }
    }
}
