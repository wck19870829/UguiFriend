using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    [UguiBinding(typeof(UguiDialogueBoxData))]
    /// <summary>
    /// 对话窗
    /// </summary>
    public class UguiDialogueBox : UguiObject
    {
        protected override void RefreshView()
        {

        }
    }

    public class UguiDialogueBoxData : UguiObjectData
    {

    }
}