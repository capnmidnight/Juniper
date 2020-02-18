using System.Collections.Generic;
using System.Windows.Forms;

namespace Juniper.Primrose
{
    /// <summary>
    /// A description of how a specific operating system handles keyboard shortcuts.
    /// </summary>
    public class OperatingSystem
    {
        public string Name { get; }

        private readonly Dictionary<Keys, KeyboardCommand> translations = new Dictionary<Keys, KeyboardCommand>();

        /// <summary>
        /// A description of how a specific operating system handles keyboard shortcuts.
        /// </summary>
        /// <param name="A friendly name for the operating system"></param>
        private OperatingSystem(string osName, params (KeyboardCommand cmd, Keys combo)[] translations)
        {
            Name = osName;

            foreach(var (cmd, combo) in translations)
            {
                this.translations[combo] = cmd;
            }
        }

        public KeyboardCommand Translate(Keys key)
        {
            return translations.Get(key, KeyboardCommand.None);
        }

        public static readonly OperatingSystem Windows = new OperatingSystem("Windows",
            (KeyboardCommand.Undo, Keys.Alt | Keys.Back),
            (KeyboardCommand.Undo, Keys.Control | Keys.Z),
            (KeyboardCommand.Redo, Keys.Alt | Keys.Shift | Keys.Back),
            (KeyboardCommand.Redo, Keys.Control | Keys.Y),
            (KeyboardCommand.Cut, Keys.Shift | Keys.Delete),
            (KeyboardCommand.Cut, Keys.Control | Keys.X),
            (KeyboardCommand.Copy, Keys.Control | Keys.Insert),
            (KeyboardCommand.Copy, Keys.Control | Keys.C),
            (KeyboardCommand.Paste, Keys.Shift | Keys.Insert),
            (KeyboardCommand.Paste, Keys.Control | Keys.V),
            (KeyboardCommand.InsertNewLine, Keys.Return),
            (KeyboardCommand.InsertTab, Keys.Tab),

            (KeyboardCommand.MovePreviousChar, Keys.Left),
            (KeyboardCommand.MoveNextChar, Keys.Right),
            (KeyboardCommand.MovePreviousWord, Keys.Control | Keys.Left),
            (KeyboardCommand.MoveNextWord, Keys.Control | Keys.Right),
            (KeyboardCommand.MoveStartOfLine, Keys.Home),
            (KeyboardCommand.MoveEndOfLine, Keys.End),
            (KeyboardCommand.MovePreviousLine, Keys.Up),
            (KeyboardCommand.MoveNextLine, Keys.Down),
            (KeyboardCommand.MovePageDown, Keys.PageDown),
            (KeyboardCommand.MovePageUp, Keys.PageUp),
            (KeyboardCommand.MoveStartOfDocument, Keys.Control | Keys.Home),
            (KeyboardCommand.MoveEndOfDocument, Keys.Control | Keys.End),

            (KeyboardCommand.SelectAll, Keys.Control | Keys.A),

            (KeyboardCommand.SelectPreviousChar, Keys.Shift | Keys.Left),
            (KeyboardCommand.SelectNextChar, Keys.Shift | Keys.Right),
            (KeyboardCommand.SelectPreviousWord, Keys.Shift | Keys.Control | Keys.Left),
            (KeyboardCommand.SelectNextWord, Keys.Shift | Keys.Control | Keys.Right),
            (KeyboardCommand.SelectStartOfLine, Keys.Shift | Keys.Home),
            (KeyboardCommand.SelectEndOfLine, Keys.Shift | Keys.End),
            (KeyboardCommand.SelectPreviousLine, Keys.Shift | Keys.Up),
            (KeyboardCommand.SelectNextLine, Keys.Shift | Keys.Down),
            (KeyboardCommand.SelectPageUp, Keys.Shift | Keys.PageUp),
            (KeyboardCommand.SelectPageDown, Keys.Shift | Keys.PageDown),
            (KeyboardCommand.SelectStartOfDocument, Keys.Shift | Keys.Control | Keys.Home),
            (KeyboardCommand.SelectEndOfDocument, Keys.Shift | Keys.Control | Keys.End),

            (KeyboardCommand.DeletePreviousChar, Keys.Back),
            (KeyboardCommand.DeleteNextChar, Keys.Delete),
            (KeyboardCommand.DeletePreviousWord, Keys.Control | Keys.Back),
            (KeyboardCommand.DeleteNextWord, Keys.Control | Keys.Delete),

            (KeyboardCommand.ScrollUp, Keys.Control | Keys.Up),
            (KeyboardCommand.ScrollDown, Keys.Control | Keys.Down),
            (KeyboardCommand.Find, Keys.Control | Keys.F),
            (KeyboardCommand.FindNext, Keys.F3),
            (KeyboardCommand.FindPrevious, Keys.Shift | Keys.F3),
            (KeyboardCommand.SearchAndReplace, Keys.Control | Keys.H),
            (KeyboardCommand.ReplaceNext, Keys.Alt | Keys.R)
        );

