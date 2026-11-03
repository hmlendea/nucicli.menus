using System;

using Moq;

using NUnit.Framework;

namespace NuciCLI.Menus.UnitTests
{
    [TestFixture]
    public sealed class MenuCommandRegistrationTests
    {
        private Menu menu = null!;
        private Mock<Action> action = null!;

        [SetUp]
        public void SetUp()
        {
            menu = new();
            action = new();
        }

        [TearDown]
        public void TearDown()
            => menu.Dispose();

        [TestCase("exit")]
        [TestCase("help")]
        public void GivenABuiltInCommand_WhenRegisteringItsName_ThenADuplicateIsRejected(string commandName)
            => Assert.That(
                () => menu.AddCommand(commandName, "A duplicate command", action.Object),
                Throws.TypeOf<ArgumentException>());

        [TestCase("run")]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("two words")]
        [TestCase("\u0219terge")]
        public void GivenAnUnusedName_WhenRegisteringItTwice_ThenOnlyTheFirstRegistrationSucceeds(string commandName)
        {
            menu.AddCommand(commandName, "A registered command", action.Object);

            Assert.That(
                () => menu.AddCommand(commandName, "A duplicate command", action.Object),
                Throws.TypeOf<ArgumentException>());
            action.VerifyNoOtherCalls();
        }

        [TestCase("EXIT")]
        [TestCase("Help")]
        [TestCase("Run")]
        public void GivenDistinctCasing_WhenRegisteringCommands_ThenBothNamesAreAccepted(string commandName)
        {
            menu.AddCommand(commandName, "A case-sensitive command", action.Object);

            Assert.That(
                () => menu.AddCommand(commandName, "A duplicate command", action.Object),
                Throws.TypeOf<ArgumentException>());
            action.VerifyNoOtherCalls();
        }

        [Test]
        public void GivenANullName_WhenRegisteringACommand_ThenTheActionIsNotExecuted()
        {
            Assert.That(
                () => menu.AddCommand(null!, "An invalid command", action.Object),
                Throws.TypeOf<ArgumentNullException>());
            action.VerifyNoOtherCalls();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("A description")]
        public void GivenADescription_WhenRegisteringACommand_ThenTheActionIsNotExecuted(string? description)
        {
            menu.AddCommand("run", description!, action.Object);

            action.VerifyNoOtherCalls();
        }

        [Test]
        public void GivenANullAction_WhenRegisteringACommand_ThenItsNameIsReserved()
        {
            menu.AddCommand("run", "A command without an action", null!);

            Assert.That(
                () => menu.AddCommand("run", "A duplicate command", action.Object),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void GivenRegisteredCommands_WhenDisposingTheMenu_ThenActionsAreNotExecuted()
        {
            menu.AddCommand("run", "A registered command", action.Object);

            menu.Dispose();

            action.VerifyNoOtherCalls();
            Assert.That(menu.IsDisposed);
        }
    }
}
