using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public MeshRenderer _meshRenderer;
    private Vector3 _dir;
    public void Init(BlockData blockData)
    {
        _dir = blockData.dir switch
        {
            BlockDir.up => transform.up,
            BlockDir.right => transform.right,
            BlockDir.left => -transform.right,
            BlockDir.down => -transform.up,
            _ => throw new System.ArgumentOutOfRangeException()
        };
        _meshRenderer.material.color = blockData.color;
    }

    public Vector3 Dir => _dir;

}