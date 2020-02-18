using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Juniper.Primrose.Commands
{
    public class BasicTextInput : CommandPack
    {
        private static readonly Dictionary<Keys, KeyboardCommand> BASE_COMMANDS = new Dictionary<Keys, KeyboardCommand>
        {
            [Keys.Left] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorLeft(tokenRows, prim.frontCursor);
            },
            [Keys.Right] = (prim, tokenRows, currentToken) =>
            {
                // prim.cursorRight(tokenRows, prim.frontCursor);
            },
            [Keys.Home] = (prim, tokenRows, currentToken) =>
            {
                // prim.cursorHome(tokenRows, prim.frontCursor);
            },
            [Keys.End] = (prim, tokenRows, currentToken) =>
            {
                // prim.cursorEnd(tokenRows, prim.frontCursor);
            },
            [Keys.Back] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.frontCursor.i === prim.backCursor.i)
                //{
                //    prim.frontCursor.left(tokenRows);
                //}
                //prim.selectedText = "";
                //prim.scrollIntoView(prim.frontCursor);
            },
            [Keys.Enter] = (prim, tokenRows, currentToken) =>
            {
                //prim.emit("change", {
                //    [KeyboardCommandName.target] = prim
                //        });
            },
            [Keys.Delete] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.frontCursor.i === prim.backCursor.i)
                //{
                //    prim.backCursor.right(tokenRows);
                //}
                //prim.selectedText = "";
                //prim.scrollIntoView(prim.frontCursor);
            },
            [Keys.Tab] = (prim, tokenRows, currentToken) =>
            {
                //prim.selectedText = prim.tabString;
            },

            [Keys.Control | Keys.Left] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorSkipLeft(tokenRows, prim.frontCursor);
            },
            [Keys.Control | Keys.Right] = (prim, tokenRows, currentToken) =>
            {
                // prim.cursorSkipRight(tokenRows, prim.frontCursor);
            },

            [Keys.Control | Keys.Home] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorFullHome(tokenRows, prim.frontCursor);
            },
            [Keys.Control | Keys.End] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorFullEnd(tokenRows, prim.frontCursor);
            },

            [Keys.Control | Keys.A] = (prim, tokenRows, currentToken) =>
            {
                //prim.frontCursor.fullhome(tokenRows);
                //prim.backCursor.fullend(tokenRows);
            },

            [Keys.Control | Keys.Z] = (prim, tokenRows, currentToken) =>
            {
                //prim.redo();
                //prim.scrollIntoView(prim.frontCursor);
            },
            [Keys.Control | Keys.Y] = (prim, tokenRows, currentToken) =>
            {
                //prim.undo();
                //prim.scrollIntoView(prim.frontCursor);
            },

            [Keys.Shift | Keys.Left] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorLeft(tokenRows, prim.backCursor);
            },
            [Keys.Shift | Keys.Control | Keys.Left] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorSkipLeft(tokenRows, prim.backCursor);
            },
            [Keys.Shift | Keys.Right] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorRight(tokenRows, prim.backCursor);
            },
            [Keys.Shift | Keys.Control | Keys.Right] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorSkipRight(tokenRows, prim.backCursor);
            },
            [Keys.Shift | Keys.Home] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorHome(tokenRows, prim.backCursor);
            },
            [Keys.Shift | Keys.End] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorEnd(tokenRows, prim.backCursor);
            },
            [Keys.Shift | Keys.Delete] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.frontCursor.i === prim.backCursor.i)
                //{
                //    prim.frontCursor.home(tokenRows);
                //    prim.backCursor.end(tokenRows);
                //}
                //prim.selectedText = "";
                //prim.scrollIntoView(prim.frontCursor);
            },

            [Keys.Control | Keys.Shift | Keys.Home] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorFullHome(tokenRows, prim.backCursor);
            },
            [Keys.Control | Keys.Shift | Keys.End] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorFullEnd(tokenRows, prim.backCursor);
            }
        };

        public static readonly BasicTextInput Default = new BasicTextInput("Plain text", null);
        public static readonly BasicTextInput TextEditor = new BasicTextInput("Text editor", new Dictionary<Keys, KeyboardCommand>
        {
            [Keys.Up] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorUp(tokenRows, prim.frontCursor);
            },
            [Keys.Down] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorDown(tokenRows, prim.frontCursor);
            },
            [Keys.PageUp] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorPageUp(tokenRows, prim.frontCursor);
            },
            [Keys.PageDown] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorPageDown(tokenRows, prim.frontCursor);
            },
            [Keys.Enter] = (prim, tokenRows, currentToken) =>
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

            [Keys.Shift | Keys.Up] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorUp(tokenRows, prim.backCursor);
            },
            [Keys.Shift | Keys.Down] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorDown(tokenRows, prim.backCursor);
            },
            [Keys.Shift | Keys.PageUp] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorPageUp(tokenRows, prim.backCursor);
            },
            [Keys.Shift | Keys.PageDown] = (prim, tokenRows, currentToken) =>
            {
                //prim.cursorPageDown(tokenRows, prim.backCursor);
            },

            [Keys.Control | Keys.Down] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.scroll.y < tokenRows.length)
                //{
                //    ++prim.scroll.y;
                //}
            },
            [Keys.Control | Keys.Up] = (prim, tokenRows, currentToken) =>
            {
                //if (prim.scroll.y > 0)
                //{
                //    --prim.scroll.y;
                //}
            }
        });

        private static Dictionary<Keys, KeyboardCommand> ValidateCommands(Dictionary<Keys, KeyboardCommand> additionalCommands)
        {
            var commands = BASE_COMMANDS.ToDictionary(kv => kv.Key, kv => kv.Value);
            if (additionalCommands is object)
            {
                foreach (var key in commands.Keys)
                {
                    commands[key] = additionalCommands[key];
                }
            }

            return commands;
        }

        public BasicTextInput(string additionalName, Dictionary<Keys, KeyboardCommand> additionalCommands)
            : base(additionalName ?? "Text editor commands", ValidateCommands(additionalCommands))
        { }
    }
}