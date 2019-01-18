using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace RedScarf.UguiFriend
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class UguiTools
    {
        public static List<UguiObject> objTempList;
        public static Dictionary<string, UguiObject> objTempDict;
        public static Dictionary<string,UguiObjectData> objDataTempSet;
        public static List<UguiObjectData> objDataTempList;

        static UguiTools()
        {
            objTempList = new List<UguiObject>();
            objTempDict = new Dictionary<string, UguiObject>();
            objDataTempSet = new Dictionary<string, UguiObjectData>();
            objDataTempList = new List<UguiObjectData>();
        }

        /// <summary>
        /// 是否为有效数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidNumber(Vector2 value)
        {
            var isNaN = float.IsNaN(value.x) || 
                        float.IsNaN(value.y);

            return !isNaN;
        }

        /// <summary>
        /// 是否为有效数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidNumber(Vector3 value)
        {
            var isNaN = float.IsNaN(value.x) || 
                        float.IsNaN(value.y) || 
                        float.IsNaN(value.z);

            return !isNaN;
        }

        /// <summary>
        /// 是否为有效数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsValidNumber(Vector4 value)
        {
            var isNaN = float.IsNaN(value.x) ||
                        float.IsNaN(value.y) ||
                        float.IsNaN(value.z)||
                        float.IsNaN(value.w);

            return !isNaN;
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="children"></param>
        /// <param name="content"></param>
        /// <param name="dataList"></param>
        /// <param name="prefabSource"></param>
        public static void SetChildrenDatas(List<UguiObject>children,Transform content,List<UguiObjectData>dataList,UguiObject prefabSource)
        {
            //继续使用的对象复用,未使用中的对象放入池中,最后补齐新增加的对象
            objDataTempSet.Clear();
            objDataTempList.Clear();
            objTempList.Clear();
            foreach (var data in dataList)
            {
                objDataTempSet.Add(data.guid, data);
            }
            foreach (var child in children)
            {
                if (!objDataTempSet.ContainsKey(child.Guid))
                {
                    objTempList.Add(child);
                }
                else
                {
                    objDataTempList.Add(objDataTempSet[child.Guid]);
                }
            }
            foreach (var removeChild in objTempList)
            {
                children.Remove(removeChild);
            }
            UguiObjectPool.Instance.Push(objTempList);
            DestroyChildren(content.gameObject);

            if (prefabSource == null)
            {
                UguiObjectPool.Instance.Get(objTempList, objDataTempList, content);
            }
            else
            {
                UguiObjectPool.Instance.Get(objTempList, objDataTempList, prefabSource, content);
            }
            children.AddRange(objTempList);
            objTempList.Clear();
            objDataTempList.Clear();
            objTempDict.Clear();
            foreach (var child in children)
            {
                objTempDict.Add(child.Guid, child);
            }
            children.Clear();
            foreach (var data in dataList)
            {
                children.Add(objTempDict[data.guid]);
            }
            objTempDict.Clear();
        }

        /// <summary>
        /// 销毁所有子元素
        /// </summary>
        /// <param name="go"></param>
        public static void DestroyChildren(GameObject go)
        {
            for (var i = go.transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(go.transform.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 是否在屏幕视图坐标中
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <param name="canvas"></param>
        /// <param name="viewRect"></param>
        /// <returns></returns>
        public static bool InScreenViewRect(Vector3 worldPoint, Canvas canvas, Rect viewRect)
        {
            if (canvas == null)
                throw new Exception("Canvas is null.");

            Camera cam = null;
            if (canvas.rootCanvas.worldCamera != null)
            {
                if (canvas.rootCanvas.renderMode == RenderMode.ScreenSpaceCamera || 
                    canvas.rootCanvas.renderMode == RenderMode.WorldSpace)
                {
                    cam = canvas.rootCanvas.worldCamera;
                }
            }

            var screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPoint);
            var viewportPoint = UguiMathf.ScreenPoint2ViewportPoint(screenPoint);

            return viewRect.Contains(viewportPoint);
        }

        /// <summary>
        /// 获取全局坐标系ui元素尺寸
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Rect? GetGlobalGraphicRectIncludeChildren(RectTransform content)
        {
            if (content == null)
                throw new Exception("Content is null.");

            var children = content.GetComponentsInChildren<Graphic>();
            if (children.Length == 0) return null;

            Rect? globalRect=null;
            var cornersArr = new Vector3[4];
            foreach (var graphic in children)
            {
                graphic.rectTransform.GetWorldCorners(cornersArr);
                var childRect = GetRectContainsPoints(cornersArr);

                var mask = graphic.GetComponentInParent<Mask>();
                if (mask != null)
                {
                    mask.rectTransform.GetWorldCorners(cornersArr);
                    var maskRect= GetRectContainsPoints(cornersArr);
                    var overlap = UguiMathf.RectOverlap(childRect, maskRect);
                    if (overlap!=null)
                    {
                        childRect = (Rect)overlap;
                    }
                    else continue;
                }

                if (globalRect == null) globalRect = childRect;
                globalRect = UguiMathf.RectCombine(childRect, (Rect)globalRect);
            }

            return globalRect;
        }

        /// <summary>
        /// 获取物体的世界坐标系边界(递归包含所有子物体)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="includeContent">是否包含容器自身</param>
        /// <returns></returns>
        public static Rect? GetGlobalRectIncludeChildren(RectTransform content,bool includeContent)
        {
            if (content == null)
                throw new Exception("Content is null.");

            var children = content.GetComponentsInChildren<RectTransform>();
            if (children.Length == 1 && !includeContent) return null;

            Rect? contentRect = null;
            var cornersArr = new Vector3[4];
            foreach (var child in children)
            {
                if (!includeContent)
                {
                    if (child == content)
                    {
                        continue;
                    }
                }

                if (contentRect == null)
                {
                    child.GetWorldCorners(cornersArr);
                    contentRect= GetRectContainsPoints(cornersArr);
                }
                child.GetWorldCorners(cornersArr);
                var childRect = GetRectContainsPoints(cornersArr);
                contentRect = UguiMathf.RectCombine((Rect)contentRect,childRect);
            }

            return contentRect;
        }

        /// <summary>
        /// 获取相对于其他容器的局部坐标系边界(包含所有子物体)
        /// </summary>
        /// <param name="content">容器</param>
        /// <param name="relative">相对于</param>
        /// <param name="includeContent">true:包含content。false:不包含content</param>
        /// <returns></returns>
        public static Rect? GetLocalRectIncludeChildren(RectTransform content,RectTransform relative,bool includeContent)
        {
            if (relative == null)
                throw new Exception("Relative is null.");

            var contentRect = GetGlobalRectIncludeChildren(content, includeContent);
            if (contentRect == null) return null;

            var rect = (Rect)contentRect;
            var corners = new Vector3[]
            {
                new Vector3(rect.xMin,rect.yMin),
                new Vector3(rect.xMax,rect.yMin),
                new Vector3(rect.xMax,rect.yMax),
                new Vector3(rect.xMin,rect.yMax)
            };
            for (var i = 0; i < corners.Length; i++)
            {
                corners[i] = relative.worldToLocalMatrix.MultiplyPoint(corners[i]);
            }
            rect = GetRectContainsPoints(corners);

            return rect;
        }

        /// <summary>
        /// 获取包含所有点的矩形
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static Rect GetRectContainsPoints(Vector3[]points)
        {
            if (points == null)
                throw new Exception("Points is null.");
            if (points.Length == 0)
                throw new Exception("Points length is zero.");

            var rect = new Rect(points[0], Vector3.zero);
            foreach (var point in points)
            {
                rect.xMax = Mathf.Max(rect.xMax,point.x);
                rect.xMin = Mathf.Min(rect.xMin, point.x);
                rect.yMax = Mathf.Max(rect.yMax,point.y);
                rect.yMin = Math.Min(rect.yMin, point.y);
            }

            return rect;
        }

        /// <summary>
        /// 点从局部坐标转换为世界坐标
        /// </summary>
        /// <param name="localPoints"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Vector3[] LocalPoints2GlobalPoints(Vector3[] localPoints, Transform content)
        {
            if (content == null)
                throw new Exception("Content is null.");
            if (localPoints == null)
                throw new Exception("Local points is null.");

            var globalPoints = new Vector3[localPoints.Length];
            for (var i = 0; i < localPoints.Length; i++)
            {
                globalPoints[i] = content.TransformPoint(localPoints[i]);
            }

            return globalPoints;
        }

        /// <summary>
        /// 点从世界坐标转换为局部坐标
        /// </summary>
        /// <param name="globalPoints"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Vector3[] GlobalPoints2LocalPoints(Vector3[] globalPoints, Transform content)
        {
            if(content==null)
                throw new Exception("Content is null.");
            if (globalPoints == null)
                throw new Exception("Global points is null.");

            var localPoints = new Vector3[globalPoints.Length];
            for (var i=0;i<localPoints.Length;i++)
            {
                localPoints[i] = content.InverseTransformPoint(globalPoints[i]);
            }

            return localPoints;
        }

        /// <summary>
        /// 设置锚点
        /// </summary>
        /// <param name="target"></param>
        /// <param name="allign"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        public static void SetAnchor(RectTransform target, AnchorPresets anchorPresets, int offsetX = 0, int offsetY = 0)
        {
            target.anchoredPosition = new Vector3(offsetX, offsetY, 0);

            switch (anchorPresets)
            {
                case AnchorPresets.TopLeft:
                    target.anchorMin = new Vector2(0, 1);
                    target.anchorMax = new Vector2(0, 1);
                    break;

                case AnchorPresets.TopCenter:
                    target.anchorMin = new Vector2(0.5f, 1);
                    target.anchorMax = new Vector2(0.5f, 1);
                    break;

                case AnchorPresets.TopRight:
                    target.anchorMin = new Vector2(1, 1);
                    target.anchorMax = new Vector2(1, 1);
                    break;

                case AnchorPresets.MiddleLeft:
                    target.anchorMin = new Vector2(0, 0.5f);
                    target.anchorMax = new Vector2(0, 0.5f);
                    break;

                case AnchorPresets.MiddleCenter:
                    target.anchorMin = new Vector2(0.5f, 0.5f);
                    target.anchorMax = new Vector2(0.5f, 0.5f);
                    break;

                case AnchorPresets.MiddleRight:
                    target.anchorMin = new Vector2(1, 0.5f);
                    target.anchorMax = new Vector2(1, 0.5f);
                    break;

                case AnchorPresets.BottomLeft:
                    target.anchorMin = new Vector2(0, 0);
                    target.anchorMax = new Vector2(0, 0);
                    break;

                case AnchorPresets.BottonCenter:
                    target.anchorMin = new Vector2(0.5f, 0);
                    target.anchorMax = new Vector2(0.5f, 0);
                    break;

                case AnchorPresets.BottomRight:
                    target.anchorMin = new Vector2(1, 0);
                    target.anchorMax = new Vector2(1, 0);
                    break;

                case AnchorPresets.HorStretchTop:
                    target.anchorMin = new Vector2(0, 1);
                    target.anchorMax = new Vector2(1, 1);
                    break;

                case AnchorPresets.HorStretchMiddle:
                    target.anchorMin = new Vector2(0, 0.5f);
                    target.anchorMax = new Vector2(1, 0.5f);
                    break;

                case AnchorPresets.HorStretchBottom:
                    target.anchorMin = new Vector2(0, 0);
                    target.anchorMax = new Vector2(1, 0);
                    break;

                case AnchorPresets.VertStretchLeft:
                    target.anchorMin = new Vector2(0, 0);
                    target.anchorMax = new Vector2(0, 1);
                    break;

                case AnchorPresets.VertStretchCenter:
                    target.anchorMin = new Vector2(0.5f, 0);
                    target.anchorMax = new Vector2(0.5f, 1);
                    break;

                case AnchorPresets.VertStretchRight:
                    target.anchorMin = new Vector2(1, 0);
                    target.anchorMax = new Vector2(1, 1);
                    break;

                case AnchorPresets.StretchAll:
                    target.anchorMin = new Vector2(0, 0);
                    target.anchorMax = new Vector2(1, 1);
                    break;
            }
        }

        /// <summary>
        /// 设置轴心点
        /// </summary>
        /// <param name="target"></param>
        /// <param name="preset"></param>
        public static void SetPivot(RectTransform target, PivotPresets pivotPresets)
        {
            switch (pivotPresets)
            {
                case (PivotPresets.TopLeft): target.pivot = new Vector2(0, 1); break;
                case (PivotPresets.TopCenter): target.pivot = new Vector2(0.5f, 1); break;
                case (PivotPresets.TopRight): target.pivot = new Vector2(1, 1); break;
                case (PivotPresets.MiddleLeft): target.pivot = new Vector2(0, 0.5f); break;
                case (PivotPresets.MiddleCenter): target.pivot = new Vector2(0.5f, 0.5f); break;
                case (PivotPresets.MiddleRight): target.pivot = new Vector2(1, 0.5f); break;
                case (PivotPresets.BottomLeft): target.pivot = new Vector2(0, 0); break;
                case (PivotPresets.BottomCenter): target.pivot = new Vector2(0.5f, 0); break;
                case (PivotPresets.BottomRight): target.pivot = new Vector2(1, 0); break;
            }
        }

        /// <summary>
        /// 添加一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T AddChild<T>(string name,Transform parent=null)where T:Component
        {
            var go = new GameObject(name);
            if (parent != null)
                go.transform.SetParent(parent);

            go.transform.localPosition = Vector3.zero;
            go.transform.rotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            var component = go.GetComponent<T>();
            if(component==null)
                component= go.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// 复制一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T AddChild<T>(T source, Transform parent = null) where T : Component
        {
            if (source == null)
                throw new Exception("Source is null.");

            var clone = GameObject.Instantiate<T>(source);
            if (parent != null)
                clone.transform.SetParent(parent);
            clone.transform.localScale = Vector3.one;
            clone.transform.localRotation = Quaternion.identity;
            clone.transform.localPosition = Vector3.zero;

            return clone;
        }

        /// <summary>
        /// 查询一个不使用的层
        /// </summary>
        /// <returns></returns>
        public static int FindUnusedLayer()
        {
            for (var i=0;i<32;i++)
            {
                if (string.IsNullOrEmpty(LayerMask.LayerToName(i)))
                {
                    return i;
                }
            }

            return -1;
        }

        public static void CopyProps(object source, object destination,bool onlyCopyValueType=true)
        {
            if (source == null || destination == null)
                return;

            var flag = BindingFlags.Public | BindingFlags.Instance;
            var props = source.GetType().GetProperties(flag);
            foreach (var prop in props)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    if (onlyCopyValueType)
                    {
                        if (!typeof(ValueType).IsAssignableFrom(prop.PropertyType))
                        {
                            continue;
                        }
                    }

                    try
                    {
                        var value = prop.GetValue(source, null);
                        prop.SetValue(destination, value, null);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
            }
        }

        #region 其他

        /// <summary>
        /// 查找子类
        /// </summary>
        /// <param name="baseType">父类</param>
        /// <returns></returns>
        public static List<Type> FindSubClass(Type baseType)
        {
            var subTypeList = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assemblie in assemblies)
            {
                var types = assemblie.GetTypes();
                foreach (var type in types)
                {
                    if (baseType.IsAssignableFrom(type))
                    {
                        subTypeList.Add(type);
                    }
                }
            }

            return subTypeList;
        }

        /// <summary>
        /// byte[]转string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string Bytes2String(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return string.Empty;

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// string转byte[]
        /// </summary>
        /// <param name="str"></param>
        public static byte[] String2Bytes(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 二进制序列化对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeObject(object obj)
        {
            if (obj == null)
                return null;

            using (var ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                byte[] bytes = ms.GetBuffer();
                ms.Close();

                return bytes;
            }
        }

        /// <summary>
        /// 二进制反序列化对象
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object DeserializeObject(byte[] bytes)
        {
            if (bytes == null||bytes.Length==0)
                return null;

            using (var ms = new MemoryStream())
            {
                ms.Position = 0;
                var formatter = new BinaryFormatter();
                var obj = formatter.Deserialize(ms);
                ms.Close();

                return obj;
            }
        }

        #endregion
    }

    /// <summary>
    /// 锚点预设，对应编辑面板中的预设
    /// </summary>
    public enum AnchorPresets
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottonCenter,
        BottomRight,
        BottomStretch,
        VertStretchLeft,
        VertStretchRight,
        VertStretchCenter,
        HorStretchTop,
        HorStretchMiddle,
        HorStretchBottom,
        StretchAll
    }

    /// <summary>
    /// 轴心点预设
    /// </summary>
    public enum PivotPresets
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }
}