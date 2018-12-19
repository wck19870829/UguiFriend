using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend.Demo
{
    [UguiBinding(typeof(ItemBData))]
    public class ItemB : UguiObject
    {
        protected override void RefreshView()
        {

        }
    }

    public class ItemBData : UguiObjectData
    {

    }
}