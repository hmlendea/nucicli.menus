using System;

namespace NuciCLI.Menus
{
    /// <summary>
    /// Defines the contract of the menu manager.
    /// </summary>
    internal interface IMenuManager
    {
        /// <summary>
        /// Gets or sets the active menu identifier.
        /// </summary>
        string? ActiveMenuId { get; }

        /// <summary>
        /// Gets a value indicating whether the menu manager is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets or sets a value indicating whether statistics are enabled.
        /// </summary>
        bool AreStatisticsEnabled { get; set; }

        /// <summary>
        /// Starts the menu manager with the specified menu type.
        /// </summary>
        /// <typeparam name="TMenu">The type of the menu.</typeparam>
        void Start<TMenu>() where TMenu : Menu;

        /// <summary>
        /// Starts the menu manager with the specified menu type and parameters.
        /// </summary>
        /// <typeparam name="TMenu">The type of the menu.</typeparam>
        /// <param name="parameters">The parameters to pass to the menu.</param>
        void Start<TMenu>(params object[] parameters) where TMenu : Menu;

        /// <summary>
        /// Opens the menu of the specified type.
        /// </summary>
        /// <typeparam name="TMenu">The type of the menu.</typeparam>
        void OpenMenu<TMenu>() where TMenu : Menu;

        /// <summary>
        /// Opens the menu of the specified type with parameters.
        /// </summary>
        /// <typeparam name="TMenu">The type of the menu.</typeparam>
        /// <param name="parameters">The parameters to pass to the menu.</param>
        void OpenMenu<TMenu>(params object[] parameters) where TMenu : Menu;

        /// <summary>
        /// Opens the menu of the specified type.
        /// </summary>
        /// <param name="menuType">The type of the menu.</param>
        void OpenMenu(Type menuType);

        /// <summary>
        /// Opens the menu of the specified type with parameters.
        /// </summary>
        /// <param name="menuType">The type of the menu.</param>
        /// <param name="parameters">The parameters to pass to the menu.</param>
        void OpenMenu(Type menuType, params object[] parameters);

        /// <summary>
        /// Closes the current menu.
        /// </summary>
        void CloseMenu();

        /// <summary>
        /// Closes the menu with the specified identifier.
        /// </summary>
        /// <param name="menuId">The identifier of the menu to close.</param>
        void CloseMenu(string menuId);

        /// <summary>
        /// Switches to the menu with the specified identifier.
        /// </summary>
        /// <param name="menuId">The identifier of the menu to switch to.</param>
        void SwitchToMenu(string menuId);
    }
}
