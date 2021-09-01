using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : Singleton<ObjectPool<T>> where T : Poolable
{
    public static int id = 0;

    [SerializeField]
    protected GameObject poolingObject;

    [SerializeField]
    protected int initCount = 10;

    private Queue<T> objectPool = new Queue<T>();

    protected override void Awake()
    {
        base.Awake();

        InitializeObjectPool();
    }

    private void InitializeObjectPool()
    {
        for (int i = 0; i < initCount; i++)
        {
            T obj = CreateObject();

            objectPool.Enqueue(obj);
        }
    }

    private T CreateObject()
    {
        T obj = Instantiate(poolingObject).GetComponent<T>();

        obj.name = $"{id}";
        id++;
        ResetObject(obj);
        return obj;
    }

    protected virtual void ResetObject(T obj)
    {
        obj.SetEnabled(false);
        obj.Reset();
        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;
        obj.transform.localScale = Vector3.one;
    }

    public T GetObject()
    {
        T obj = objectPool.Count > 0 ? objectPool.Dequeue() : CreateObject();

        obj.SetEnabled(true);
        obj.transform.SetParent(null);

        return obj;
    }

    public void ReturnObject(T obj)
    {
        ResetObject(obj);
        objectPool.Enqueue(obj);
    }
}
