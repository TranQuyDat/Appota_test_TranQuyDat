using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzelGame : MonoBehaviour
{
    public LevelLoader _levelLoader;
    public PanelEvents _panelEvents;
    public Transform _content;
    private int _CurLevelid;
    public bool _Istart = false;
    private int _moveCount = 7;
    private int _blockCount = 4;
    private BlockController _blockTarget = null;
    private void Update()
    {
        if (!_Istart) return;
        if (_blockTarget != null && _blockTarget.IsDestroyed)
        {
            _blockCount--;
            _blockTarget = null;
        }
        
        OnTouched();
    }
    public void Init(int levelId)
    {
        _CurLevelid = levelId;
    }

    public void StartGame()
    {
        _blockTarget = null;
        LevelData levedata = _levelLoader.LoadLevel(_CurLevelid);
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
        BlockController._isPause = b;
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
    private void OnTouched()
    {
        if (BlockController.IsAnyblockMoving || Input.touches.Length <= 0) return;

        Touch touch = Input.touches[0];
        if (touch.phase != TouchPhase.Began) return;
        RaycastHit hit;
        Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hit, 100f, LayerMask.GetMask("Block"));
        if (hit.collider == null) return;
        _blockTarget = hit.collider.GetComponent<BlockController>();
        _blockTarget.CanMove();
        _moveCount--;
    }

    public bool islose => _Istart && _moveCount <= 0 && _blockCount > 0;
    public bool iswin => _Istart && _blockCount <= 0 && _moveCount >= 0;

}
