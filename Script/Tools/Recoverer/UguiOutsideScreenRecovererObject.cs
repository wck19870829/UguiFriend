using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// Object屏幕越界回收器
    /// </summary>
    public class UguiOutsideScreenRecovererObject : UguiOutsideScreenRecoverer<UguiObject>
    {
        protected override void ProcessItemBeforeRecycle(UguiObject child)
        {
            UguiObjectPool.Instance.Push(child);
        }
    }
}