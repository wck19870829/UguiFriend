using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    /// <summary>
    /// 虚拟手指
    /// </summary>
    public class UguiKeyboardVirtualFinger : MonoBehaviour
    {
        protected Collider m_Coll;
        protected Rigidbody m_Rigidbody;

        protected virtual void Awake()
        {
            m_Coll = GetComponent<Collider>();
            m_Coll.isTrigger = true;
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Rigidbody.useGravity = false;
            m_Rigidbody.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var keypress=other.gameObject.GetComponent<UguiKeypress>();
            if (keypress!=null)
            {
                keypress.KeyDown();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var keypress = other.gameObject.GetComponent<UguiKeypress>();
            if (keypress != null)
            {
                keypress.KeyUp();
            }
        }
    }
}