        public static readonly OperatingSystem Linux = new OperatingSystem("Linux",
            (KeyboardCommand.Undo, Keys.Control | Keys.Z),
            (KeyboardCommand.Redo, Keys.Control | Keys.Y),
            (KeyboardCommand.Cut, Keys.Control | Keys.X),
            (KeyboardCommand.Copy, Keys.Control | Keys.C),
            (KeyboardCommand.Paste, Keys.Control | Keys.V),
            (KeyboardCommand.InsertNewLine, Keys.Return),
            (KeyboardCommand.InsertTab, Keys.Tab),

            (KeyboardCommand.MovePreviousChar, Keys.Left),
            (KeyboardCommand.MoveNextChar, Keys.Right),
            (KeyboardCommand.MovePreviousWord, Keys.Control | Keys.Left),
            (KeyboardCommand.MoveNextWord, Keys.Control | Keys.Right),
            (KeyboardCommand.MoveStartOfLine, Keys.Home),
            (KeyboardCommand.MoveEndOfLine, Keys.End),
            (KeyboardCommand.MovePreviousLine, Keys.Up),
            (KeyboardCommand.MoveNextLine, Keys.Down),
            (KeyboardCommand.MovePageUp, Keys.PageUp),
            (KeyboardCommand.MovePageDown, Keys.PageDown),
            (KeyboardCommand.MoveStartOfDocument, Keys.Control | Keys.Home),
            (KeyboardCommand.MoveEndOfDocument, Keys.Control | Keys.End),

            (KeyboardCommand.SelectAll, Keys.Control | Keys.A),

            (KeyboardCommand.SelectPreviousChar, Keys.Shift | Keys.Left),
            (KeyboardCommand.SelectNextChar, Keys.Shift | Keys.Right),
            (KeyboardCommand.SelectPreviousWord, Keys.Shift | Keys.Control | Keys.Left),
            (KeyboardCommand.SelectNextWord, Keys.Shift | Keys.Control | Keys.Right),
            (KeyboardCommand.SelectStartOfLine, Keys.Shift | Keys.Home),
            (KeyboardCommand.SelectEndOfLine, Keys.Shift | Keys.End),
            (KeyboardCommand.SelectPreviousLine, Keys.Shift | Keys.Up),
            (KeyboardCommand.SelectNextLine, Keys.Shift | Keys.Down),
            (KeyboardCommand.SelectPageUp, Keys.Shift | Keys.PageUp),
            (KeyboardCommand.SelectPageDown, Keys.Shift | Keys.PageDown),
            (KeyboardCommand.SelectStartOfDocument, Keys.Shift | Keys.Control | Keys.Home),
            (KeyboardCommand.SelectEndOfDocument, Keys.Shift | Keys.Control | Keys.End),

            (KeyboardCommand.DeletePreviousChar, Keys.Back),
            (KeyboardCommand.DeleteNextChar, Keys.Delete),
            (KeyboardCommand.DeletePreviousWord, Keys.Control | Keys.Back),
            (KeyboardCommand.DeleteNextWord, Keys.Control | Keys.Delete),

            (KeyboardCommand.ScrollUp, Keys.Control | Keys.Up),
            (KeyboardCommand.ScrollDown, Keys.Control | Keys.Down),
            (KeyboardCommand.Find, Keys.Control | Keys.F),
            (KeyboardCommand.FindNext, Keys.F3),
            (KeyboardCommand.FindPrevious, Keys.Shift | Keys.F3),
            (KeyboardCommand.SearchAndReplace, Keys.Control | Keys.H),
            (KeyboardCommand.ReplaceNext, Keys.Alt | Keys.R)
        );

