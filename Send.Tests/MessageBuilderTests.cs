using System;
using System.IO;
using Xunit;
using Send;

namespace Send.Tests
{
  public class MessageBuilderTests
  {
    [Fact]
    public void ReadName_ReadsNameFromConsole_ReturnsNameFromConsole()
    {
      // Arrange
      var output = new StringWriter();
      Console.SetOut(output);

      var input = new StringReader("Siseko");
      Console.SetIn(input);

      // Act
      var messageBuilder = new MessageBuilder();
      var name = messageBuilder.ReadName();

      // Assert
      Assert.Equal("Siseko", name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void ConstructMessage_NameIsNullOrEmpty_ReturnEmptyMessage(string name)
    {
      // Act
      var messageBuilder = new MessageBuilder();
      var message = messageBuilder.ConstructMessage(name);

      // Assert
      Assert.Empty(message);
    }

    [Theory]
    [InlineData("Siseko")]
    [InlineData("Wonga Assessment")]
    public void ConstructMessage_NameIsNotNullOrEmpty_ReturnCorrectMessage(string name)
    {
      // Act
      var messageBuilder = new MessageBuilder();
      var message = messageBuilder.ConstructMessage(name);

      // Assert
      Assert.Equal($"Hello my name is, {name}", message);
    }
  }
}
