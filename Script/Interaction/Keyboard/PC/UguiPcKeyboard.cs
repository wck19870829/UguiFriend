using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// PC键盘
    /// </summary>
    public class UguiPcKeyboard : UguiKeyboard
    {
        protected bool m_IsShiftPress;
        protected bool m_IsCtrlPress;
        protected bool m_IsAltPress;
        protected bool m_IsCapsLock;
        protected bool m_IsNumLock;
        protected bool m_IsScrollLock;
        protected bool m_IsUpper;

        public override void ResetKeyboard()
        {
            m_IsCapsLock = false;
            m_IsShiftPress = false;
            m_IsAltPress = false;
            m_IsCtrlPress = false;
            m_IsUpper = false;

            base.ResetKeyboard();
        }

        /// <summary>
        /// Shift键是否按下
        /// </summary>
        public bool IsShiftPress { get { return m_IsShiftPress; } }

        /// <summary>
        /// Alt键是否按下
        /// </summary>
        public bool IsAltPress { get { return m_IsAltPress; } }

        /// <summary>
        /// Ctrl键是否按下
        /// </summary>
        public bool IsCtrlPress { get { return m_IsCtrlPress; } }

        /// <summary>
        /// 大小写是否锁定
        /// </summary>
        public bool IsCapsLock { get { return m_IsCapsLock; } }

        /// <summary>
        /// 数字键锁定
        /// </summary>
        public bool IsNumLock { get { return m_IsNumLock; } }

        /// <summary>
        /// 滚动锁定
        /// </summary>
        public bool IsScrollLock { get { return m_IsScrollLock; } }

        /// <summary>
        /// 是否为大写
        /// </summary>
        public bool IsUpper { get { return m_IsUpper; } }

        protected override void CheckStateChange(KeyCode keyCode, UguiKeypress.KeypressState keypressState)
        {
            m_IsShiftPress = false;
            SetKeepPressState(KeyCode.LeftShift, ref m_IsShiftPress);
            SetKeepPressState(KeyCode.RightShift, ref m_IsShiftPress);

            m_IsAltPress = false;
            SetKeepPressState(KeyCode.LeftAlt, ref m_IsAltPress);
            SetKeepPressState(KeyCode.RightAlt, ref m_IsAltPress);

            m_IsCtrlPress = false;
            SetKeepPressState(KeyCode.LeftControl, ref m_IsCtrlPress);
            SetKeepPressState(KeyCode.RightControl, ref m_IsCtrlPress);

            m_IsCapsLock = false;
            SetKeepPressState(KeyCode.CapsLock, ref m_IsCapsLock);

            m_IsScrollLock = false;
            SetKeepPressState(KeyCode.ScrollLock, ref m_IsCapsLock);

            m_IsNumLock = false;
            SetKeepPressState(KeyCode.Numlock, ref m_IsCapsLock);

            m_IsUpper = (m_IsCapsLock == m_IsShiftPress) ?
                        false :
                        (m_IsCapsLock || m_IsShiftPress);

            base.CheckStateChange(keyCode, keypressState);
        }

        protected void SetKeepPressState(KeyCode keyCode, ref bool state)
        {
            if (keyDict.ContainsKey(keyCode))
            {
                if (keyDict[keyCode].State == UguiKeypress.KeypressState.Press)
                {
                    state = true;
                }
            }
        }
    }
}