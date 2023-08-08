using FluentAssertions;
using Newtonsoft.Json;
using OktaAutomation.Hcl;

namespace OktaAutomation.UnitTests
{
    [TestClass]
    public class HclTests
    {
        [TestMethod]
        public void Parse_SingleLineAttributes()
        {
            // Arrange
            var expectedOutput = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "ExpectedOutput.tf"));
            var input = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "singleline-input.tf"));
            var helper = new HclHelper();

            // Act
            var terraform = helper.Parse(input);
            var json = JsonConvert.SerializeObject(terraform, Formatting.Indented);
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "output.tf"), json);

            // Assert
            expectedOutput.Should().Be(json);
        }

        [TestMethod]
        public void Parse_MultiLineAttributes()
        {
            // Arrange
            var expectedOutput = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "ExpectedOutput.tf"));
            var input = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "multiline-input.tf"));
            var helper = new HclHelper();

            // Act
            var terraform = helper.Parse(input);
            var json = JsonConvert.SerializeObject(terraform, Formatting.Indented);
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "output.tf"), json);

            // Assert
            expectedOutput.Should().Be(json);
        }
    }
}