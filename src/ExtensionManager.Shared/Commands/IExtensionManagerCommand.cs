using System;

namespace ExtensionManager
{
    /// <summary>
    /// Defines the publicly-exposed methods and properties of a command in the Extension Manager.
    /// </summary>
    public interface IExtensionManagerCommand
    {
        /// <summary>
        /// Supplies code that is to be executed when the user chooses this command from
        /// menus or toolbars.
        /// </summary>
        /// <param name="sender">Reference to the sender of the event.</param>
        /// <param name="e">
        /// A <see cref="T:System.EventArgs" /> that contains the event
        /// data.
        /// </param>
        void Execute(object sender, EventArgs e);
    }
}