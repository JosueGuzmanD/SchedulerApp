using FluentAssertions;
using SchedulerApplication.Common.Validator;
using SchedulerApplication.Models.SchedulerConfigurations;

namespace SchedulerApp.Testing;

    public class ConfigurationValidatorTests
    {
        [Fact]
        public void Validate_ShouldThrowException_WhenConfigurationIsDisabled()
        {
            // Arrange
            var validator = new ConfigurationValidator();
            var configuration = new OnceSchedulerConfiguration { IsEnabled = false };

            // Act
            Action act = () => validator.Validate(configuration);

            // Assert
            act.Should().Throw<ArgumentException>().WithMessage("Configuration must be enabled.");
        }

        [Fact]
        public void Validate_ShouldNotThrowException_WhenConfigurationIsEnabled()
        {
            // Arrange
            var validator = new ConfigurationValidator();
            var configuration = new OnceSchedulerConfiguration { IsEnabled = true };

            // Act
            Action act = () => validator.Validate(configuration);

            // Assert
            act.Should().NotThrow<ArgumentException>();
        }
}

