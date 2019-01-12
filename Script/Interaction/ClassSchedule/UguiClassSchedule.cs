using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 课程表
    /// </summary>
    public class UguiClassSchedule : UguiLayoutGroup
    {
        static readonly List<TimetableType> defaultTimetableTypeList;

        [SerializeField] protected UguiGridLayoutGroup m_DateGrid;                          //顶部日期容器
        [SerializeField] protected UguiGridLayoutGroup m_TimetableTypeGrid;                 //左侧时间区间
        [SerializeField] protected List<TimetableType> m_TimetableTypeList;

        protected DateTime dateFrom;
        protected DateTime dateTo;

        static UguiClassSchedule()
        {
            defaultTimetableTypeList = new List<TimetableType>()
            {
                new TimetableType(8,0),
                new TimetableType(9,0),
                new TimetableType(10,0),
                new TimetableType(11,0),
                new TimetableType(12,0),
                new TimetableType(14,0),
                new TimetableType(15,0),
                new TimetableType(16,0),
                new TimetableType(17,0),
                new TimetableType(18,0)
            };
        }

        protected UguiClassSchedule()
        {

        }

        /// <summary>
        /// 由时间获取位置
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        protected Vector2 GetPosByTime(DateTime dateTime)
        {
            var pos = Vector2.zero;

            return pos;
        }

        public override void UpdateChildrenLocalPosition()
        {
            //创建时间线

            //创建左侧时间区间
            if (m_TimetableTypeList.Count == 0)
            {
                m_TimetableTypeList.AddRange(defaultTimetableTypeList);
            }
            m_TimetableTypeList.Sort((a, b) =>
            {
                var scoreA = a.hour * 60 + a.minute;
                var scoreB = b.hour * 60 + b.minute;

                return scoreA - scoreB;
            });

            if (m_ChildDataList.Count == 0)
            {

            }
            else
            {
                dateFrom = DateTime.MaxValue;
                dateTo = DateTime.MinValue;
                //foreach (var detail in data.detailList)
                //{
                //    if (detail.start.Date != detail.end.Date)
                //    {
                //        Debug.LogErrorFormat("课时不支持跨天! start:{0} end:{1}", detail.start, detail.end);
                //        continue;
                //    }

                //    dateFrom = UguiMathf.Min(detail.start, detail.end, dateFrom);
                //    dateTo = UguiMathf.Max(detail.start, detail.end, dateTo);
                //}
            }
        }

        [Serializable]
        public struct TimetableType
        {
            public int hour;
            public int minute;

            public TimetableType(int hour,int minute)
            {
                this.hour = hour;
                this.minute = minute;
            }
        }
    }

    public class UguiClassScheduleData : UguiObjectData
    {
        public List<UguiClassScheduleItemData> detailList;

        public UguiClassScheduleData()
        {
            detailList = new List<UguiClassScheduleItemData>();
        }

        public UguiClassScheduleData(List<UguiClassScheduleItemData> detailList)
        {
            this.detailList = detailList;
        }
    }
}