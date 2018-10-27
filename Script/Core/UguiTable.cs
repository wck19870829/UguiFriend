using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    [ExecuteInEditMode]
    /// <summary>
    /// 可变长宽表格
    /// </summary>
    public class UguiTable : UIBehaviour
    {
        [SerializeField]protected Axis m_StartAxis;
        [SerializeField]protected Vector2 m_Spacing;

        [Range(1,100)]
        [SerializeField]protected int m_ConstraintCount;
        protected bool m_Reposition;

        protected override void Start()
        {
            base.Start();

            m_Reposition = true;
        }

        protected virtual void Update()
        {
            if (m_Reposition)
            {

            }
            Reposition();
        }

        public virtual void Reposition()
        {
            var offset = Vector2.zero;
            for (var i=0;i<transform.childCount;i++)
            {
                var child = transform.GetChild(i) as RectTransform;
                //var rect = UguiTools.GetRectIncludeChildren(child);

                //child.localPosition = offset;

                //offset += rect.size + m_Spacing;
            }

            m_Reposition = false;
        }

        public enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }
    }
}