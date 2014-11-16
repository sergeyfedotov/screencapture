using System;
using System.Windows.Forms;
using System.Windows;

namespace Screencapture
{
    /// <summary>
    /// http://www.liensberger.it/web/blog/?p=207
    /// </summary>
    public sealed class KeyboardHook : IDisposable
    {
        private class ListenerWindow : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;

            public event EventHandler<HotKeyPressEventArgs> KeyPressed;

            public ListenerWindow()
            {
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_HOTKEY)
                {
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    if (KeyPressed != null)
                    {
                        KeyPressed(this, new HotKeyPressEventArgs(modifier, key));
                    }
                }
            }

            public void Dispose()
            {
                this.DestroyHandle();
            }
        }

        private ListenerWindow Window = new ListenerWindow();
        
        private int CurrentId;

        public event EventHandler<HotKeyPressEventArgs> KeyPressed;

        public KeyboardHook()
        {
            Window.KeyPressed += delegate(object sender, HotKeyPressEventArgs args)
            {
                EventHandler<HotKeyPressEventArgs> eh = KeyPressed;

                if (eh != null)
                {
                    eh(this, args);
                }
            };
        }

        public void RegisterHotKey(ModifierKeys modifiers, Keys key)
        {
            CurrentId = CurrentId + 1;

            if (!NativeMethods.RegisterHotKey(Window.Handle, CurrentId, (uint)modifiers, (uint)key))
            {
                throw new InvalidOperationException("Couldn't register the hot key.");
            }
        }

        public void Dispose()
        {
            for (int i = CurrentId; i > 0; i -= 1)
            {
                NativeMethods.UnregisterHotKey(Window.Handle, i);
            }

            Window.Dispose();
        }
    }

    public class HotKeyPressEventArgs : EventArgs
    {
        public ModifierKeys Modifiers
        {
            private set;
            get;
        }

        public Keys Key 
        {
            private set;
            get; 
        }

        internal HotKeyPressEventArgs(ModifierKeys modifiers, Keys key)
        {
            Modifiers = modifiers;
            Key = key;
        }
    }

    [Flags]
    public enum ModifierKeys : uint
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4,
        Windows = 8
    }
}
