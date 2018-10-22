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
        [SerializeField]protected Corner m_StartCorner;
        [SerializeField]protected Axis m_StartAxis;
        [SerializeField]protected Vector2 m_Spacing;
        [SerializeField]protected Constraint m_Constraint;
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
            var childrenBounds = new Bounds();
            for (var i=0;i<transform.childCount;i++)
            {
                var child = transform.GetChild(i) as RectTransform;
                var corners = new Vector3[4];
                child.GetWorldCorners(corners);
                foreach (var corner in corners)
                {
                    Debug.Log(corner);
                }
                //var b = UguiTools.GetChildrenBounds(transform.GetChild(i));
                //Debug.Log(b);
                //childrenBounds.Encapsulate(b);
            }

            m_Reposition = false;
        }

        public enum Axis
        {
            Horizontal = 0,
            Vertical = 1
        }

        public enum Corner
        {
            UpperLeft = 0,
            UpperRight = 1,
            LowerLeft = 2,
            LowerRight = 3
        }

        public enum Constraint
        {
            Flexible = 0,
            FixedColumnCount = 1,
            FixedRowCount = 2
        }
    }
}