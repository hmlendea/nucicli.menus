using System;
using System.Collections.Generic;
using System.Threading;

namespace NuciCLI.Menus
{
    /// <summary>
    /// Menu manager.
    /// </summary>
    public class MenuManager
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static MenuManager Instance
        {
            get
            {
                if (instance is null)
                {
                    lock (syncRoot)
                    {
                        instance ??= new MenuManager();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets or sets the active menu identifier.
        /// </summary>
        public string ActiveMenuId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the menu manager is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether statistics are enabled.
        /// </summary>
        public bool AreStatisticsEnabled { get; set; } = false;

        /// <summary>
        /// Event raised when the menu manager is starting.
        /// </summary>
        public EventHandler Starting;

        /// <summary>
        /// Event raised when the menu manager has started.
        /// </summary>
        public EventHandler Started;

        /// <summary>
        /// Event raised when the menu manager has stopped.
        /// </summary>
        public EventHandler Stopped;

        /// <summary>
        /// Event raised when the active menu has changed.
        /// </summary>
        public EventHandler ActiveMenuChanged;

        static volatile MenuManager instance;
        static readonly Lock syncRoot = new();

        readonly Dictionary<string, Menu> menus = [];

        Menu ActiveMenu => menus[ActiveMenuId];

        /// <summary>
        /// Starts the menu manager with the specified menu type.
        /// </summary>
        public void Start<TMenu>() where TMenu : Menu
            => Start<TMenu>(null);

        /// <summary>
        /// Starts the menu manager with the specified menu type and parameters.
        /// </summary>
        /// <typeparam name="TMenu">The type of the menu.</typeparam>
        /// <param name="parameters">The parameters to pass to the menu.</param>
        public void Start<TMenu>(params object[] parameters) where TMenu : Menu
        {
            Starting?.Invoke(this, EventArgs.Empty);

            OpenMenu<TMenu>(parameters);
            Menu menu = menus[ActiveMenuId];

            IsRunning = true;

            Started?.Invoke(this, EventArgs.Empty);

            while (IsRunning)
            {
                TakeCommand();
            }

            Stopped?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Opens the menu of the specified type.
        /// </summary>
        /// <typeparam name="TMenu">The type of the menu.</typeparam>
        public void OpenMenu<TMenu>() where TMenu : Menu
            => OpenMenu<TMenu>(null);

        /// <summary>
        /// Opens the menu of the specified type with parameters.
        /// </summary>
        /// <typeparam name="TMenu">The type of the menu.</typeparam>
        /// <param name="parameters">The parameters to pass to the menu
        public void OpenMenu<TMenu>(params object[] parameters) where TMenu : Menu
            => OpenMenu(typeof(TMenu), parameters);

        /// <summary>
        /// Opens the menu.
        /// </summary>
        /// <param name="menuType">Menu type.</param>
        public void OpenMenu(Type menuType)
            => OpenMenu(menuType, null);

        /// <summary>
        /// Opens the menu.
        /// </summary>
        /// <param name="menuType">Menu type.</param>
        /// <param name="parameters">Menu parameters.</param>
        public void OpenMenu(Type menuType, params object[] parameters)
        {
            Menu newMenu = (Menu)Activator.CreateInstance(menuType, parameters);

            menus.Add(newMenu.Id, newMenu);

            if (!string.IsNullOrWhiteSpace(ActiveMenuId))
            {
                Menu curMenu = menus[ActiveMenuId];

                newMenu.ParentId = curMenu.Id;
                NuciConsole.WriteLine();
            }

            SwitchToMenu(newMenu.Id);
        }

        /// <summary>
        /// Closes the current menu.
        /// </summary>
        public void CloseMenu()
            => CloseMenu(ActiveMenuId);

        /// <summary>
        /// Closes the menu with the specified identifier.
        /// </summary>
        /// <param name="menuId">The identifier of the menu to close.</param>
        public void CloseMenu(string menuId)
        {
            Menu menu = menus[menuId];

            string parentId = menu.ParentId;

            menus.Remove(menuId);
            menu.Dispose();

            if (!string.IsNullOrEmpty(parentId))
            {
                SwitchToMenu(parentId);
            }
            else
            {
                IsRunning = false;
            }
        }

        /// <summary>
        /// Switches to the menu with the specified identifier.
        /// </summary>
        /// <param name="menuId">The identifier of the menu to switch to.</param>
        public void SwitchToMenu(string menuId)
        {
            if (ActiveMenuId.Equals(menuId))
            {
                return;
            }

            ActiveMenuId = menuId;
            MenuPrinter.PrintMenuHeader(ActiveMenu);

            ActiveMenuChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the command.
        /// </summary>
        void TakeCommand()
        {
            string cmd;

            try
            {
                cmd = NuciConsole.ReadLine(ActiveMenu.Prompt, ActiveMenu.PromptColour);
            }
            catch (InputCancellationException)
            {
                NuciConsole.CursorY -= 1;
                return;
            }

            if (!ActiveMenu.Commands.TryGetValue(cmd, out Command command))
            {
                NuciConsole.WriteLine("Unknown command", NuciConsoleColour.Red);
                return;
            }

            CommandResult result = command.Execute();

            if (AreStatisticsEnabled)
            {
                MenuPrinter.PrintCommandResults(result);
            }

            NuciConsole.WriteLine();
        }
    }
}
