using ShellcodeRunner;

namespace ShellcodeRunner.Tests;

public class ProgramTests
{
    [Fact]
    public void ValidateArgs_NoArgs_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Program.ValidateArgs(Array.Empty<string>()));
    }

    [Fact]
    public void ValidateArgs_HttpUrl_DoesNotThrow()
    {
        Program.ValidateArgs(new[] { "http://example.com/payload.bin" });
    }

    [Fact]
    public void ValidateArgs_HttpsUrl_DoesNotThrow()
    {
        Program.ValidateArgs(new[] { "https://example.com/payload.bin" });
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/payload.bin")]
    [InlineData("")]
    public void ValidateArgs_InvalidUrl_ThrowsArgumentException(string url)
    {
        Assert.Throws<ArgumentException>(() => Program.ValidateArgs(new[] { url }));
    }
}
