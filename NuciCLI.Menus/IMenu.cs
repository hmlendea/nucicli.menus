using System;
using System.Collections.Generic;

namespace NuciCLI.Menus
{
    /// <summary>
    /// Defines the contract of a command-line menu.
    /// </summary>
    internal interface IMenu
    {
        /// <summary>
        /// Gets or sets the unique identifier for this menu.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the parent menu identifier.
        /// </summary>
        string? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the title colour.
        /// </summary>
        NuciConsoleColour TitleColour { get; set; }

        /// <summary>
        /// Gets or sets the title decoration colour.
        /// </summary>
        NuciConsoleColour TitleDecorationColour { get; set; }

        /// <summary>
        /// Gets or sets the prompt colour.
        /// </summary>
        NuciConsoleColour PromptColour { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the title decoration.
        /// </summary>
        string TitleDecoration { get; set; }

        /// <summary>
        /// Gets or sets the prompt.
        /// </summary>
        string Prompt { get; set; }

        /// <summary>
        /// Gets a value indicating whether this menu is disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Occurs when this menu was created.
        /// </summary>
        event EventHandler? Created;

        /// <summary>
        /// Occurs when this menu was disposed.
        /// </summary>
        event EventHandler? Disposed;

        /// <summary>
        /// Exits the menu, closing it and returning to the parent menu if applicable.
        /// </summary>
        void Exit();

        /// <summary>
        /// Adds a command to the menu.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="description">The description of the command.</param>
        /// <param name="action">The action to execute when the command is invoked.</param>
        void AddCommand(string name, string description, Action action);
    }
}
