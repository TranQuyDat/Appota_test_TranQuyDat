using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public MeshRenderer _meshRenderer;
    public BoxCollider _boxCollider;

    private float _speed = 5f;
    private bool _canMove = false;
    private Vector3 _dir;
    private void Update()
    {
        CheckTouched();
        if (!_canMove ) return;
        move(_dir);
    }
    private void move(Vector3 dir)
    {
        float moveDistance = _speed * Time.deltaTime;

        Vector3 halfExtents = GetComponent<BoxCollider>().bounds.extents;
        Quaternion rotation = transform.rotation;
        Vector3 origin = transform.position;

        if (Physics.BoxCast(origin, halfExtents, dir, out RaycastHit hit, rotation, moveDistance, LayerMask.GetMask("Block")))
        {
            // Di chuyển tới sát vật cản, trừ 1 chút
            transform.position += dir * (hit.distance - 0.1f);
            _canMove = false;
            return;
        }

        transform.position += dir * moveDistance;
    }

    private void CheckTouched()
    {
        if (Input.touches.Length <= 0) return;
        Touch touch = Input.touches[0];
        if (touch.phase != TouchPhase.Began) return;
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, 100f, LayerMask.GetMask("Block"));
        if (hit.collider == null || hit.collider.gameObject != gameObject) return;
        _canMove = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null) return;
        _canMove = false;
    }
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

    public float Speed => _speed;

    public void OnDestroy()
    {
        _canMove = false;
    }

    public bool ShowGizmos = false;
    private void OnDrawGizmos()
    {
        if (!ShowGizmos) return;
        Gizmos.color = Color.black;
        float moveDistance = _speed * Time.deltaTime;
        Gizmos.DrawLine(transform.position, transform.position + transform.position * moveDistance);
    }
}
