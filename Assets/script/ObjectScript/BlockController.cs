using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockController : MonoBehaviour,IDestroyable
{
    public BoxCollider _boxCollider;
    private Block _block;
    private float _speed = 5f;
    private bool _canMove = false;
    private bool _isDestroyed = false;

    private static bool _isAnyblockMoving = false;
    public static bool _isPause = false;
    private void Awake()
    {
        _block =  this.GetComponent<Block>();
    }
    private void Update()
    {
        if (_isPause || !_canMove ) return;
        if (ScanAndReact(_block.Dir)) return ;
        move(_block.Dir);
        AutoInActive();
    }
    private void OnEnable()
    {
        _isDestroyed = false;
    }
    private void move(Vector3 dir)
    {
        _isAnyblockMoving = true;
        transform.position += dir * _speed * Time.deltaTime;
    }
    

    private bool ScanAndReact(Vector3 dir)
    {
        float moveDistance = _speed * Time.deltaTime;

        Vector3 halfExtents = _boxCollider.bounds.extents;
        Quaternion rotation = transform.rotation;
        Vector3 origin = transform.position;

        if (!Physics.BoxCast(origin, halfExtents, dir, out RaycastHit hit, rotation, moveDistance, 
            LayerMask.GetMask("Block", "OneHitKill"))) 
            return false;

        if (hit.collider.CompareTag("Block"))
        {
            // Di chuyển tới sát vật cản, trừ 1 chút
            transform.position += dir * (hit.distance - 0.1f);
            _canMove = false;
            _isAnyblockMoving = false;
            return true;
        }
        else
        {
            OnDestroy();
            return true;
        }
    }

   
    
    private void AutoInActive()
    {
        Rect b = ScreenInfo.ViewportRectOnXY;
        if (b.Contains(transform.position)) return;
        _canMove = false;
        _isAnyblockMoving = false;
        _isDestroyed = true;
        ObjectPooling.ReleaseToPool(gameObject, "Block");
    }
   
    public float Speed => _speed;
    public void CanMove() => _canMove = true;
    public bool IsDestroyed => _isDestroyed;
    public static bool IsAnyblockMoving => _isAnyblockMoving;

    public void OnDestroy()
    {
        _isAnyblockMoving = false;
        _canMove = false;
        _isDestroyed = true;
        ObjectPooling.ReleaseToPool(gameObject, "Block");
    }

}
