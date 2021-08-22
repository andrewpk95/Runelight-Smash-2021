using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : Singleton<ObjectPool<T>> where T : MonoBehaviour
{
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

        ResetObject(obj);
        return obj;
    }

    protected virtual void ResetObject(T obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.transform);
    }

    public T GetObject()
    {
        T obj = objectPool.Count > 0 ? objectPool.Dequeue() : CreateObject();

        obj.gameObject.SetActive(true);
        obj.transform.SetParent(null);

        return obj;
    }

    public void ReturnObject(T obj)
    {
        ResetObject(obj);
        objectPool.Enqueue(obj);
    }
}