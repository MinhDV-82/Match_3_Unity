using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EffectType
{
    NormalMatch
};


public class EffectManager : GameObjectSpawner
{
    private float timeEndAni = 0.6f;

    private Dictionary<EffectType, List<GameObject>> _effects = new();
    private Dictionary<EffectType, float> _timeEffects = new();
    private static EffectManager instance;
    public static EffectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<EffectManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(EffectManager).Name);
                    instance = obj.AddComponent<EffectManager>();
                }
            }
            return instance;
        }
    }

    private ParticleSystem _practicleSystem;
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //! Init 
        InitTimeEffects();
    }

    private void InitTimeEffects()
    {
        Array arrayEffect = System.Enum.GetValues(typeof(EffectType));
        foreach (EffectType effectType in arrayEffect)
        {
            _timeEffects[effectType] = 2f;
        }
    }

    private IEnumerator DestroyEffect(EffectType effectType, GameObject obj)
    {
        yield return new WaitForSeconds(_timeEffects[effectType]);

        for (int i = 0; i < _effects[effectType].Count; i++)
        {
            if (obj.Equals(_effects[effectType][i]))
            {
                obj.SetActive(false);

                yield break;
            }
        }
    }

    private GameObject GetPooledObject(EffectType effectType)
    {
        if (!_effects.ContainsKey(effectType))
        {
            _effects[effectType] = new();
        }

        for (int i = 0; i < _effects[effectType].Count; i++)
        {
            if (!_effects[effectType][i].activeInHierarchy)
            {
                _effects[effectType][i].SetActive(true);
                return _effects[effectType][i];
            }
        }
        GameObject objEffect = GetGameObjectEffect(effectType);
        GameObject obj = Instantiate(objEffect);
        _effects[effectType].Add(obj);

        return obj;
    }
    private GameObject GetGameObjectEffect(EffectType effectType)
    {
        for (int i = 0; i < _prefabs.Count; i++)
        {
            if (_prefabs[i].name == effectType.ToString())
            {
                return _prefabs[i];
            }
        }
        return _prefabs[0];
    }
    public void PlayEffect(EffectType effectType, Transform transformSpawn)
    {
        GameObject obj = GetPooledObject(effectType);
        obj.transform.SetParent(transform, false);

        obj.transform.position = transformSpawn.position;
        _practicleSystem = obj.GetComponent<ParticleSystem>();
        _practicleSystem.Play();
        
        StartCoroutine(DestroyEffect(effectType, obj));
    }
    public void PlayEffect(EffectType effectType, int x, int y)
    {
        GameObject obj = GetPooledObject(effectType);

        obj.transform.SetParent(transform, false);
        obj.transform.position = new Vector2(x, y);
        _practicleSystem = obj.GetComponent<ParticleSystem>();
        _practicleSystem.Play();

        StartCoroutine(DestroyEffect(effectType, obj));

    }

}