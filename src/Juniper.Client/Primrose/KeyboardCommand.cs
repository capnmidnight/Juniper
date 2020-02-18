namespace Juniper.Primrose
{
    public enum KeyboardCommand
    {
        None,

        Undo,
        Redo,
        Cut,
        Copy,
        Paste,
        InsertNewLine,
        InsertTab,

        MovePreviousChar,
        MoveNextChar,
        MovePreviousWord,
        MoveNextWord,
        MoveStartOfLine,
        MoveEndOfLine,
        MovePreviousLine,
        MoveNextLine,
        MovePageDown,
        MovePageUp,
        MoveStartOfDocument,
        MoveEndOfDocument,

        SelectAll,

        SelectPreviousChar,
        SelectNextChar,
        SelectPreviousWord,
        SelectNextWord,
        SelectStartOfLine,
        SelectEndOfLine,
        SelectPreviousLine,
        SelectNextLine,
        SelectPageDown,
        SelectPageUp,
        SelectStartOfDocument,
        SelectEndOfDocument,

        DeletePreviousChar,
        DeleteNextChar,
        DeletePreviousWord,
        DeleteNextWord,

        ScrollUp,
        ScrollDown,
        Find,
        FindNext,
        FindPrevious,
        SearchAndReplace,
        ReplaceNext
    }
}