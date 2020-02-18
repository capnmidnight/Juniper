using System.Collections.Generic;
using System.Linq;

namespace Juniper.Primrose
{
    public delegate void KeyboardCommandDelegate(object prim, object tokenRows, object currentToken);

    /// <summary>
    /// A CommandPack is a collection of key sequences and text editor commands.
    /// It provides a means of using a single text rendering control to create
    /// a variety of text-controls that utilize the text space differently.
    /// </summary>
    public sealed class Commands
    {

        public static readonly Commands Default = new Commands("Plain text", new Dictionary<KeyboardCommand, KeyboardCommandDelegate>
        {
            [KeyboardCommand.Undo] = (prim, tokenRows, currentToken) =>
            {
                //prim.redo();
                //prim.scrollIntoView(prim.frontCursor);
            },
            [KeyboardCommand.Redo] = (prim, tokenRows, currentToken) =>
            {
                //prim.undo();
                //prim.scrollIntoView(prim.frontCursor);
            },
            [KeyboardCommand.Cut] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.frontCursor.i === prim.backCursor.i)
                //{
                //    prim.frontCursor.home(tokenRows);
                //    prim.backCursor.end(tokenRows);
                //}
                //prim.selectedText = "";
                //prim.scrollIntoView(prim.frontCursor);
            },
            [KeyboardCommand.Copy] = (prim, tokenRows, currentToken) =>
            {
            },
            [KeyboardCommand.Paste] = (prim, tokenRows, currentToken) =>
            {
            },
            [KeyboardCommand.InsertNewLine] = (prim, tokenRows, currentToken) =>
            {
                //prim.emit("change", {
                //    [KeyboardCommandName.target] = prim
                //        });
            },
            [KeyboardCommand.InsertTab] = (prim, tokenRows, currentToken) =>
            {
                //prim.selectedText = prim.tabString;
            },




