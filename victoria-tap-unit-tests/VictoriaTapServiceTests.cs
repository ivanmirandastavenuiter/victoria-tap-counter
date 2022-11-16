using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using victoria_tap.Entities;
using victoria_tap.Providers.Contracts;
using victoria_tap.Repositories;
using victoria_tap.Repositories.Contracts;
using victoria_tap.Services;
using victoria_tap.Services.Contracts;
using victoria_tap.Shared;
using Xunit;

namespace victoria_tap_unit_tests
{
    public class VictoriaTapServiceTests
    {
        private readonly IFixture _fixture;
        private readonly Mock<IVictoriaTapRepository> _victoriaTapRepository;
        private readonly Mock<ISpendingInfoCalculator> _spendingInfoCalculator;

        public VictoriaTapServiceTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _victoriaTapRepository = _fixture.Freeze<Mock<IVictoriaTapRepository>>();
            _spendingInfoCalculator = _fixture.Freeze<Mock<ISpendingInfoCalculator>>();
        }

        [Fact]
        public void CreateDispenser_WhenInvoked_ShouldReturnNewDispenserObject()
        {
            // Arrange
            var input = _fixture.Create<float>();
            var dispenser = _fixture.Create<Dispenser>();
            dispenser.FlowVolume = input;

            _victoriaTapRepository.Setup(x => x.CreateDispenser(input))
                                  .Returns(dispenser);

            var service = _fixture.Create<VictoriaTapService>();

            // Act
            var result = service.CreateDispenser(input);

            // Assert
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().NotBeNull();
            result.Data.FlowVolume.Should().BeGreaterThan(0);
            result.Data.FlowVolume.Should().Be(input);
            result.Data.Should().BeOfType(typeof(CreateDispenserResponse));

            _victoriaTapRepository.Verify(x => x.CreateDispenser(input), Times.Once);
        }

        [Fact]
        public void ChangeDispenserStatus_WhenInvokedWithCorrectConfiguration_ShouldChangeDispenserStatus()
        {
            // Arrange
            var dispenser = _fixture.Build<Dispenser>()
                                    .With(x => x.Status, DispenserStatus.Closed)
                                    .Create();

            var status = "open";
            var updatedAt = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var dispenserId = dispenser.Id;

            _victoriaTapRepository.Setup(x => x.GetDispenserById(dispenserId))
                                  .Returns(dispenser);

            _victoriaTapRepository.Setup(x => x.ChangeDispenserStatus(dispenser))
                                  .Returns(true);

            _victoriaTapRepository.Setup(x => x.GetDispenserInfoById(dispenserId))
                                  .Returns(() => null);

            _victoriaTapRepository.Setup(x => x.AddDispenserSpendingInfoItem(It.IsAny<DispenserInfo>()));

            var service = _fixture.Create<VictoriaTapService>();

            // Act
            var result = service.ChangeDispenserStatus(status, updatedAt, dispenserId);

            // Assert
            result.Data.Should().BeTrue();

            dispenser.Status.Should().Be(DispenserStatus.Open);

            _victoriaTapRepository.Verify(x => x.GetDispenserById(dispenserId), Times.Once);
            _victoriaTapRepository.Verify(x => x.ChangeDispenserStatus(dispenser), Times.Once);
            _victoriaTapRepository.Verify(x => x.GetDispenserInfoById(dispenserId), Times.Once);
            _victoriaTapRepository.Verify(x => x.AddDispenserSpendingInfoItem(It.IsAny<DispenserInfo>()), Times.Once);
        }

