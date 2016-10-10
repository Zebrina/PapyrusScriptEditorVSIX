﻿using System;
using System.Runtime.InteropServices;

namespace Papyrus.Common {
    internal static class NativeMethods {
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AllocConsole();
        
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
        
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
