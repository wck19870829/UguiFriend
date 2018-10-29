using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(UguiKeyboard))]
    /// <summary>
    /// Windows系统下键盘同步
    /// </summary>
    public sealed class UguiKeyboardSyncWin : MonoBehaviour
    {
        UguiKeyboard keyboard;

        private void Awake()
        {
            keyboard = GetComponent<UguiKeyboard>();
            keyboard.OnKey += OnKeyDown;
            keyboard.OnKeyDown += OnKeyDown;
            keyboard.OnKeyUp += OnKeyUp;
        }

        private void OnGUI()
        {
            //Debug.LogFormat("{0}  {1}", Event.current.keyCode, Event.current.type);
        }

        void OnKeyDown(Event e)
        {
            var ki = new tagKEYBDINPUT();
            ki.wVk = VK_LWIN;
            ki.dwFlags = 0;
            var input = new Input();
            input.type = (int)InputType.Keyboard;
            SendInput(1, ref input, Marshal.SizeOf(input.GetType()));
        }

        void OnKeyUp(Event e)
        {
            var ki = new tagKEYBDINPUT();
            ki.wVk = VK_LWIN;
            ki.dwFlags = KeyEvenTfKeyUp;
            var input = new Input();
            input.type = (int)InputType.Keyboard;
            SendInput(1,ref input,Marshal.SizeOf(input));
        }

        #region 引用win32api方法

        [DllImport("user32.dll")]
        static extern int SendInput(UInt32 nInputs,ref Input pInput, int cbSize);

        //[DllImport("user32.dll")]
        //static extern int SendInput(UInt32 nInputs, Input[] pInputs, int cbSize);

        [StructLayout(LayoutKind.Explicit)]
        struct Input
        {
            [FieldOffset(0)] public Int32 type;
            [FieldOffset(4)] public MouseInput mi;
            [FieldOffset(4)] public tagKEYBDINPUT ki;
            [FieldOffset(4)] public tagHARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MouseInput
        {
            public Int32 dx;
            public Int32 dy;
            public Int32 Mousedata;
            public Int32 dwFlag;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct tagKEYBDINPUT
        {
            public Int16 wVk;
            public Int16 wScan;
            public Int32 dwFlags;
            public Int32 time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct tagHARDWAREINPUT
        {
            public Int32 uMsg;
            public Int16 wParamL;
            public Int16 wParamH;
        }

        enum InputType
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 2,
        }

        [Flags()]
        enum KeyEventTF
        {
            ExtendedKey = 0x0001,
            KeyUp = 0x0002,
            Unicode = 0x0004,
            ScanCode = 0x0008,
        }

        #endregion

        #region 常量定义

        public const int KeyEvenTfExtendedKey = 0x0001;
        public const int KeyEvenTfKeyUp = 0x0002;
        public const int KeyEvenTfScanCode = 0x0008;
        public const int KeyEvenTfUnicode = 0x0004;

        public const byte VK_LWIN = 0x5B;

        public const byte vbKeyLButton = 0x1;    // 鼠标左键
        public const byte vbKeyRButton = 0x2;    // 鼠标右键
        public const byte vbKeyCancel = 0x3;     // CANCEL 键
        public const byte vbKeyMButton = 0x4;    // 鼠标中键
        public const byte vbKeyBack = 0x8;       // BACKSPACE 键
        public const byte vbKeyTab = 0x9;        // TAB 键
        public const byte vbKeyClear = 0xC;      // CLEAR 键
        public const byte vbKeyReturn = 0xD;     // ENTER 键
        public const byte vbKeyShift = 0x10;     // SHIFT 键
        public const byte vbKeyControl = 0x11;   // CTRL 键
        public const byte vbKeyAlt = 18;         // Alt 键  (键码18)
        public const byte vbKeyMenu = 0x12;      // MENU 键
        public const byte vbKeyPause = 0x13;     // PAUSE 键
        public const byte vbKeyCapital = 0x14;   // CAPS LOCK 键
        public const byte vbKeyEscape = 0x1B;    // ESC 键
        public const byte vbKeySpace = 0x20;     // SPACEBAR 键
        public const byte vbKeyPageUp = 0x21;    // PAGE UP 键
        public const byte vbKeyEnd = 0x23;       // End 键
        public const byte vbKeyHome = 0x24;      // HOME 键
        public const byte vbKeyLeft = 0x25;      // LEFT ARROW 键
        public const byte vbKeyUp = 0x26;        // UP ARROW 键
        public const byte vbKeyRight = 0x27;     // RIGHT ARROW 键
        public const byte vbKeyDown = 0x28;      // DOWN ARROW 键
        public const byte vbKeySelect = 0x29;    // Select 键
        public const byte vbKeyPrint = 0x2A;     // PRINT SCREEN 键
        public const byte vbKeyExecute = 0x2B;   // EXECUTE 键
        public const byte vbKeySnapshot = 0x2C;  // SNAPSHOT 键
        public const byte vbKeyDelete = 0x2E;    // Delete 键
        public const byte vbKeyHelp = 0x2F;      // HELP 键
        public const byte vbKeyNumlock = 0x90;   // NUM LOCK 键

        //常用键 字母键A到Z
        public const byte vbKeyA = 65;
        public const byte vbKeyB = 66;
        public const byte vbKeyC = 67;
        public const byte vbKeyD = 68;
        public const byte vbKeyE = 69;
        public const byte vbKeyF = 70;
        public const byte vbKeyG = 71;
        public const byte vbKeyH = 72;
        public const byte vbKeyI = 73;
        public const byte vbKeyJ = 74;
        public const byte vbKeyK = 75;
        public const byte vbKeyL = 76;
        public const byte vbKeyM = 77;
        public const byte vbKeyN = 78;
        public const byte vbKeyO = 79;
        public const byte vbKeyP = 80;
        public const byte vbKeyQ = 81;
        public const byte vbKeyR = 82;
        public const byte vbKeyS = 83;
        public const byte vbKeyT = 84;
        public const byte vbKeyU = 85;
        public const byte vbKeyV = 86;
        public const byte vbKeyW = 87;
        public const byte vbKeyX = 88;
        public const byte vbKeyY = 89;
        public const byte vbKeyZ = 90;

        //数字键盘0到9
        public const byte vbKey0 = 48;    // 0 键
        public const byte vbKey1 = 49;    // 1 键
        public const byte vbKey2 = 50;    // 2 键
        public const byte vbKey3 = 51;    // 3 键
        public const byte vbKey4 = 52;    // 4 键
        public const byte vbKey5 = 53;    // 5 键
        public const byte vbKey6 = 54;    // 6 键
        public const byte vbKey7 = 55;    // 7 键
        public const byte vbKey8 = 56;    // 8 键
        public const byte vbKey9 = 57;    // 9 键


        public const byte vbKeyNumpad0 = 0x60;    //0 键
        public const byte vbKeyNumpad1 = 0x61;    //1 键
        public const byte vbKeyNumpad2 = 0x62;    //2 键
        public const byte vbKeyNumpad3 = 0x63;    //3 键
        public const byte vbKeyNumpad4 = 0x64;    //4 键
        public const byte vbKeyNumpad5 = 0x65;    //5 键
        public const byte vbKeyNumpad6 = 0x66;    //6 键
        public const byte vbKeyNumpad7 = 0x67;    //7 键
        public const byte vbKeyNumpad8 = 0x68;    //8 键
        public const byte vbKeyNumpad9 = 0x69;    //9 键
        public const byte vbKeyMultiply = 0x6A;   // MULTIPLICATIONSIGN(*)键
        public const byte vbKeyAdd = 0x6B;        // PLUS SIGN(+) 键
        public const byte vbKeySeparator = 0x6C;  // ENTER 键
        public const byte vbKeySubtract = 0x6D;   // MINUS SIGN(-) 键
        public const byte vbKeyDecimal = 0x6E;    // DECIMAL POINT(.) 键
        public const byte vbKeyDivide = 0x6F;     // DIVISION SIGN(/) 键


        //F1到F12按键
        public const byte vbKeyF1 = 0x70;   //F1 键
        public const byte vbKeyF2 = 0x71;   //F2 键
        public const byte vbKeyF3 = 0x72;   //F3 键
        public const byte vbKeyF4 = 0x73;   //F4 键
        public const byte vbKeyF5 = 0x74;   //F5 键
        public const byte vbKeyF6 = 0x75;   //F6 键
        public const byte vbKeyF7 = 0x76;   //F7 键
        public const byte vbKeyF8 = 0x77;   //F8 键
        public const byte vbKeyF9 = 0x78;   //F9 键
        public const byte vbKeyF10 = 0x79;  //F10 键
        public const byte vbKeyF11 = 0x7A;  //F11 键
        public const byte vbKeyF12 = 0x7B;  //F12 键

        #endregion
    }
}