using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace RedScarf.UguiFriend.Demo
{
    public class PanelA : UIBehaviour
    {
        public UguiGridLayoutGroup m_Grid;
        public int itemCount = 100; 

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                var dataList = new List<UguiObjectData>();
                for (var i=0;i< itemCount; i++)
                {
                    var data = new ItemAData();
                    dataList.Add(data);
                }
                m_Grid.Set(dataList);
            }
        }
    }
}