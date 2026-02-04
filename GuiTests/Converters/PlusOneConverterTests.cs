using GUI.Converters;

namespace GuiTests.Converters;

public class PlusOneConverterTests
{
    [Test]
    public void GivenConverter_WhenConverting_ThenReturnsValuePlusOne()
    {
        // Arrange
        var converter = new PlusOneConverter();

        // Act & Assert
        Assert.That(converter.Convert(0, null, null, null), Is.EqualTo(1));
        Assert.That(converter.Convert(5, null, null, null), Is.EqualTo(6));
        Assert.That(converter.Convert(-3, null, null, null), Is.EqualTo(-2));
    }
}