using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedScarf.UguiFriend.Demo
{
    [UguiBinding(typeof(PanelAData))]
    public class PanelA : UguiObject
    {
        [SerializeField] private UguiGridLayoutGroup m_Grid;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var data = new PanelAData();
                data.dataList = new List<UguiObjectData>();
                for (var i=0;i<1000;i++)
                {
                    var itemA = new ItemAData();
                    data.dataList.Add(itemA);
                }
                Data = data;
            }
        }

        protected override void RefreshView()
        {
            var data = Data as PanelAData;
            if (data != null)
            {
                m_Grid.Set(data.dataList);
            }
        }
    }

    public class PanelAData : UguiObjectData
    {
        public List<UguiObjectData> dataList;
    }
}