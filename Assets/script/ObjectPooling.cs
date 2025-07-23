using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling
{
    private static Dictionary<string, List<GameObject>> _Pools;
    public static GameObject GetPool(string key, GameObject prefab = null)
    {
        if (_Pools == null) _Pools = new Dictionary<string, List<GameObject>>();
        if (!_Pools.ContainsKey(key))
        {
            _Pools[key] = new List<GameObject>();
        }

        GameObject obj;
        if (_Pools[key].Count <= 0)
        {
            //create
            obj = (prefab != null) ? GameObject.Instantiate(prefab) : new GameObject();
            return obj;
        }
        obj = _Pools[key][0];
        _Pools[key].Remove(obj);

        return obj; //get fr listPool
    }

    public static void ReleaseToPool(GameObject obj, string key)
    {
        obj.SetActive(false);
        _Pools[key].Add(obj);
    }
}