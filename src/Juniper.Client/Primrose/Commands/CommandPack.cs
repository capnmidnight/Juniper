using System.Collections.Generic;
using System.Windows.Forms;

namespace Juniper.Primrose.Commands
{
    public delegate void KeyboardCommand(object prim, object tokenRows, object currentToken);

    /// <summary>
    /// A CommandPack is a collection of key sequences and text editor commands.
    /// It provides a means of using a single text rendering control to create
    /// a variety of text-controls that utilize the text space differently.
    /// </summary>
    public abstract class CommandPack
    {
        public string Name { get; }

        public IReadOnlyDictionary<Keys, KeyboardCommand> Commands { get; }

        /// <summary>
        /// Creates a new command pack.
        /// </summary>
        /// <param name="name">A friendly name for the command pack.</param>
        /// <param name="commands">An object literal of key-value pairs describing the commands.
        /// 
        /// * The object key elements are strings describing the key sequence that activates the command.
        /// * The value elements are the action that occurs when the command is activated.</param>
        protected CommandPack(string name, Dictionary<Keys, KeyboardCommand> commands)
        {
            Name = name;
            Commands = commands;
        }
    }
}