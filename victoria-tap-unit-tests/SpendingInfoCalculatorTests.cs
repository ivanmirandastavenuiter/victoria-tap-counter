using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using victoria_tap.Entities;
using victoria_tap.Providers;
using victoria_tap.Shared;
using Xunit;

namespace victoria_tap_unit_tests
{
    public sealed class SpendingInfoCalculatorTests
    {
        private readonly IFixture _fixture;

        public SpendingInfoCalculatorTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [Fact]
        public void CalculateClosedTapAmount_WhenInvoked_ShouldReturnUpdatedDispenserSpendingInfo()
        {
            // Arrange
            var dispenser = _fixture.Build<Dispenser>()
                                    .With(x => x.FlowVolume, 0.064)
                                    .With(x => x.LastUpdatedAt, DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"))
                                    .Create();

            var dispenserInfo = _fixture.Create<DispenserInfo>();
            dispenserInfo.Amount = 0;
            dispenserInfo.DispenserId = dispenser.Id;

            var usages = new Usages()
            {
                FlowVolume = dispenser.FlowVolume,
                OpenedAt = DateTimeOffset.Now.AddSeconds(-10).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                ClosedAt = null,
                TotalSpent = 0
            };

            dispenserInfo.Usages = new List<Usages>() { usages };

            var spendingInfoCalculator = _fixture.Create<SpendingInfoCalculator>();

            // Act
            var updatedDispenserInfo = spendingInfoCalculator.CalculateClosedTapAmount(dispenser, dispenserInfo);

            // Assert
            updatedDispenserInfo.Should().NotBeNull();

            updatedDispenserInfo.Amount.Should().BeInRange(7.00, 8.00);
            var updatedUsages = updatedDispenserInfo.Usages.First();
            updatedUsages.TotalSpent.Should().BeInRange(7.00, 8.00);
            updatedUsages.ClosedAt.Should().NotBeNull();
        }

        [Fact]
        public void CalculateOpenTapAmount_WhenInvoked_ShouldCreateNewEntryOnUsagesWithRealTimeCalculation()
        {
            // Arrange
            var dispenserInfo = _fixture.Create<DispenserInfo>();
            dispenserInfo.Amount = 2.34;

            dispenserInfo.Usages = new List<Usages>()
            {
                new Usages()
                {
                    FlowVolume = 0.064f,
                    OpenedAt = DateTimeOffset.Now.AddSeconds(-10).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ClosedAt = DateTimeOffset.Now.AddSeconds(-5).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    TotalSpent = 2.34
                },
                new Usages()
                {
                    FlowVolume = 0.064f,
                    OpenedAt = DateTimeOffset.Now.AddSeconds(-10).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ClosedAt = null,
                    TotalSpent = 0
                },
            };

            var spendingInfoCalculator = _fixture.Create<SpendingInfoCalculator>();

            // Act
            var updatedDispenserInfo = spendingInfoCalculator.CalculateOpenTapAmount(dispenserInfo);

            // Assert
            updatedDispenserInfo.Should().NotBeNull();

            updatedDispenserInfo.Amount.Should().BeGreaterThan(2.34);
            updatedDispenserInfo.Amount.Should().BeInRange(10.00, 11.00);
            var openedUsage = updatedDispenserInfo.Usages.Last();
            openedUsage.TotalSpent.Should().BeInRange(7.00, 8.00);
            openedUsage.ClosedAt.Should().BeNull();
        }
    }
}
