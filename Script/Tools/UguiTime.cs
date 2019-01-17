using UnityEngine;
using System.Collections;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// Ugui内部时间
    /// </summary>
    public class UguiTime : UguiSingleton<UguiTime>, IUguiSingletonCreate<UguiTime>
    {
        void Update()
        {
            
        }

        public void OnSingletonCreate(UguiTime instance)
        {

        }
    }
}