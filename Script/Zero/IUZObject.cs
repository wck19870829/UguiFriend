using UnityEngine;
using System.Collections;

/// <summary>
/// Ugui物体接口
/// </summary>
public interface IUZObject
{
    object Data { get; set; }
}

public interface IUZObject<TData>: IUZObject
    where TData: IUZObjectData
{
    new TData Data { get; set; }
}

/// <summary>
/// 数据接口
/// </summary>
public interface IUZObjectData
{
    object ID { get;}
}

public interface IUZObjectData<T>: IUZObjectData
{
    new T ID { get; }
}
