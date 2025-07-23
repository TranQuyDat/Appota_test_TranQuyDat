using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour, IDestroyable
{
    public void OnDestroy()
    {
        ObjectPooling.ReleaseToPool(gameObject, "SawBlade");
    }
}
