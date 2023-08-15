using FluentAssertions;
using Newtonsoft.Json;
using OktaAutomation.Hcl;

namespace OktaAutomation.UnitTests
{
    [TestClass]
    public class HclTests
    {
        [TestMethod]
        public void ParseResource_SingleLineAttributes()
        {
            // Arrange
            var expectedOutput = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "ExpectedOutput.tf"));
            var input = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "singleline-input.tf"));
            var helper = new HclHelper();

            // Act
            var terraform = helper.ParseResource(input);
            var json = JsonConvert.SerializeObject(terraform, Formatting.Indented);
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "output.tf"), json);

            // Assert
            expectedOutput.Should().Be(json);
        }

        [TestMethod]
        public void ParseResource_MultiLineAttributes()
        {
            // Arrange
            var expectedOutput = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "ExpectedOutput.tf"));
            var input = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "multiline-input.tf"));
            var helper = new HclHelper();

            // Act
            var terraform = helper.ParseResource(input);
            var json = JsonConvert.SerializeObject(terraform, Formatting.Indented);
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "output.tf"), json);

            // Assert
            expectedOutput.Should().Be(json);
        }

        [TestMethod]
        public void Hcl_Test()
        {
            // Arrange
            var expectedOutput = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "ExpectedOutput.tf"));
            var input = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "hcl-input.tf"));
            var helper = new HclHelper();

            // Act
            var terraform = helper.ParseAttributes(input, input.Split("\n").First());
            var json = JsonConvert.SerializeObject(terraform, Formatting.Indented);
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestFiles", "hcl-output.tf"), json);

            // Assert
            expectedOutput.Should().Be(json);
        }
    }
}