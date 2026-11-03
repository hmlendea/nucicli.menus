using System;

using NuciCLI.Menus;

using NUnit.Framework;

namespace NuciCLI.Menus.UnitTests
{
    [TestFixture]
    public sealed class MenuTests
    {
        private Menu menu = null!;

        [SetUp]
        public void SetUp()
            => menu = new Menu("Test menu");

        [TearDown]
        public void TearDown()
            => menu.Dispose();

        [Test]
        public void GivenAConstructedMenu_WhenReadingItsTitle_ThenTheConfiguredTitleIsReturned()
            => Assert.That(menu.Title, Is.EqualTo("Test menu"));

        [Test]
        public void GivenAConstructedMenu_WhenReadingItsDefaults_ThenTheExpectedDefaultsAreReturned()
        {
            Assert.That(menu.TitleDecoration, Is.EqualTo("-==< "));
            Assert.That(menu.Prompt, Is.EqualTo("> "));
            Assert.That(menu.TitleColour.Name, Is.EqualTo("Green"));
            Assert.That(menu.TitleDecorationColour.Name, Is.EqualTo("Yellow"));
            Assert.That(menu.PromptColour.Name, Is.EqualTo("White"));
            Assert.That(menu.IsDisposed, Is.False);
        }

        [Test]
        public void GivenAConstructedMenu_WhenReadingItsIdentifier_ThenItIsNotEmpty()
            => Assert.That(menu.Id, Is.Not.Empty);

        [Test]
        public void GivenAConstructedMenu_WhenAddingACommand_ThenNoExceptionIsThrown()
            => Assert.DoesNotThrow(() => menu.AddCommand("run", "Run the command", static () => { }));

        [Test]
        public void GivenACommandNameAlreadyRegistered_WhenAddingItAgain_ThenAnArgumentExceptionIsThrown()
        {
            menu.AddCommand("run", "Run the command", static () => { });

            Assert.That(
                () => menu.AddCommand("run", "Run it again", static () => { }),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void GivenAnUndisposedMenu_WhenDisposingIt_ThenItIsMarkedAsDisposedAndRaisesTheEvent()
        {
            int disposedEventCount = 0;
            menu.Disposed += (_, _) => disposedEventCount++;

            menu.Dispose();

            Assert.That(menu.IsDisposed, Is.True);
            Assert.That(disposedEventCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenADisposedMenu_WhenDisposingItAgain_ThenTheEventIsRaisedOnlyOnce()
        {
            int disposedEventCount = 0;
            menu.Disposed += (_, _) => disposedEventCount++;

            menu.Dispose();
            menu.Dispose();

            Assert.That(disposedEventCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenAConfiguredMenu_WhenChangingItsProperties_ThenTheNewValuesAreReturned()
        {
            menu.Id = "custom-id";
            menu.ParentId = "parent-id";
            menu.Title = "Updated title";
            menu.TitleDecoration = "[ ";
            menu.Prompt = "Command: ";
            menu.TitleColour = NuciConsoleColour.Red;
            menu.TitleDecorationColour = NuciConsoleColour.Blue;
            menu.PromptColour = NuciConsoleColour.Cyan;

            Assert.That(menu.Id, Is.EqualTo("custom-id"));
            Assert.That(menu.ParentId, Is.EqualTo("parent-id"));
            Assert.That(menu.Title, Is.EqualTo("Updated title"));
            Assert.That(menu.TitleDecoration, Is.EqualTo("[ "));
            Assert.That(menu.Prompt, Is.EqualTo("Command: "));
            Assert.That(menu.TitleColour.Name, Is.EqualTo("Red"));
            Assert.That(menu.TitleDecorationColour.Name, Is.EqualTo("Blue"));
            Assert.That(menu.PromptColour.Name, Is.EqualTo("Cyan"));
        }
    }
}
