using Moq;
using MythApi.Common.Database.Models;
using MythApi.Endpoints.v1;
using MythApi.Mythologies.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UnitTests
{
    public class MythologyEndpointsTests
    {
        private Mock<IMythologyRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IMythologyRepository>();
        }

        [Test]
        public async Task GetAllMythologies_ShouldReturnAllMythologies()
        {
            // Arrange
            var mythologies = new List<Mythology>
            {
                new Mythology { Id = 1, Name = "Greek" },
                new Mythology { Id = 2, Name = "Norse" }
            };
            _mockRepository.Setup(repo => repo.GetAllMythologiesAsync()).ReturnsAsync(mythologies);

            // Act
            var result = await Mythologies.GetAllMythologies(_mockRepository.Object);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Greek"));
            Assert.That(result[1].Name, Is.EqualTo("Norse"));
        }

        [Test]
        public async Task GetMythologyById_ShouldReturnMythology_WhenMythologyExists()
        {
            // Arrange
            var mythology = new Mythology 
            { 
                Id = 1, 
                Name = "Greek",
                Gods = new List<God>
                {
                    new God { Id = 1, Name = "Zeus", MythologyId = 1, Description = "God of the sky" }
                }
            };
            _mockRepository.Setup(repo => repo.GetMythologyByIdAsync(1)).ReturnsAsync(mythology);

            // Act
            var result = await Mythologies.GetMythologyById(1, _mockRepository.Object);

            // Assert
            var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<Mythology>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value.Id, Is.EqualTo(1));
            Assert.That(okResult.Value.Name, Is.EqualTo("Greek"));
            Assert.That(okResult.Value.Gods.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetMythologyById_ShouldReturnNotFound_WhenMythologyDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetMythologyByIdAsync(999)).ReturnsAsync((Mythology?)null);

            // Act
            var result = await Mythologies.GetMythologyById(999, _mockRepository.Object);

            // Assert
            var notFoundResult = result as Microsoft.AspNetCore.Http.HttpResults.NotFound;
            Assert.That(notFoundResult, Is.Not.Null);
        }

        [Test]
        public async Task GetMythologyById_ShouldReturnNotFound_WhenIdIsZero()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetMythologyByIdAsync(0)).ReturnsAsync((Mythology?)null);

            // Act
            var result = await Mythologies.GetMythologyById(0, _mockRepository.Object);

            // Assert
            var notFoundResult = result as Microsoft.AspNetCore.Http.HttpResults.NotFound;
            Assert.That(notFoundResult, Is.Not.Null);
        }

        [Test]
        public async Task GetMythologyById_ShouldReturnNotFound_WhenIdIsNegative()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetMythologyByIdAsync(-1)).ReturnsAsync((Mythology?)null);

            // Act
            var result = await Mythologies.GetMythologyById(-1, _mockRepository.Object);

            // Assert
            var notFoundResult = result as Microsoft.AspNetCore.Http.HttpResults.NotFound;
            Assert.That(notFoundResult, Is.Not.Null);
        }

        [Test]
        public async Task GetMythologyById_ShouldReturnMythology_WhenMythologyHasNoGods()
        {
            // Arrange
            var mythology = new Mythology 
            { 
                Id = 1, 
                Name = "Greek",
                Gods = new List<God>()
            };
            _mockRepository.Setup(repo => repo.GetMythologyByIdAsync(1)).ReturnsAsync(mythology);

            // Act
            var result = await Mythologies.GetMythologyById(1, _mockRepository.Object);

            // Assert
            var okResult = result as Microsoft.AspNetCore.Http.HttpResults.Ok<Mythology>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value.Id, Is.EqualTo(1));
            Assert.That(okResult.Value.Gods.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetMythologyById_ShouldReturnNotFound_WhenIdIsMaxValue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetMythologyByIdAsync(int.MaxValue)).ReturnsAsync((Mythology?)null);

            // Act
            var result = await Mythologies.GetMythologyById(int.MaxValue, _mockRepository.Object);

            // Assert
            var notFoundResult = result as Microsoft.AspNetCore.Http.HttpResults.NotFound;
            Assert.That(notFoundResult, Is.Not.Null);
        }
    }
}