        [Fact]
        public void ChangeDispenserStatus_WhenClosingOpenedTap_ShouldCloseTapAndSaveDispenserSpendingInfo()
        {
            // Arrange
            var dispenser = _fixture.Build<Dispenser>()
                                    .With(x => x.Status, DispenserStatus.Open)
                                    .Create();

            var status = "closed";
            var updatedAt = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var dispenserId = dispenser.Id;

            var dispenserInfo = _fixture.Create<DispenserInfo>();

            dispenserInfo.DispenserId = dispenserId;
            dispenserInfo.Amount = 0;
            dispenserInfo.Usages = new List<Usages>()
            {
                new Usages()
                {
                    FlowVolume = dispenser.FlowVolume,
                    OpenedAt = DateTimeOffset.Now.AddSeconds(-5).ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            };

            _victoriaTapRepository.Setup(x => x.GetDispenserById(dispenserId))
                                  .Returns(dispenser);

            _victoriaTapRepository.Setup(x => x.ChangeDispenserStatus(dispenser))
                                  .Returns(true);

            _victoriaTapRepository.Setup(x => x.GetDispenserInfoById(dispenserId))
                                  .Returns(() => dispenserInfo);

            _spendingInfoCalculator.Setup(x => x.CalculateClosedTapAmount(dispenser, dispenserInfo))
                                   .Returns(dispenserInfo);

            _victoriaTapRepository.Setup(x => x.UpdateDispenserSpendingInfoItem(dispenserInfo));

            var service = _fixture.Create<VictoriaTapService>();

            // Act
            var result = service.ChangeDispenserStatus(status, updatedAt, dispenserId);

            // Assert
            result.Data.Should().BeTrue();

            dispenser.Status.Should().Be(DispenserStatus.Closed);

            _victoriaTapRepository.Verify(x => x.GetDispenserById(dispenserId), Times.Once);
            _victoriaTapRepository.Verify(x => x.ChangeDispenserStatus(dispenser), Times.Once);
            _victoriaTapRepository.Verify(x => x.GetDispenserInfoById(dispenserId), Times.Once);
            _spendingInfoCalculator.Verify(x => x.CalculateClosedTapAmount(dispenser, dispenserInfo), Times.Once);
            _victoriaTapRepository.Verify(x => x.UpdateDispenserSpendingInfoItem(dispenserInfo), Times.Once);
        }

        [Fact]
        public void ChangeDispenserStatus_WhenOpeningNewTap_ShouldAddNewDispenserSpendingInfoItem()
        {
            // Arrange
            var dispenser = _fixture.Build<Dispenser>()
                                    .With(x => x.Status, DispenserStatus.Closed)
                                    .Create();

            var status = "open";
            var updatedAt = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var dispenserId = dispenser.Id;

            var dispenserInfo = _fixture.Create<DispenserInfo>();

            dispenserInfo.DispenserId = dispenserId;
            dispenserInfo.Amount = 20.4;
            dispenserInfo.Usages = new List<Usages>()
            {
                new Usages()
                {
                    FlowVolume = dispenser.FlowVolume,
                    OpenedAt = DateTimeOffset.Now.AddSeconds(-5).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ClosedAt = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    TotalSpent = 20.4
                }
            };

            _victoriaTapRepository.Setup(x => x.GetDispenserById(dispenserId))
                                  .Returns(dispenser);

            _victoriaTapRepository.Setup(x => x.ChangeDispenserStatus(dispenser))
                                  .Returns(true);

            _victoriaTapRepository.Setup(x => x.GetDispenserInfoById(dispenserId))
                                  .Returns(() => dispenserInfo);

            _victoriaTapRepository.Setup(x => x.UpdateDispenserSpendingInfoItem(dispenserInfo));

            var service = _fixture.Create<VictoriaTapService>();

            // Act
            var result = service.ChangeDispenserStatus(status, updatedAt, dispenserId);

            // Assert
            result.Data.Should().BeTrue();

            dispenser.Status.Should().Be(DispenserStatus.Open);
            dispenserInfo.Usages.Should().HaveCount(2);
            dispenserInfo.Usages.Last().ClosedAt.Should().BeNull();
            dispenserInfo.Usages.Last().FlowVolume.Should().Be(dispenser.FlowVolume);

            _victoriaTapRepository.Verify(x => x.GetDispenserById(dispenserId), Times.Once);
            _victoriaTapRepository.Verify(x => x.ChangeDispenserStatus(dispenser), Times.Once);
            _victoriaTapRepository.Verify(x => x.GetDispenserInfoById(dispenserId), Times.Once);
            _victoriaTapRepository.Verify(x => x.UpdateDispenserSpendingInfoItem(dispenserInfo), Times.Once);
        }

        [Fact]
        public void ChangeDispenserStatus_WhenInvokedWithIncorrectStatus_ShouldReturnFalse()
        {
            // Arrange
            var dispenser = _fixture.Build<Dispenser>()
                                    .With(x => x.Status, DispenserStatus.Open)
                                    .Create();

            var status = "open";
            var updatedAt = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var dispenserId = dispenser.Id;

            _victoriaTapRepository.Setup(x => x.GetDispenserById(dispenserId))
                                  .Returns(dispenser);

            _victoriaTapRepository.Setup(x => x.ChangeDispenserStatus(dispenser))
                                  .Returns(true);

            _victoriaTapRepository.Setup(x => x.GetDispenserInfoById(dispenserId))
                                  .Returns(() => null);

            _victoriaTapRepository.Setup(x => x.AddDispenserSpendingInfoItem(It.IsAny<DispenserInfo>()));

            var service = _fixture.Create<VictoriaTapService>();

            // Act
            var result = service.ChangeDispenserStatus(status, updatedAt, dispenserId);

            // Assert
            result.Data.Should().BeFalse();

            _victoriaTapRepository.Verify(x => x.GetDispenserById(dispenserId), Times.Once);
            _victoriaTapRepository.Verify(x => x.ChangeDispenserStatus(dispenser), Times.Never);
            _victoriaTapRepository.Verify(x => x.GetDispenserInfoById(dispenserId), Times.Never);
            _victoriaTapRepository.Verify(x => x.AddDispenserSpendingInfoItem(It.IsAny<DispenserInfo>()), Times.Never);
        }
    }
}