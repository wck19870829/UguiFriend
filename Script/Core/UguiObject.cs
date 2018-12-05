using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    public class UguiObject : UIBehaviour, IUguiObject
    {
        protected object m_Data;

        public object Data
        {
            get
            {
                return m_Data;
            }
            set
            {
                m_Data = value;
            }
        }
    }
}