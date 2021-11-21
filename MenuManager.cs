using System;
using System.Collections.Generic;

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
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new MenuManager();
                        }
                    }
                }

                return instance;
            }
        }

        public string ActiveMenuId { get; private set; }

        public bool IsRunning { get; private set; }

        public bool AreStatisticsEnabled { get; set; }

        public EventHandler Starting;

        public EventHandler Started;

        public EventHandler Stopped;

        public EventHandler ActiveMenuChanged;

        static volatile MenuManager instance;
        static object syncRoot = new object();

        IDictionary<string, Menu> menus;

        Menu ActiveMenu => menus[ActiveMenuId];

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuManager"/> class.
        /// </summary>
        public MenuManager()
        {
            menus = new Dictionary<string, Menu>();
        }

        public void Start<TMenu>() where TMenu : Menu
            => Start<TMenu>(null);

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

        public void OpenMenu<TMenu>() where TMenu : Menu
            => OpenMenu<TMenu>(null);

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

        public void SwitchToMenu(string menuId)
        {
            if (ActiveMenuId == menuId)
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
            string cmd = null;
            
            try
            {
                cmd = NuciConsole.ReadLine(ActiveMenu.Prompt, ActiveMenu.PromptColour);
            }
            catch (InputCancellationException)
            {
                NuciConsole.CursorY -= 1;
                return;
            }

            if (!ActiveMenu.Commands.ContainsKey(cmd))
            {
                NuciConsole.WriteLine("Unknown command", NuciConsoleColour.Red);
                return;
            }

            Command command = ActiveMenu.Commands[cmd];
            CommandResult result = command.Execute();

            if (AreStatisticsEnabled)
            {
                MenuPrinter.PrintCommandResults(result);
            }

            NuciConsole.WriteLine();
        }
    }
}
