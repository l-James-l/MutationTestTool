using GUI.Converters;
using Models.Enums;
using System.Windows.Media;

namespace GuiTests.Converters;

public class MutationStatusColourConverterTests
{
    [Test]
    public void GivenConverter_WhenConverting_ThenReturnsExpectedColour()
    {
        // Arrange
        var converter = new MutationStatusColourConverter();
        // Act & Assert

        Assert.That(converter.Convert(MutantStatus.Discovered, null, null, null), Is.EqualTo(Brushes.Red));
        Assert.That(converter.Convert(MutantStatus.Available, null, null, null), Is.EqualTo(Brushes.Orange));
        Assert.That(converter.Convert(MutantStatus.CausedBuildError, null, null, null), Is.EqualTo(Brushes.Red));
        Assert.That(converter.Convert(MutantStatus.TestOngoing, null, null, null), Is.EqualTo(Brushes.Blue));
        Assert.That(converter.Convert(MutantStatus.Survived, null, null, null), Is.EqualTo(Brushes.Red));
        Assert.That(converter.Convert(MutantStatus.Killed, null, null, null), Is.EqualTo(Brushes.Green));
        Assert.That(converter.Convert(MutantStatus.NoCoverage, null, null, null), Is.EqualTo(Brushes.Red));
        Assert.That(converter.Convert(MutantStatus.IgnoredMultipleOnLine, null, null, null), Is.EqualTo(Brushes.Gray));
    }

    [Test]
    public void GivenInvalidValue_WhenConverting_ThenReturnsRedBrush()
    {
        // Arrange
        var converter = new MutationStatusColourConverter();
        var invalidValue = "InvalidStatus";
        
        // Act
        var result = converter.Convert(invalidValue, null, null, null);
     
        // Assert
        Assert.That(result, Is.EqualTo(Brushes.Red));
    }
}
