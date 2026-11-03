using System;
using System.Collections.Generic;

using Moq;

using NUnit.Framework;

namespace NuciCLI.Menus.UnitTests
{
    [TestFixture]
    public sealed class MenuManagerTests
    {
        private MenuManager manager = null!;
        private Mock<EventHandler> activeMenuChanged = null!;

        [SetUp]
        public void SetUp()
        {
            manager = new();
            activeMenuChanged = new();
            manager.ActiveMenuChanged += activeMenuChanged.Object;
        }

        [Test]
        public void GivenTheSingleton_WhenRequestingItAgain_ThenTheSameInstanceIsReturned()
            => Assert.That(
                MenuManager.Instance,
                Is.SameAs(MenuManager.Instance));

        [Test]
        public void GivenANewManager_WhenInspectingItsState_ThenNoMenuIsActiveAndItIsNotRunning()
        {
            Assert.That(manager.ActiveMenuId, Is.Null);
            Assert.That(!manager.IsRunning);
        }

        [TestCase("missing")]
        [TestCase("")]
        [TestCase(" ")]
        public void GivenAnUnknownIdentifier_WhenClosingAMenu_ThenNoStateOrEventsAreChanged(string menuId)
        {
            Assert.That(
                () => manager.CloseMenu(menuId),
                Throws.TypeOf<KeyNotFoundException>());
            Assert.That(manager.ActiveMenuId, Is.Null);
            Assert.That(!manager.IsRunning);
            activeMenuChanged.VerifyNoOtherCalls();
        }

        [Test]
        public void GivenNoActiveMenu_WhenClosingIt_ThenAnArgumentNullExceptionIsThrown()
        {
            Assert.That(
                () => manager.CloseMenu(),
                Throws.TypeOf<ArgumentNullException>());
            activeMenuChanged.VerifyNoOtherCalls();
        }

        [Test]
        public void GivenANullIdentifier_WhenClosingAMenu_ThenAnArgumentNullExceptionIsThrown()
        {
            Assert.That(
                () => manager.CloseMenu(null!),
                Throws.TypeOf<ArgumentNullException>());
            activeMenuChanged.VerifyNoOtherCalls();
        }

        [Test]
        public void GivenANullIdentifier_WhenSwitchingMenus_ThenNoStateOrEventsAreChanged()
        {
            Assert.That(
                () => manager.SwitchToMenu(null!),
                Throws.TypeOf<NullReferenceException>());
            Assert.That(manager.ActiveMenuId, Is.Null);
            activeMenuChanged.VerifyNoOtherCalls();
        }

        [TestCase(null)]
        [TestCase(typeof(string))]
        [TestCase(typeof(IDisposable))]
        public void GivenAnUnconstructibleType_WhenOpeningAMenu_ThenNoStateOrEventsAreChanged(Type? menuType)
        {
            Assert.That(
                () => manager.OpenMenu(menuType!),
                Throws.Exception);
            Assert.That(manager.ActiveMenuId, Is.Null);
            Assert.That(!manager.IsRunning);
            activeMenuChanged.VerifyNoOtherCalls();
        }

        [TestCase("Dark Souls")]
        [TestCase("Age of Empires")]
        public void GivenIncompatibleArguments_WhenOpeningAMenuByType_ThenAMissingMethodExceptionIsThrown(string title)
        {
            Assert.That(
                () => manager.OpenMenu(typeof(Menu), title, title),
                Throws.TypeOf<MissingMethodException>());
            Assert.That(manager.ActiveMenuId, Is.Null);
            activeMenuChanged.VerifyNoOtherCalls();
        }

        [TestCase("Dark Souls")]
        [TestCase("Age of Empires")]
        public void GivenIncompatibleArguments_WhenOpeningAMenuGenerically_ThenAMissingMethodExceptionIsThrown(string title)
        {
            Assert.That(
                () => manager.OpenMenu<Menu>(title, title),
                Throws.TypeOf<MissingMethodException>());
            Assert.That(manager.ActiveMenuId, Is.Null);
            activeMenuChanged.VerifyNoOtherCalls();
        }
    }
}
