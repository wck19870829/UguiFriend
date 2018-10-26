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
        protected static Dictionary<KeyCode, char> shiftOnCharDict;

        protected UguiPcKeyboard pcKeyboard;

        static UguiPcKeypress()
        {
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

        public override void UpdateState()
        {
            if (m_AutoName)
            {
                if (nameText == null)
                    nameText = GetComponentInChildren<Text>();
                if (nameText != null)
                {
                    var shiftNameStr = "";
                    if (shiftOnCharDict.ContainsKey(m_KeyCode))
                    {
                        shiftNameStr = shiftOnCharDict[m_KeyCode]+"\r\n";
                    }
                    var nameStr = "";
                    if (displayNameDict.ContainsKey(m_KeyCode))
                    {
                        nameStr=displayNameDict[m_KeyCode];
                    }
                    else
                    {
                        nameStr= string.Intern(m_KeyCode.ToString());
                    }
                    if (IsLetter(m_KeyCode))
                    {
                        nameStr = pcKeyboard.IsUpper ? nameStr.ToUpper() : nameStr.ToLower();
                    }
                    nameText.text = shiftNameStr + nameStr;
                }
            }
        }

        protected override char GetInputCharacter(KeyCode keyCode)
        {
            char ch = emptyCharacter;
            if (pcKeyboard.IsShiftPress)
            {
                if (shiftOnCharDict.ContainsKey(keyCode))
                {
                    ch = shiftOnCharDict[keyCode];
                }
            }
            else
            {
                if (inputCharacterDict.ContainsKey(keyCode))
                {
                    ch = inputCharacterDict[keyCode];
                }
            }

            return ch;
        }
    }
}