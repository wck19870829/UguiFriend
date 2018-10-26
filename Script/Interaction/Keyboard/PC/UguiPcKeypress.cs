using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// PC按键
    /// </summary>
    public class UguiPcKeypress : UguiKeypress
    {
        protected static Dictionary<KeyCode, KeyCode> numLockDict;         //NumLock锁定时按键映射
        protected static Dictionary<KeyCode, string> keyCodeNameDict;
        protected static Dictionary<KeyCode, char> shiftOnCharDict;
        protected static Dictionary<KeyCode, char> shiftOffCharDict;

        protected UguiPcKeyboard pcKeyboard;

        static UguiPcKeypress()
        {
            numLockDict = new Dictionary<KeyCode, KeyCode>()
            {
                {KeyCode.Keypad0,KeyCode.Insert },
                {KeyCode.Keypad1,KeyCode.End },
                {KeyCode.Keypad2,KeyCode.DownArrow },
                {KeyCode.Keypad3,KeyCode.PageDown },
                {KeyCode.Keypad4,KeyCode.LeftArrow },
                {KeyCode.Keypad6,KeyCode.RightArrow },
                {KeyCode.Keypad7,KeyCode.Home },
                {KeyCode.Keypad8,KeyCode.UpArrow },
                {KeyCode.Keypad9,KeyCode.PageUp },
                {KeyCode.Period,KeyCode.Delete }
            };
            keepPressSet = new HashSet<KeyCode>()
            {
                KeyCode.LeftShift,
                KeyCode.RightShift,
                KeyCode.LeftCommand,
                KeyCode.RightCommand,
                KeyCode.LeftControl,
                KeyCode.RightControl,
                KeyCode.CapsLock,
                KeyCode.LeftWindows,
                KeyCode.RightWindows,
                KeyCode.ScrollLock,
                KeyCode.LeftAlt,
                KeyCode.RightAlt,

                //虚拟键盘特殊处理
                KeyCode.CapsLock,
                KeyCode.ScrollLock,
                KeyCode.Numlock
            };
            shiftOffCharDict = new Dictionary<KeyCode, char>()
            {
                { KeyCode.Alpha0,'0'},
                { KeyCode.Alpha1,'1'},
                { KeyCode.Alpha2,'2'},
                { KeyCode.Alpha3,'3'},
                { KeyCode.Alpha4,'4'},
                { KeyCode.Alpha5,'5'},
                { KeyCode.Alpha6,'6'},
                { KeyCode.Alpha7,'7'},
                { KeyCode.Alpha8,'8'},
                { KeyCode.Alpha9,'9'},
                { KeyCode.BackQuote,'`'},
                { KeyCode.Minus,'-'},
                { KeyCode.Equals,'=' },
                { KeyCode.LeftBracket,'['},
                { KeyCode.RightBracket,']'},
                { KeyCode.Backslash,'\\'},
                { KeyCode.Semicolon,';'},
                { KeyCode.Quote,'\'' },
                { KeyCode.Comma,',' },
                { KeyCode.Period,'.'},
                { KeyCode.Slash,'/' }
            };
            shiftOnCharDict = new Dictionary<KeyCode, char>()
            {
                { KeyCode.BackQuote,'~'},
                { KeyCode.Minus,'_'},
                { KeyCode.Equals,'+' },
                { KeyCode.Alpha0,')'},
                { KeyCode.Alpha1,'!'},
                { KeyCode.Alpha2,'@'},
                { KeyCode.Alpha3,'#'},
                { KeyCode.Alpha4,'$'},
                { KeyCode.Alpha5,'%'},
                { KeyCode.Alpha6,'^'},
                { KeyCode.Alpha7,'&'},
                { KeyCode.Alpha8,'*'},
                { KeyCode.Alpha9,'('},
                { KeyCode.LeftBracket,'{' },
                { KeyCode.RightBracket,'}' },
                { KeyCode.Backslash,'|' },
                { KeyCode.Comma,'<' },
                { KeyCode.Period,'>'},
                { KeyCode.Slash,'?' },
                { KeyCode.Semicolon,':' },
                { KeyCode.Quote,'"' }
            };
            keyCodeNameDict = new Dictionary<KeyCode, string>()
            {
                { KeyCode.Equals,"="},
                { KeyCode.Space,""},
                { KeyCode.Backspace,"Backspace" },
                { KeyCode.CapsLock,"Caps Lock" },
                { KeyCode.LeftShift,"Shift" },
                { KeyCode.RightShift,"Shift" },
                { KeyCode.LeftAlt,"Alt" },
                { KeyCode.RightAlt,"Alt" },
                { KeyCode.At,"@" },
                { KeyCode.Exclaim,"!"},
                { KeyCode.Hash,"#" },
                { KeyCode.Dollar,"$" },
                { KeyCode.Caret,"^"},
                { KeyCode.Ampersand,"&"},
                { KeyCode.Asterisk,"*"},
                { KeyCode.LeftParen,"("},
                { KeyCode.RightParen,")"},
                { KeyCode.Underscore,"_"},
                { KeyCode.KeypadPlus,"+"},
                { KeyCode.Colon,":"},
                { KeyCode.DoubleQuote,"\"" },
                { KeyCode.Escape,"Esc"},
                { KeyCode.LeftControl,"Ctrl"},
                { KeyCode.RightControl,"Ctrl" },
                { KeyCode.KeypadEquals,"="},
                { KeyCode.KeypadPeriod,"."},
                { KeyCode.KeypadDivide,"/"},
            };
        }

        protected override void Init()
        {
            if (!m_Init)
            {
                pcKeyboard = GetComponentInParent<UguiPcKeyboard>();
                if(pcKeyboard == null)
                    throw new Exception("PC keyboard is null.");
            }

            base.Init();
        }

        internal override void UpdateState()
        {
            if (pcKeyboard.IsShiftPress)
            {

            }
            else
            {

            }

            if (nameText == null)
                nameText = GetComponentInChildren<Text>();
            if (nameText != null)
            {
                var shiftNameStr = "";
                var nameStr = "";
                if (keyCodeNameDict.ContainsKey(m_KeyCode))
                {
                    nameStr = keyCodeNameDict[m_KeyCode];
                }
                else
                {
                    nameStr = m_KeyCode.ToString();
                }
                if (IsLetter(m_KeyCode))
                {
                    nameStr = pcKeyboard.IsUpper ? nameStr.ToUpper() : nameStr.ToLower();
                }
                nameText.text = shiftNameStr + nameStr;
            }
        }
    }
}