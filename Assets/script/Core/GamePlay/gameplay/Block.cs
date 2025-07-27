using System;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class Block : MonoBehaviour ,IDestroyable
{
    public TrailRenderer _trailRenderer;
    public MeshRenderer _meshRenderer;
    public TextMesh _txtUnlockCount;
    public MaterialAssetData _MaterialAssetData;
    
    private bool _canMove = false;
    private static bool _isAnyblockMoving = false;
    private float _speed = 0f;
    private Vector2 _nodeSize;
    private MaterialAssetData.MaterialType _materialType;
    private Vector3 _dir;
    private static event Action<Block> _onOtherBlockDestroyed;
    private int _unlockCount = 0;

    private void Update()
    {
        if (!_canMove) return;
        AutoInActive();
    }
    private void FixedUpdate()
    {
        if (!_canMove || ScanAndReact(_dir)) return;
        Move();
    }

    private void OnEnable()
    {
        _speed = 5f;
        _trailRenderer.emitting = false;
    }
    private void OnDisable()
    {
        _onOtherBlockDestroyed -= OnOtherBlockDestroyedHandle;
        _trailRenderer.emitting = false;
        _trailRenderer.Clear();
    }

    public void Init(BlockData blockData, Vector2 nodeSize)
    {
        _nodeSize = nodeSize;
        _unlockCount = blockData.UnlockCount;
        switch (blockData.dir)
        {
            case BlockDir.up:
                _materialType = MaterialAssetData.MaterialType.mat_up;
                _dir = Vector3.up;
                break;
            case BlockDir.right:
                _materialType = MaterialAssetData.MaterialType.mat_right;
                _dir = Vector3.right;
                break;
            case BlockDir.left:
                _materialType = MaterialAssetData.MaterialType.mat_left;
                _dir = Vector3.left;
                break;
            case BlockDir.down:
                _materialType = MaterialAssetData.MaterialType.mat_down;
                _dir = Vector3.down;
                break;
        }
        
        if (blockData.UnlockCount <= 0)
        {
            _meshRenderer.material = _MaterialAssetData.GetMaterial(_materialType);
            return;
        }
        _meshRenderer.material = _MaterialAssetData.GetMaterial(MaterialAssetData.MaterialType.mat_lock);
        _txtUnlockCount.gameObject.SetActive(true);
        _txtUnlockCount.text = ""+ blockData.UnlockCount;
        _onOtherBlockDestroyed += OnOtherBlockDestroyedHandle;
    }

    private void Move()
    {
        _isAnyblockMoving = true;
        _speed += 0.3f;
        _trailRenderer.emitting = _speed*Time.fixedDeltaTime>0.13f;

        transform.position = Vector3.MoveTowards(transform.position,transform.position + _dir,
            _speed * Time.fixedDeltaTime) ;
    }

    public void AutoInActive()
    {
        Rect r = ScreenInfo.ViewportRectOnXY;
        if (r.Contains(transform.position)) return;
        OnDestroy();
    }

    private bool ScanAndReact(Vector3 dir)
    {
        Vector3 scaledStep = Vector3.Scale(dir, _nodeSize/2);
        float moveDistance = scaledStep.magnitude;

        Vector3 origin = transform.position;


        if (!Physics.Raycast(origin, dir, out RaycastHit hit, moveDistance, LayerMask.GetMask("Block", "OneHitKill")))
            return false;

        _canMove = false;

        if (hit.collider.CompareTag("Block"))
        {
            _speed = 5f;
            BounceBack(hit.transform, dir, () =>
            {
                _isAnyblockMoving = false;
                _trailRenderer.enabled = false;
            });
            return true;
        }
        else
        {
            Color color = _meshRenderer.material.GetColor("_BaseColor");
            FxManager.Instance.PlayFXWithColor(FxType.destroy, transform.position, color).Forget();
            OnDestroy();
            return true;
        }
    }

    private void BounceBack(Transform other, Vector3 dir, Action OnComplete = null)
    {
        float time = 0.1f;
        Vector3 dis = dir.normalized * 0.2f;
        Vector3 otherPos = other.transform.position;
        Vector3 selflPos = otherPos - Vector3.Scale(_dir, _nodeSize);

        Sequence s = DOTween.Sequence();
        s.Append(other.transform.DOMove(other.position + dis, time).SetEase(Ease.OutQuad));
        s.Append(other.transform.DOMove(otherPos, time).SetEase(Ease.OutQuad));
        s.Append(transform.DOMove(selflPos, time).SetEase(Ease.InQuad));

        if (OnComplete != null)
            s.OnComplete(() => OnComplete());
    }

    public void OnOtherBlockDestroyedHandle(Block b)
    {
        if (b == this) return;
        _unlockCount--;
        _txtUnlockCount.text = "" + _unlockCount;

        if (_unlockCount > 0) return;
        _meshRenderer.material = _MaterialAssetData.GetMaterial(_materialType);
        _onOtherBlockDestroyed -=OnOtherBlockDestroyedHandle;
        _txtUnlockCount.gameObject.SetActive(false);
    }

    public void StartMove() => _canMove = true;
    public bool Isunlock => _unlockCount <= 0;
    public static bool IsAnyblockMoving => _isAnyblockMoving;
    public bool IsDestroyed => !gameObject.activeSelf;

    public void OnDestroy()
    {
        _onOtherBlockDestroyed?.Invoke(this);
        _isAnyblockMoving = false;
        _canMove = false;
        ObjectPooling.ReleaseToPool(gameObject, "Block");
    }
}
