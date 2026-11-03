using System;
using System.Collections.Generic;

using Moq;

using NUnit.Framework;

namespace NuciCLI.Menus.UnitTests
{
    [TestFixture]
    public sealed class MenuManagerLifecycleTests
    {
        private MenuManager manager = null!;
        private Mock<EventHandler> starting = null!;
        private Mock<EventHandler> started = null!;
        private Mock<EventHandler> stopped = null!;
        private Mock<EventHandler> activeMenuChanged = null!;

        [SetUp]
        public void SetUp()
        {
            manager = new();
            starting = new();
            started = new();
            stopped = new();
            activeMenuChanged = new();
            manager.Starting += starting.Object;
            manager.Started += started.Object;
            manager.Stopped += stopped.Object;
            manager.ActiveMenuChanged += activeMenuChanged.Object;
        }

        [TestCase("Dark Souls")]
        [TestCase("Age of Empires")]
        public void GivenInvalidConstructorArguments_WhenStarting_ThenOnlyTheStartingEventIsRaised(string title)
        {
            Assert.That(
                () => manager.Start<Menu>(title, title),
                Throws.TypeOf<MissingMethodException>());
            starting.Verify(handler => handler(manager, EventArgs.Empty), Times.Once);
            starting.VerifyNoOtherCalls();
            VerifyNoStartupSideEffects();
        }

        [Test]
        public void GivenAThrowingStartingHandler_WhenStartingWithoutArguments_ThenTheOriginalExceptionEscapes()
        {
            InvalidOperationException exception = new("Startup was interrupted.");
            starting.Setup(handler => handler(manager, EventArgs.Empty)).Throws(exception);

            Assert.That(
                () => manager.Start<Menu>(),
                Throws.TypeOf<InvalidOperationException>().With.SameAs(exception));
            starting.Verify(handler => handler(manager, EventArgs.Empty), Times.Once);
            VerifyNoStartupSideEffects();
        }

        [Test]
        public void GivenAThrowingStartingHandler_WhenStartingWithArguments_ThenTheOriginalExceptionEscapes()
        {
            InvalidOperationException exception = new("Startup was interrupted.");
            starting.Setup(handler => handler(manager, EventArgs.Empty)).Throws(exception);

            Assert.That(
                () => manager.Start<Menu>("Dark Souls"),
                Throws.TypeOf<InvalidOperationException>().With.SameAs(exception));
            starting.Verify(handler => handler(manager, EventArgs.Empty), Times.Once);
            VerifyNoStartupSideEffects();
        }

        [Test]
        public void GivenAStartingHandler_WhenStartingWithInvalidArguments_ThenItObservesThePreStartupState()
        {
            List<bool> runningStates = [];
            List<string?> activeIdentifiers = [];
            manager.Starting += (sender, arguments) =>
            {
                runningStates.Add(manager.IsRunning);
                activeIdentifiers.Add(manager.ActiveMenuId);
            };

            Assert.That(
                () => manager.Start<Menu>("Dark Souls", "Age of Empires"),
                Throws.TypeOf<MissingMethodException>());
            Assert.That(runningStates, Has.Count.EqualTo(1));
            Assert.That(runningStates[0], Is.False);
            Assert.That(activeIdentifiers, Has.Count.EqualTo(1));
            Assert.That(activeIdentifiers[0], Is.Null);
        }

        private void VerifyNoStartupSideEffects()
        {
            Assert.That(!manager.IsRunning);
            Assert.That(manager.ActiveMenuId, Is.Null);
            started.VerifyNoOtherCalls();
            stopped.VerifyNoOtherCalls();
            activeMenuChanged.VerifyNoOtherCalls();
        }
    }
}
