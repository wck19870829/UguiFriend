using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 多点触控模拟
    /// </summary>
    public sealed class UguiMultPointSimulation : UguiSingleton<UguiMultPointSimulation>, 
        IUguiSingletonCreate<UguiMultPointSimulation>
    {
        UguiMultPointSimulationRaycaster raycaster;
        UguiMultPointSimulationInputModule inputModule;

        public void OnSingletonCreate(UguiMultPointSimulation instance)
        {
            raycaster = gameObject.AddComponent<UguiMultPointSimulationRaycaster>();
            if (EventSystem.current)
            {
                Destroy(EventSystem.current.gameObject);
            }
            inputModule = gameObject.AddComponent<UguiMultPointSimulationInputModule>();
            name = "[EventSystem_MultPointSimulation]";
        }
    }
}