using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour, IDestroyable
{
    public Animation _animation;
    private void Start()
    {
        _animation.Play();
    }
    public void OnDestroy()
    {
        ObjectPooling.ReleaseToPool(gameObject, "SawBlade");
    }
}
