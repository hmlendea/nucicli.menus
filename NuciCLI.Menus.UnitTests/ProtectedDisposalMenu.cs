namespace NuciCLI.Menus.UnitTests
{
    internal sealed class ProtectedDisposalMenu : Menu
    {
        public void ReleaseResources(bool isDisposing)
            => Dispose(isDisposing);
    }
}
