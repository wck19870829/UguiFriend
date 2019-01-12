using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 课程表单节课程详情
    /// </summary>
    public class UguiClassScheduleItem : UguiObject
    {
        [SerializeField] protected Text nameText;
        [SerializeField] protected Text descText;
        [SerializeField] protected Text classroomText;
        [SerializeField] protected Graphic bg;

        protected override void RefreshView()
        {
            var data = Data as UguiClassScheduleItemData;

            if (nameText != null) nameText.text = data.className;
            if (descText != null) descText.text = data.classDesc;
            if (classroomText != null) classroomText.text = data.classroom;
            if (bg != null) bg.color = data.styleColor;
        }
    }

    public class UguiClassScheduleItemData : UguiObjectData
    {
        public string className;            //课程名称
        public string classDesc;            //课程描述
        public string classroom;            //教室名称
        public Color styleColor;            //颜色
        public DateTime start;              //起止时间
        public DateTime end;

        public UguiClassScheduleItemData(string className,string classDesc,string classroom,Color styleColor, DateTime start,DateTime end)
        {
            this.className = className;
            this.classDesc = classDesc;
            this.classroom = classroom;
            this.styleColor = styleColor;
            this.start = start;
            this.end = end;
        }
    }
}