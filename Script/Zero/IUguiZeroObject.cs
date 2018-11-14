using UnityEngine;
using System.Collections;

/// <summary>
/// Ugui物体接口
/// </summary>
public interface IUguiZeroObject
{
    object Data { get; set; }
}

public interface IUguiZeroObject<TData>: IUguiZeroObject
{
    new TData Data { get; set; }
}

/// <summary>
/// 数据接口
/// </summary>
public interface IUguiZeroObjectData
{
    object ID { get;}
}

public interface IUguiZeroObjectData<T>:IUguiZeroObjectData
{
    new object ID { get; }
}
