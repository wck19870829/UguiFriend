using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    public class UguiNavigationBarItem : UguiObject
    {
        protected override void RefreshView()
        {

        }
    }

    public class UguiNavigationBarItemData : UguiObjectData
    {
        public string title;
        public string message;
        public string icon;

        public UguiNavigationBarItemData(string title, string message, string icon = "")
        {
            this.title = title;
            this.message = message;
            this.icon = icon;
        }
    }
}