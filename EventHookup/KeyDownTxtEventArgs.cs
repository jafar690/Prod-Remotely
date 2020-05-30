using System;

namespace Silgred.Event.Hookup
{
    public class KeyDownTxtEventArgs : EventArgs
    {
        public KeyDownTxtEventArgs(KeyEventArgsExt keyEvent, string chars)
        {
            KeyEvent = keyEvent;
            Chars = chars ?? string.Empty;
        }

        public KeyEventArgsExt KeyEvent { get; }
        public string Chars { get; }
    }
}