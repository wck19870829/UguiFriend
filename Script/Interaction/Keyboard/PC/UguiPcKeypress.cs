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
        protected static Dictionary<KeyCode, string> shiftOnCharDict;

        protected UguiPcKeyboard pcKeyboard;

        static UguiPcKeypress()
        {
            shiftOnCharDict = new Dictionary<KeyCode, string>()
            {
                { KeyCode.BackQuote,"~"},
                { KeyCode.Minus,"_"},
                { KeyCode.Equals,"+" },
                { KeyCode.Alpha0,")"},
                { KeyCode.Alpha1,"!"},
                { KeyCode.Alpha2,"@"},
                { KeyCode.Alpha3,"#"},
                { KeyCode.Alpha4,"$"},
                { KeyCode.Alpha5,"%"},
                { KeyCode.Alpha6,"^"},
                { KeyCode.Alpha7,"&"},
                { KeyCode.Alpha8,"*"},
                { KeyCode.Alpha9,"("},
                { KeyCode.LeftBracket,"{" },
                { KeyCode.RightBracket,"}" },
                { KeyCode.Backslash,"|" },
                { KeyCode.Comma,"<" },
                { KeyCode.Period,">"},
                { KeyCode.Slash,"?" },
                { KeyCode.Semicolon,":" },
                { KeyCode.Quote,"\"" }
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
                    if (numLockDict.ContainsKey(m_RawKeyCode))
                    {
                        var topStr = GetKeyCodeDisplayName(m_RawKeyCode)+"\r\n";
                        var bottomStr = GetKeyCodeDisplayName(numLockDict[m_RawKeyCode]);
                        nameText.text = topStr + bottomStr;
                    }
                    else
                    {
                        var shiftNameStr = "";
                        if (shiftOnCharDict.ContainsKey(m_RawKeyCode))
                        {
                            shiftNameStr = shiftOnCharDict[m_RawKeyCode] + "\r\n";
                        }
                        var nameStr = GetKeyCodeDisplayName(m_RawKeyCode);
                        if (IsLetter(m_RawKeyCode))
                        {
                            nameStr = pcKeyboard.IsUpper ? nameStr.ToUpper() : nameStr.ToLower();
                        }
                        nameText.text = shiftNameStr + nameStr;
                    }
                }
            }
            m_CurrentKeyCode = m_RawKeyCode;
            if (!pcKeyboard.IsNumLock)
            {
                if (numLockDict.ContainsKey(m_RawKeyCode))
                {
                    m_CurrentKeyCode = numLockDict[m_RawKeyCode];
                }
            }
        }

        protected override string GetInputCharacter()
        {
            var str = string.Empty;
            var keyCode = m_RawKeyCode;

            if (numLockDict.ContainsKey(keyCode) && !pcKeyboard.IsNumLock)
            {
                keyCode = m_CurrentKeyCode;
            }

            if (inputCharacterDict.ContainsKey(keyCode))
            {
                str = inputCharacterDict[keyCode];
            }

            if (shiftOnCharDict.ContainsKey(keyCode))
            {
                if (pcKeyboard.IsShiftPress)
                {
                    str = shiftOnCharDict[keyCode];
                }
            }

            if (IsLetter(keyCode))
            {
                str = pcKeyboard.IsUpper ? str.ToUpper() : str.ToLower();
            }

            return str;
        }
    }
}