            [KeyboardCommand.MovePreviousChar] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorLeft(tokenRows, prim.frontCursor);
            },
            [KeyboardCommand.MoveNextChar] = (prim, tokenRows, currentToken) =>
            {
                // prim.cursorRight(tokenRows, prim.frontCursor);
            },
            [KeyboardCommand.MovePreviousWord] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorSkipLeft(tokenRows, prim.frontCursor);
            },
            [KeyboardCommand.MoveNextWord] = (prim, tokenRows, currentToken) =>
            {
                // prim.cursorSkipRight(tokenRows, prim.frontCursor);
            },
            [KeyboardCommand.MoveStartOfLine] = (prim, tokenRows, currentToken) =>
            {
                // prim.cursorHome(tokenRows, prim.frontCursor);
            },
            [KeyboardCommand.MoveEndOfLine] = (prim, tokenRows, currentToken) =>
            {
                // prim.cursorEnd(tokenRows, prim.frontCursor);
            },
            [KeyboardCommand.MoveStartOfDocument] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorFullHome(tokenRows, prim.frontCursor);
            },
            [KeyboardCommand.MoveEndOfDocument] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorFullEnd(tokenRows, prim.frontCursor);
            },

            [KeyboardCommand.SelectAll] = (prim, tokenRows, currentToken) =>
            {
                //prim.frontCursor.fullhome(tokenRows);
                //prim.backCursor.fullend(tokenRows);
            },

            [KeyboardCommand.SelectPreviousChar] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorLeft(tokenRows, prim.backCursor);
            },
            [KeyboardCommand.SelectNextChar] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorRight(tokenRows, prim.backCursor);
            },
            [KeyboardCommand.SelectPreviousWord] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorSkipLeft(tokenRows, prim.backCursor);
            },
            [KeyboardCommand.SelectNextWord] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorSkipRight(tokenRows, prim.backCursor);
            },
            [KeyboardCommand.SelectStartOfLine] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorHome(tokenRows, prim.backCursor);
            },
            [KeyboardCommand.SelectEndOfLine] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorEnd(tokenRows, prim.backCursor);
            },

            [KeyboardCommand.SelectStartOfDocument] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorFullHome(tokenRows, prim.backCursor);
            },
            [KeyboardCommand.SelectEndOfDocument] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorFullEnd(tokenRows, prim.backCursor);
            },

            [KeyboardCommand.DeletePreviousChar] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.frontCursor.i === prim.backCursor.i)
                //{
                //    prim.frontCursor.left(tokenRows);
                //}
                //prim.selectedText = "";
                //prim.scrollIntoView(prim.frontCursor);
            },
            [KeyboardCommand.DeleteNextChar] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.frontCursor.i === prim.backCursor.i)
                //{
                //    prim.backCursor.right(tokenRows);
                //}
                //prim.selectedText = "";
                //prim.scrollIntoView(prim.frontCursor);
            },

            [KeyboardCommand.DeletePreviousWord] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.frontCursor.i === prim.backCursor.i)
                //{
                //    prim.frontCursor.left(tokenRows);
                //}
                //prim.selectedText = "";
                //prim.scrollIntoView(prim.frontCursor);
            },
            [KeyboardCommand.DeleteNextWord] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.frontCursor.i === prim.backCursor.i)
                //{
                //    prim.backCursor.right(tokenRows);
                //}
                //prim.selectedText = "";
                //prim.scrollIntoView(prim.frontCursor);
            }
        });

        public static readonly Commands TextEditor = Default.Extend("Text editor", new Dictionary<KeyboardCommand, KeyboardCommandDelegate>
        {
            [KeyboardCommand.InsertNewLine] = (prim, tokenRows, currentToken) =>
            {
                //var indent = "";
                //var tokenRow = tokenRows[prim.frontCursor.y];
                //if (tokenRow.length > 0 && tokenRow[0].type === "whitespace")
                //{
                //    indent = tokenRow[0].value;
                //}
                //prim.selectedText = "\n" + indent;
                //prim.scrollIntoView(prim.frontCursor);
            },

            [KeyboardCommand.MovePreviousLine] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorUp(tokenRows, prim.frontCursor);
            },
            [KeyboardCommand.MoveNextLine] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorDown(tokenRows, prim.frontCursor);
            },
            [KeyboardCommand.MovePageUp] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorPageUp(tokenRows, prim.frontCursor);
            },
            [KeyboardCommand.MovePageDown] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorPageDown(tokenRows, prim.frontCursor);
            },

            [KeyboardCommand.SelectPreviousLine] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorUp(tokenRows, prim.backCursor);
            },
            [KeyboardCommand.SelectNextLine] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorDown(tokenRows, prim.backCursor);
            },
            [KeyboardCommand.SelectPageUp] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorPageUp(tokenRows, prim.backCursor);
            },
            [KeyboardCommand.SelectPageDown] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorPageDown(tokenRows, prim.backCursor);
            },

            [KeyboardCommand.ScrollUp] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.scroll.y > 0)
                //{
                //    --prim.scroll.y;
                //}
            },
            [KeyboardCommand.ScrollDown] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.scroll.y < tokenRows.length)
                //{
                //    ++prim.scroll.y;
                //}
            }
        });

        public string Name { get; }

        public IReadOnlyDictionary<KeyboardCommand, KeyboardCommandDelegate> Actions { get; }

        /// <summary>
        /// Creates a new command pack.
        /// </summary>
        /// <param name="name">A friendly name for the command pack.</param>
        /// <param name="commands">An object literal of key-value pairs describing the commands.
        /// 
        /// * The object key elements are strings describing the key sequence that activates the command.
        /// * The value elements are the action that occurs when the command is activated.</param>
        private Commands(string name, Dictionary<KeyboardCommand, KeyboardCommandDelegate> commands)
        {
            Name = name;
            Actions = commands;
        }

        private Commands Extend(string name, Dictionary<KeyboardCommand, KeyboardCommandDelegate> commands)
        {
            var combined = Actions.ToDictionary(kv => kv.Key, kv => kv.Value);
            foreach(var command in commands)
            {
                combined[command.Key] = command.Value;
            }

            return new Commands(name, combined);
        }
    }
}