        public static readonly OperatingSystem MacOS = new OperatingSystem("macOS",
            (KeyboardCommand.Undo, Keys.LWin | Keys.Z),
            (KeyboardCommand.Redo, Keys.Shift | Keys.LWin | Keys.Z),
            (KeyboardCommand.Cut, Keys.LWin | Keys.X),
            (KeyboardCommand.Copy, Keys.LWin | Keys.C),
            (KeyboardCommand.Paste, Keys.LWin | Keys.V),
            (KeyboardCommand.InsertNewLine, Keys.Return),
            (KeyboardCommand.InsertTab, Keys.Tab),

            (KeyboardCommand.MovePreviousChar, Keys.Left),
            (KeyboardCommand.MoveNextChar, Keys.Right),
            (KeyboardCommand.MovePreviousWord, Keys.Alt | Keys.Left),
            (KeyboardCommand.MoveNextWord, Keys.Alt | Keys.Right),
            (KeyboardCommand.MoveStartOfLine, Keys.LWin | Keys.Left),
            (KeyboardCommand.MoveStartOfLine, Keys.Control | Keys.A),
            (KeyboardCommand.MoveEndOfLine, Keys.LWin | Keys.Right),
            (KeyboardCommand.MoveEndOfLine, Keys.Control | Keys.E),
            (KeyboardCommand.MovePreviousLine, Keys.Up),
            (KeyboardCommand.MovePreviousLine, Keys.Control | Keys.P),
            (KeyboardCommand.MoveNextLine, Keys.Down),
            (KeyboardCommand.MoveNextLine, Keys.Control | Keys.N),
            (KeyboardCommand.MovePageUp, Keys.PageUp),
            (KeyboardCommand.MovePageDown, Keys.PageDown),
            (KeyboardCommand.MoveStartOfDocument, Keys.LWin | Keys.Up),
            (KeyboardCommand.MoveEndOfDocument, Keys.LWin | Keys.Down),

            (KeyboardCommand.SelectAll, Keys.LWin | Keys.A),

            (KeyboardCommand.SelectPreviousChar, Keys.Shift | Keys.Left),
            (KeyboardCommand.SelectNextChar, Keys.Shift | Keys.Right),
            (KeyboardCommand.SelectPreviousWord, Keys.Shift | Keys.Alt | Keys.Left),
            (KeyboardCommand.SelectNextWord, Keys.Shift | Keys.Alt | Keys.Right),
            (KeyboardCommand.SelectStartOfLine, Keys.Shift | Keys.LWin | Keys.Left),
            (KeyboardCommand.SelectStartOfLine, Keys.Shift | Keys.Control | Keys.A),
            (KeyboardCommand.SelectEndOfLine, Keys.Shift | Keys.LWin | Keys.Right),
            (KeyboardCommand.SelectEndOfLine, Keys.Shift | Keys.Control | Keys.E),
            (KeyboardCommand.SelectPreviousLine, Keys.Shift | Keys.Up),
            (KeyboardCommand.SelectPreviousLine, Keys.Shift | Keys.Control | Keys.P),
            (KeyboardCommand.SelectNextLine, Keys.Shift | Keys.Down),
            (KeyboardCommand.SelectNextLine, Keys.Shift | Keys.Control | Keys.N),
            (KeyboardCommand.SelectPageUp, Keys.Shift | Keys.PageUp),
            (KeyboardCommand.SelectPageDown, Keys.Shift | Keys.PageDown),
            (KeyboardCommand.SelectStartOfDocument, Keys.Shift | Keys.LWin | Keys.Up),
            (KeyboardCommand.SelectEndOfDocument, Keys.Shift | Keys.LWin | Keys.Down),

            (KeyboardCommand.DeletePreviousChar, Keys.Back),
            (KeyboardCommand.DeleteNextChar, Keys.Delete),
            (KeyboardCommand.DeletePreviousWord, Keys.Alt| Keys.Back),
            (KeyboardCommand.DeleteNextWord, Keys.Alt | Keys.Delete),

            (KeyboardCommand.ScrollUp, Keys.Alt | Keys.Up),
            (KeyboardCommand.ScrollDown, Keys.Alt | Keys.Down),
            (KeyboardCommand.FindNext, Keys.LWin | Keys.G),
            (KeyboardCommand.FindPrevious, Keys.Shift | Keys.LWin | Keys.G),
            (KeyboardCommand.SearchAndReplace, Keys.LWin | Keys.F)
        );
    }
}