using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// Object屏幕越界回收器
    /// </summary>
    public class UguiAreaRecovererObject : UguiAreaRecoverer<UguiObject>
    {
        protected override void ProcessItemAfterRecycle(UguiObject child)
        {
            UguiObjectPool.Instance.Push(child);
        }
    }
}