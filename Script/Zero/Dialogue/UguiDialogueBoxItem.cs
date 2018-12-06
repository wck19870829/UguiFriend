using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    [UguiBinding(typeof(UguiDialogueBoxItemData))]
    /// <summary>
    /// 对话窗单个条目
    /// </summary>
    public class UguiDialogueBoxItem : UguiObject
    {
        protected override void RefreshView()
        {

        }
    }

    public class UguiDialogueBoxItemData : UguiObjectData
    {

    }
}