using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzelGame : MonoBehaviour
{
    public LevelLoader _levelLoader;
    public PanelEvents _panelEvents;
    public Transform _content;
    private int _CurLevelid;
    private bool _Istart = false;
    private int _moveCount = 7;
    private int _blockCount = 4;
    private Block _blockTarget = null;
    private void Update()
    {
        if (!_Istart) return;
        if (_blockTarget != null && _blockTarget.IsDestroyed)
        {
            _blockCount--;
            _blockTarget = null;
        }
        if (!Block.IsAnyblockMoving && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            HandleTouch(Input.touches[0]);
        }
    }
    public void Init(int levelId)
    {
        _CurLevelid = levelId;
    }

    public async UniTask StartGame()
    {
        _blockTarget = null;
        LevelData levedata = await _levelLoader.LoadLevel(_CurLevelid);
        if (_levelLoader == null || levedata == null) return;
        _moveCount = levedata.moveCount;
        _blockCount = levedata.blocks.Length;
        _panelEvents.SetTextLevel?.Invoke(_CurLevelid);
        _panelEvents.SetTextMoveCount?.Invoke(_moveCount);
        _Istart = true;
    }
    public void PauseGame(bool b)
    {
        _Istart = !b;
    }
    public void clearGame()
    {
        _Istart = false;
        foreach (Transform child in _content)
        {
            if (child.TryGetComponent<IDestroyable>(out IDestroyable destroyable))
            {
                destroyable.OnDestroy();
            }
        }
    }
    private void HandleTouch(Touch touch)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, 100f, LayerMask.GetMask("Block")))
        {
            SoundManager.Instance.PlaySFX(SFXType.Click);
            _blockTarget = hit.collider.GetComponent<Block>();
            if (!_blockTarget.Isunlock) return;
            if (_blockTarget != null)
            {
                _blockTarget.StartMove();
                _moveCount--;
                _panelEvents.SetTextMoveCount?.Invoke(_moveCount);
            }
        }
    }

    public bool islose => _Istart && _moveCount <= 0 && _blockCount > 0;
    public bool iswin => _Istart && _blockCount <= 0 && _moveCount >= 0;

}
