

using System.Collections.Generic;
using UnityEngine;


public class GameObjectSpawner : MonoBehaviour
{
    [SerializeField] protected List<GameObject> _prefabs;
    [SerializeField] protected GameObject prefab;
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

    [SerializeField] protected List<GameObject> objectPool = new List<GameObject>();
    protected Dictionary<int, List<GameObject> > objectPoolDictionary = new Dictionary<int, List<GameObject>>();

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
            GameObject obj = Instantiate(prefab, transform);
            obj.gameObject.SetActive(false);
            objectPool.Add(obj);
        }
    }
    protected virtual GameObject GetPooledObject()
    {
        foreach (GameObject obj in objectPool)
        {
            if (!obj.gameObject.activeInHierarchy && obj != null)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }

        GameObject objTmp = Instantiate(prefab, transform);
        objectPool.Add(objTmp);

        return objTmp;
    }

    protected virtual GameObject GetPooledObject(int index)
    {
        if(!objectPoolDictionary.ContainsKey(index)) {
            objectPoolDictionary[index] = new List<GameObject>();
        }
        foreach (GameObject obj in objectPoolDictionary[index])
            {
                if (!obj.gameObject.activeInHierarchy && obj != null)
                {
                    obj.gameObject.SetActive(true);
                    return obj;
                }
            }

        GameObject objTmp = Instantiate(_prefabs[index],transform);
        objTmp.gameObject.SetActive(true);
        objectPoolDictionary[index].Add(objTmp);

        return objTmp;
    }

    protected virtual void Spawn()
    {
        GameObject obj = GetPooledObject();
        if (obj != null)
        {
            obj.transform.position = spawnPoint.position;
        }
    }


}
