

using System.Collections.Generic;
using UnityEngine;


public class Spawner<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected List<T> prefabs;
    public List<T> Prefabs => prefabs;
    [SerializeField] protected T prefab;
    public T Prefab
    {
        get { return prefab; }
        set
        {
            prefab = value;
        }
    }
    [SerializeField] protected Transform spawnPoint;
    public Transform SpawnPoint
    {
        get { return spawnPoint; }
        set { spawnPoint = value; }
    }
    [SerializeField] protected float spawnInterval = 0.1f;
    public float SpawnInterval
    {
        get { return spawnInterval; }
        set { spawnInterval = value; }
    }

    [SerializeField] protected List<T> objectPool = new List<T>();
    protected Dictionary<int, List<T> > objectPoolDictionary = new Dictionary<int, List<T>>();

    public void InitObjectPool(int length)
    {
        for (int i = 0; i < objectPool.Count; i++)
        {
            if (objectPool[i] != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(objectPool[i]);
                }
                else
                {
                    DestroyImmediate(objectPool[i].gameObject);
                }
            }
        }
        objectPool.Clear();
        for (int i = 0; i < length; i++)
        {
            T obj = Instantiate(prefab, transform);
            obj.gameObject.SetActive(false);
            objectPool.Add(obj);
        }
    }
    protected virtual T GetPooledObject()
    {
        foreach (T obj in objectPool)
        {
            if (!obj.gameObject.activeInHierarchy && obj != null)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        T objTmp = Instantiate(prefab, transform);
        objectPool.Add(objTmp);

        return objTmp;
    }

    protected virtual T GetPooledObject(int index)
    {
        if(!objectPoolDictionary.ContainsKey(index)) {
            objectPoolDictionary[index] = new List<T>();
        }
        foreach (T obj in objectPoolDictionary[index])
            {
                if (!obj.gameObject.activeInHierarchy && obj != null)
                {
                    obj.gameObject.SetActive(true);
                    return obj;
                }
            }

        T objTmp = Instantiate(prefabs[index],transform);
        objTmp.gameObject.SetActive(true);
        objectPoolDictionary[index].Add(objTmp);

        return objTmp;
    }

    protected virtual void Spawn()
    {
        T obj = GetPooledObject();
        if (obj != null)
        {
            obj.transform.position = spawnPoint.position;
        }
    }


}
