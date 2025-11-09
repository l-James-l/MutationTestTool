namespace CoreTests.Startup;

public class EstablishLoggerConfigurationTests
{
    private string _originalWorkingDir;
    private string _tempDir;

    [SetUp]
    public void SetUp()
    {
        _originalWorkingDir = Directory.GetCurrentDirectory();
        _tempDir = Path.Combine(Path.GetTempPath(), "EstLoggerTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
        Directory.SetCurrentDirectory(_tempDir);
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            Directory.SetCurrentDirectory(_originalWorkingDir);
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, recursive: true);
            }
        }
        catch
        {
            // ignore cleanup errors
        }
    }

    [Test]
    public void GivenLogFileAndFolderDontExists_WhenLoggerConstructed_LogFileCreated()
    {
        // Arrange
        // Ensure no Log or Logs directories exist
        Assert.IsFalse(Directory.Exists(Path.Combine(_tempDir, "Log")));

        // Act & Assert - constructor should not throw
        Assert.DoesNotThrow(() => new EstablishLoggerConfiguration());

        var logsDir = Path.Combine(_tempDir, "Logs");
        Assert.IsTrue(Directory.Exists(logsDir));

        var files = Directory.GetFiles(logsDir, "Log-*.txt");
        Assert.IsNotEmpty(files);

        var filePath = files[0];
        Assert.IsTrue(File.Exists(filePath));
        var length = new FileInfo(filePath).Length;
        Assert.Greater(length, 0, "Log file should not be empty");
    }

    [Test]
    public void GivenLogFolderExists_whenLoggerConstructed_ThenNoErrorThrown()
    {
        // Arrange
        // Create 'Log' directory (singular) to prevent creation of 'Logs' directory
        Directory.CreateDirectory(Path.Combine(_tempDir, "Log"));

        // Act & Assert - constructor should not throw
        Assert.DoesNotThrow(() => new EstablishLoggerConfiguration());
    }
}
