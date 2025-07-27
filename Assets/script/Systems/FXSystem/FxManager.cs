using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum FxType { destroy,};
public class FxManager : MonoBehaviour
{
    public static FxManager Instance;

    [System.Serializable]
    public class FXEntry
    {
        public FxType fxType;
        public GameObject prefab;
    }

    public List<FXEntry> fxList = new List<FXEntry>();
    private Dictionary<FxType, GameObject> fxDict;
    public Dictionary<FxType, GameObject> FxDict => fxDict;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        fxDict = new Dictionary<FxType, GameObject>();

        foreach (var fx in fxList)
        {
            if (!fxDict.ContainsKey(fx.fxType) && fx.prefab != null)
                fxDict.Add(fx.fxType, fx.prefab);
        }
    }

    public async UniTask PlayFX(FxType fxType, Vector3 position)
    {
        if (fxDict.TryGetValue(fxType, out GameObject fxPrefab))
        {
            GameObject fx = ObjectPooling.GetPool(""+ fxType, fxPrefab);
            fx.transform.position = position;
            fx.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            ObjectPooling.ReleaseToPool(fx, "" + fxType);
        }
        else
        {
            Debug.LogWarning($"FX '{fxType}' not found.");
        }
    }
    public async UniTask PlayFXWithColor(FxType fxType, Vector3 position, Color color)
    {
        if (fxDict.TryGetValue(fxType, out GameObject fxPrefab))
        {
            GameObject fx = ObjectPooling.GetPool(fxType.ToString(), fxPrefab);
            fx.transform.position = position;
            fx.SetActive(true);

            // Gán màu cho material của ParticleSystem
            var psRenderer = fx.GetComponent<ParticleSystemRenderer>();
            if (psRenderer != null && psRenderer.material.HasProperty("_Color"))
            {
                psRenderer.material.color = color;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));
            ObjectPooling.ReleaseToPool(fx, "" + fxType);
        }
    }

}
