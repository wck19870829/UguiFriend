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
        private void OnTriggerEnter(Collider other)
        {
            Debug.LogFormat("Enter:{0}",other);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.LogFormat("Exit:{0}",other);
        }
    }
}