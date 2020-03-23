using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Juniper.VeldridIntegration.WPFSupport
{
    public abstract class Win32HwndControl : HwndHost
    {
        private const string WindowClass = "HwndWrapper";

        protected Win32HwndControl()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected IntPtr Hwnd { get; private set; }
        protected bool HwndInitialized { get; private set; }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Initialize();
            HwndInitialized = true;

            Loaded -= OnLoaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Uninitialize();
            HwndInitialized = false;

            Unloaded -= OnUnloaded;

            Dispose();
        }

        protected abstract void Initialize();
        protected abstract void Uninitialize();
        protected abstract void Resized();

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var wndClass = new NativeMethods.WndClassEx();
            wndClass.cbSize = (uint)Marshal.SizeOf(wndClass);
            wndClass.hInstance = NativeMethods.GetModuleHandle(null);
            wndClass.lpfnWndProc = NativeMethods.DefaultWindowProc;
            wndClass.lpszClassName = WindowClass;
            wndClass.hCursor = NativeMethods.LoadCursor(IntPtr.Zero, NativeMethods.IDC_ARROW);
            _ = NativeMethods.RegisterClassEx(ref wndClass);

            Hwnd = NativeMethods.CreateWindowEx(
                0, WindowClass, "", NativeMethods.WS_CHILD | NativeMethods.WS_VISIBLE,
                0, 0, (int)Width, (int)Height, hwndParent.Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            return new HandleRef(this, Hwnd);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            _ = NativeMethods.DestroyWindow(hwnd.Handle);
            Hwnd = IntPtr.Zero;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            UpdateWindowPos();

            base.OnRenderSizeChanged(sizeInfo);

            if (HwndInitialized)
            {
                Resized();
            }
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_LBUTTONDOWN:
                RaiseMouseEvent(MouseButton.Left, Mouse.MouseDownEvent);
                break;

                case NativeMethods.WM_LBUTTONUP:
                RaiseMouseEvent(MouseButton.Left, Mouse.MouseUpEvent);
                break;

                case NativeMethods.WM_RBUTTONDOWN:
                RaiseMouseEvent(MouseButton.Right, Mouse.MouseDownEvent);
                break;

                case NativeMethods.WM_RBUTTONUP:
                RaiseMouseEvent(MouseButton.Right, Mouse.MouseUpEvent);
                break;
            }

            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }

        private void RaiseMouseEvent(MouseButton button, RoutedEvent @event)
        {
            RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, button)
            {
                RoutedEvent = @event,
                Source = this
            });
        }
    }
}
