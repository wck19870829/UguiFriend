using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// 预设路径窗口历史配置
/// </summary>
public class UguiPrefabPathWindowPreset : ScriptableObject
{
    public List<string> pathList;
    public string savePath;

    public UguiPrefabPathWindowPreset()
    {
        pathList = new List<string>();
    }
}