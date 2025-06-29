using System;
using System.Collections.Generic;

namespace NuciCLI.Menus
{
    /// <summary>
    /// Command-line menu.
    /// </summary>
    public class Menu : IDisposable
    {
        /// <summary>
        /// Gets or sets the unique identifier for this menu.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the parent menu identifier.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Gets or sets the title colour.
        /// </summary>
        /// <value>The title colour.</value>
        public NuciConsoleColour TitleColour { get; set; }

        /// <summary>
        /// Gets or sets the title decoration colour.
        /// </summary>
        /// <value>The title decoration colour.</value>
        public NuciConsoleColour TitleDecorationColour { get; set; }

        /// <summary>
        /// Gets or sets the prompt colour.
        /// </summary>
        /// <value>The prompt colour.</value>
        public NuciConsoleColour PromptColour { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the title decoration.
        /// </summary>
        /// <value>The title decoration.</value>
        public string TitleDecoration { get; set; } = "-==< ";

        /// <summary>
        /// Gets or sets the prompt.
        /// </summary>
        /// <value>The prompt.</value>
        public string Prompt { get; set; } = "> ";

        /// <summary>
        /// Gets a value indicating whether this <see cref="Menu"/> is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Occurs when this <see cref="Menu"/> was created.
        /// </summary>
        public event EventHandler Created;

        /// <summary>
        /// Occurs when this <see cref="Menu"/> was disposed.
        /// </summary>
        public event EventHandler Disposed;

        internal Dictionary<string, Command> Commands { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        public Menu()
        {
            Commands = [];

            Id = Guid.NewGuid().ToString();

            TitleColour = NuciConsoleColour.Green;
            TitleDecorationColour = NuciConsoleColour.Yellow;
            PromptColour = NuciConsoleColour.White;

            AddCommand("exit", "Exit this menu", Exit);
            AddCommand("help", "Prints the command list", HandleHelp);

            Created?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Menu"/> class.
        /// </summary>
        /// <param name="title">Title.</param>
        public Menu(string title) : this() => Title = title;

        ~Menu() => Dispose(false);

        /// <summary>
        /// Disposes the menu and releases any resources it holds.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the menu and releases any resources it holds.
        /// </summary>
        /// <param name="disposing">If set to <c>true</c>, the method is being called from Dispose.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed || !disposing)
            {
                return;
            }

            IsDisposed = true;

            Commands.Clear();

            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Exits the menu, closing it and returning to the parent menu if applicable.
        /// </summary>
        public void Exit() => MenuManager.Instance.CloseMenu(Id);

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="description">The description.</param>
        /// <param name="action">The action.</param>
        public void AddCommand(string name, string description, Action action)
        {
            Command command = new(name, description, action);

            Commands.Add(command.Name, command);
        }

        void HandleHelp() => MenuPrinter.PrintCommandList(Commands);
    }
}
