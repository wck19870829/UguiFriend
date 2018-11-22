using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(UguiKeyboard))]
    /// <summary>
    /// Windows系统下键盘同步
    /// </summary>
    public class UguiKeyboardSyncWin : MonoBehaviour
    {
        UguiKeyboard keyboard;

        private void Awake()
        {
            keyboard = GetComponent<UguiKeyboard>();
            if (keyboard == null)
                throw new Exception("Ugui keyboard is null.");

            keyboard.callNativeKeyboard = true;
            keyboard.OnKeyDown += OnKeyDown;
            keyboard.OnKeyUp += OnKeyUp;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                string[] langs;
                TSFWapper.GetCurrentLang(out langs);
                foreach (var lang in langs)
                {
                    Debug.Log(lang);
                }
            }

            //var processWnd = GetProcessWnd();
            //var inputStr = CurrentCompStr(processWnd);
            //UnityEngine.Debug.LogFormat("str:{0}",inputStr);
        }

        protected virtual bool IsSwitchingInputMethod(Event e)
        {
            if (e.control)
            {
                if (e.keyCode == KeyCode.Space || e.shift)
                {
                    return true;
                }
            }

            return false;
        }

        void OnKeyDown(Event e)
        {
            if (IsSwitchingInputMethod(e))
            {
                //切换输入法

            }

            KeyDown(e.keyCode);
        }

        void OnKeyUp(Event e)
        {
            KeyUp(e.keyCode);
        }

        void KeyDown(KeyCode keyCode)
        {
            if (keyCodeDict.ContainsKey(keyCode))
            {
                if (keyboard != null && keyboard.callNativeKeyboard)
                {
                    KeyDown(keyCodeDict[keyCode]);
                }
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("Key code is invalid:{0}", keyCode);
            }
        }

        void KeyUp(KeyCode keyCode)
        {
            if (keyCodeDict.ContainsKey(keyCode))
            {
                if (keyboard != null && keyboard.callNativeKeyboard)
                {
                    KeyUp(keyCodeDict[keyCode]);
                }
            }
            else
            {
                UnityEngine.Debug.LogErrorFormat("Key code is invalid:{0}", keyCode);
            }
        }

        void KeyDown(VirtualKeyCode virtualKeyCode)
        {
            var input = new INPUT();
            var keyboard = new KEYBDINPUT();
            keyboard.KeyCode = (ushort)virtualKeyCode;
            input.Data = new MOUSEKEYBDHARDWAREINPUT();
            input.Data.Keyboard = keyboard;
            input.Type = (uint)InputType.Keyboard;
            var inputs = new INPUT[] { input };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        void KeyUp(VirtualKeyCode virtualKeyCode)
        {
            var input = new INPUT();
            var keyboard = new KEYBDINPUT();
            keyboard.KeyCode = (ushort)virtualKeyCode;
            keyboard.Flags = (uint)KeyboardFlag.KeyUp;
            input.Data = new MOUSEKEYBDHARDWAREINPUT();
            input.Data.Keyboard = keyboard;
            input.Type = (uint)InputType.Keyboard;
            var inputs = new INPUT[] { input };
            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        #region KeyCode-->VirtualKeyCode

        /// <summary>
        /// KeyCode到VirtualKeyCode的映射
        /// </summary>
        static readonly Dictionary<KeyCode, VirtualKeyCode> keyCodeDict = new Dictionary<KeyCode, VirtualKeyCode>()
        {
            {KeyCode.Backspace,VirtualKeyCode.BACK },
            {KeyCode.Tab,VirtualKeyCode.TAB },
            {KeyCode.Clear,VirtualKeyCode.CLEAR },
            {KeyCode.Return,VirtualKeyCode.RETURN },
            {KeyCode.Pause,VirtualKeyCode.PAUSE },
            {KeyCode.Escape,VirtualKeyCode.ESCAPE },
            {KeyCode.CapsLock,VirtualKeyCode.CAPITAL },
            {KeyCode.Space,VirtualKeyCode.SPACE },
            {KeyCode.PageUp,VirtualKeyCode.PRIOR },
            {KeyCode.PageDown,VirtualKeyCode.NEXT },
            {KeyCode.End,VirtualKeyCode.END },
            {KeyCode.Home,VirtualKeyCode.HOME },
            {KeyCode.LeftArrow,VirtualKeyCode.LEFT },
            {KeyCode.UpArrow,VirtualKeyCode.UP },
            {KeyCode.RightArrow,VirtualKeyCode.RIGHT },
            {KeyCode.DownArrow,VirtualKeyCode.DOWN },
            {KeyCode.Print,VirtualKeyCode.PRINT },
            {KeyCode.Insert,VirtualKeyCode.INSERT },
            {KeyCode.Delete,VirtualKeyCode.DELETE },
            {KeyCode.Help,VirtualKeyCode.HELP },
            {KeyCode.Alpha0,VirtualKeyCode.VK_0 },
            {KeyCode.Alpha1,VirtualKeyCode.VK_1 },
            {KeyCode.Alpha2,VirtualKeyCode.VK_2 },
            {KeyCode.Alpha3,VirtualKeyCode.VK_3 },
            {KeyCode.Alpha4,VirtualKeyCode.VK_4 },
            {KeyCode.Alpha5,VirtualKeyCode.VK_5 },
            {KeyCode.Alpha6,VirtualKeyCode.VK_6 },
            {KeyCode.Alpha7,VirtualKeyCode.VK_7 },
            {KeyCode.Alpha8,VirtualKeyCode.VK_8 },
            {KeyCode.Alpha9,VirtualKeyCode.VK_9 },
            {KeyCode.A,VirtualKeyCode.VK_A },
            {KeyCode.B,VirtualKeyCode.VK_B },
            {KeyCode.C,VirtualKeyCode.VK_C },
            {KeyCode.D,VirtualKeyCode.VK_D },
            {KeyCode.E,VirtualKeyCode.VK_E },
            {KeyCode.F,VirtualKeyCode.VK_F },
            {KeyCode.G,VirtualKeyCode.VK_G },
            {KeyCode.H,VirtualKeyCode.VK_H },
            {KeyCode.I,VirtualKeyCode.VK_I },
            {KeyCode.J,VirtualKeyCode.VK_J },
            {KeyCode.K,VirtualKeyCode.VK_K },
            {KeyCode.L,VirtualKeyCode.VK_L },
            {KeyCode.M,VirtualKeyCode.VK_M },
            {KeyCode.N,VirtualKeyCode.VK_N },
            {KeyCode.O,VirtualKeyCode.VK_O },
            {KeyCode.P,VirtualKeyCode.VK_P },
            {KeyCode.Q,VirtualKeyCode.VK_Q },
            {KeyCode.R,VirtualKeyCode.VK_R },
            {KeyCode.S,VirtualKeyCode.VK_S },
            {KeyCode.T,VirtualKeyCode.VK_T },
            {KeyCode.U,VirtualKeyCode.VK_U },
            {KeyCode.V,VirtualKeyCode.VK_V },
            {KeyCode.W,VirtualKeyCode.VK_W },
            {KeyCode.X,VirtualKeyCode.VK_X },
            {KeyCode.Y,VirtualKeyCode.VK_Y },
            {KeyCode.Z,VirtualKeyCode.VK_Z },
            {KeyCode.LeftWindows,VirtualKeyCode.LWIN },
            {KeyCode.RightWindows,VirtualKeyCode.RWIN },
            {KeyCode.Keypad0,VirtualKeyCode.NUMPAD0 },
            {KeyCode.Keypad1,VirtualKeyCode.NUMPAD1 },
            {KeyCode.Keypad2,VirtualKeyCode.NUMPAD2 },
            {KeyCode.Keypad3,VirtualKeyCode.NUMPAD3 },
            {KeyCode.Keypad4,VirtualKeyCode.NUMPAD4 },
            {KeyCode.Keypad5,VirtualKeyCode.NUMPAD5 },
            {KeyCode.Keypad6,VirtualKeyCode.NUMPAD6 },
            {KeyCode.Keypad7,VirtualKeyCode.NUMPAD7 },
            {KeyCode.Keypad8,VirtualKeyCode.NUMPAD8 },
            {KeyCode.Keypad9,VirtualKeyCode.NUMPAD9 },
            {KeyCode.KeypadMultiply,VirtualKeyCode.MULTIPLY },
            {KeyCode.Plus,VirtualKeyCode.ADD },
            {KeyCode.Semicolon,VirtualKeyCode.SEPARATOR },
            {KeyCode.Minus,VirtualKeyCode.SUBTRACT },
            {KeyCode.KeypadMinus,VirtualKeyCode.SUBTRACT },
            {KeyCode.Period,VirtualKeyCode.DECIMAL },
            {KeyCode.KeypadPeriod, VirtualKeyCode.DECIMAL},
            {KeyCode.KeypadDivide,VirtualKeyCode.DIVIDE },
            {KeyCode.Slash,VirtualKeyCode.DIVIDE },
            {KeyCode.F1, VirtualKeyCode.F1},
            {KeyCode.F2, VirtualKeyCode.F2},
            {KeyCode.F3, VirtualKeyCode.F3},
            {KeyCode.F4, VirtualKeyCode.F4},
            {KeyCode.F5, VirtualKeyCode.F5},
            {KeyCode.F6, VirtualKeyCode.F6},
            {KeyCode.F7, VirtualKeyCode.F7},
            {KeyCode.F8, VirtualKeyCode.F8},
            {KeyCode.F9, VirtualKeyCode.F9},
            {KeyCode.F10, VirtualKeyCode.F10},
            {KeyCode.F11, VirtualKeyCode.F11},
            {KeyCode.F12, VirtualKeyCode.F12},
            {KeyCode.Numlock,VirtualKeyCode.NUMLOCK },
            {KeyCode.ScrollLock,VirtualKeyCode.SCROLL },
            {KeyCode.LeftShift,VirtualKeyCode.LSHIFT },
            {KeyCode.RightShift,VirtualKeyCode.RSHIFT },
            {KeyCode.LeftControl,VirtualKeyCode.LCONTROL },
            {KeyCode.RightControl,VirtualKeyCode.RCONTROL },
            {KeyCode.LeftAlt,VirtualKeyCode.LMENU},
            {KeyCode.RightAlt,VirtualKeyCode.RMENU }
        };

        #endregion

        #region Windows系统方法

        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt32 SendInput(UInt32 numberOfInputs, INPUT[] inputs, Int32 sizeOfInputStructure);

        //[DllImport("imm32.dll")]
        //static extern int ImmGetCandidateListA(IntPtr hIMC, deIndex, lpCandList, dwBufLen);

        [DllImport("imm32.dll")]
        static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("imm32.dll")]
        static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        static extern int ImmGetCompositionStringW(IntPtr hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);

        const int GCS_COMPSTR = 8;

        public string CurrentCompStr(IntPtr handle)
        {
            int readType = GCS_COMPSTR;

            IntPtr hIMC = ImmGetContext(handle);
            try
            {
                int strLen = ImmGetCompositionStringW(hIMC, readType, null, 0);

                if (strLen > 0)
                {
                    byte[] buffer = new byte[strLen];

                    ImmGetCompositionStringW(hIMC, readType, buffer, strLen);

                    return Encoding.Unicode.GetString(buffer);
                }
                else
                {
                    return string.Empty;
                }
            }
            finally
            {
                ImmReleaseContext(handle, hIMC);
            }
        }

        public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

        [DllImport("kernel32.dll")]
        public static extern void SetLastError(uint dwErrCode);

        public static IntPtr GetProcessWnd()
        {
            IntPtr ptrWnd = IntPtr.Zero;
            uint pid = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;  // 当前进程 ID

            bool bResult = EnumWindows(new WNDENUMPROC(delegate (IntPtr hwnd, uint lParam)
            {
                uint id = 0;

                if (GetParent(hwnd) == IntPtr.Zero)
                {
                    GetWindowThreadProcessId(hwnd, ref id);
                    if (id == lParam)    // 找到进程对应的主窗口句柄
                    {
                        ptrWnd = hwnd;   // 把句柄缓存起来
                        SetLastError(0);    // 设置无错误
                        return false;   // 返回 false 以终止枚举窗口
                    }
                }

                return true;

            }), pid);

            return (!bResult && Marshal.GetLastWin32Error() == 0) ? ptrWnd : IntPtr.Zero;
        }

        struct INPUT
        {
            public UInt32 Type;
            public MOUSEKEYBDHARDWAREINPUT Data;
        }

        enum InputType : uint
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2
        }

        [StructLayout(LayoutKind.Explicit)]
        struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;

            [FieldOffset(0)]
            public KEYBDINPUT Keyboard;

            [FieldOffset(0)]
            public HARDWAREINPUT Hardware;
        }

        [Flags]
        internal enum KeyboardFlag : uint
        {
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            ScanCode = 0x0008,
        }

        struct MOUSEINPUT
        {
            public Int32 X;
            public Int32 Y;
            public UInt32 MouseData;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }

        internal struct KEYBDINPUT
        {
            public UInt16 KeyCode;
            public UInt16 Scan;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }

        internal struct HARDWAREINPUT
        {
            public UInt32 Msg;
            public UInt16 ParamL;
            public UInt16 ParamH;
        }

        #endregion
    }
}