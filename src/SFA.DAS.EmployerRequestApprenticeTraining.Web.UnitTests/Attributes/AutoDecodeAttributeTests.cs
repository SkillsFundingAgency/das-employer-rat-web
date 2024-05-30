﻿using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Attributes;
using SFA.DAS.EmployerRequestApprenticeTraining.Web.Models;
using SFA.DAS.Encoding;
using System.Linq;

namespace SFA.DAS.EmployerRequestApprenticeTraining.Web.UnitTests.Attributes
{
    [TestFixture]
    public class AutoDecodeAttributeTests
    {
        private Mock<IEncodingService> _encodingServiceMock;

        [SetUp]
        public void SetUp()
        {
            _encodingServiceMock = new Mock<IEncodingService>();
        }

        [Test]
        public void AutoDecodeAttribute_Should_Be_Applied_To_AccountId_Property()
        {
            // Arrange
            var type = typeof(Parameters);
            var property = type.GetProperty("AccountId");

            // Act
            var attribute = property.GetCustomAttributes(typeof(AutoDecodeAttribute), false).FirstOrDefault() as AutoDecodeAttribute;

            // Assert
            attribute.Should().NotBeNull("AutoDecodeAttribute should be applied to the AccountId property.");
            attribute.Source.Should().Be("HashedAccountId", "the source value of the AutoDecodeAttribute is incorrect.");
            attribute.EncodingType.Should().Be(EncodingType.AccountId, "the EncodingType of the AutoDecodeAttribute is incorrect.");
        }

        [Test]
        public void AutoDecodeAttribute_Should_Decode_HashedAccountId_To_AccountId()
        {
            // Arrange
            var hashedAccountId = "XYZ123";
            var expectedAccountId = 12345;
            _encodingServiceMock.Setup(x => x.Decode(hashedAccountId, EncodingType.AccountId)).Returns(expectedAccountId);

            var parameters = new Parameters
            {
                HashedAccountId = hashedAccountId
            };

            // Act
            var type = typeof(Parameters);
            var property = type.GetProperty("AccountId");
            var attribute = property.GetCustomAttributes(typeof(AutoDecodeAttribute), false).FirstOrDefault() as AutoDecodeAttribute;

            // Simulate the usage of the attribute
            if (attribute != null)
            {
                var decodedValue = _encodingServiceMock.Object.Decode(parameters.HashedAccountId, attribute.EncodingType);
                property.SetValue(parameters, decodedValue);
            }

            // Assert
            parameters.AccountId.Should().Be(expectedAccountId, "the AccountId should be decoded correctly from the HashedAccountId.");
        }
    }
}