using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend.Demo
{
    [UguiBinding(typeof(ItemAData))]
    public class ItemA : UguiObject
    {
        protected override void RefreshView()
        {

        }

        private void OnDrawGizmos()
        {
            var corners =new Vector3[4];
            (transform as RectTransform).GetLocalCorners(corners);
            foreach (var point in corners)
            {
                Gizmos.DrawSphere(point,10);
            }
        }
    }

    public class ItemAData : UguiObjectData
    {

    }
}

