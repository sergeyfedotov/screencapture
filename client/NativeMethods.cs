using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Screencapture
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("gdi32.dll")]
        internal static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
            wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        
        [DllImport("user32.dll")]
        internal static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        
        [DllImport("gdi32.dll")]
        internal static extern IntPtr DeleteDC(IntPtr hDc);
        
        [DllImport("gdi32.dll")]
        internal static extern IntPtr DeleteObject(IntPtr hDc);
        
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        
        [DllImport("gdi32.dll")]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDesktopWindow();
        
        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowDC(IntPtr ptr);
    }
}
