using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager _uiManager;
    public LevelManager _levelManager;
    public PuzzelGame _puzzelGame;
    public bool _isPause = false;
    private void Awake()
    {
        _uiManager = GetComponent<UIManager>();
        _levelManager = GetComponent<LevelManager>();
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.Portrait;
    }
    private void Start()
    {
        GameStart();
    }
    private void Update()
    {
        if (_puzzelGame.iswin)
            GameWin().Forget();
        else if (_puzzelGame.islose)
            GameLose();
        else if (_uiManager.CurrentPanel.GetId() == (int)PanelId.LosePanelId)
            _uiManager.CheckAndHandleResetClick(GameReset);
    }

    private async UniTask GameWin()
    {
        _puzzelGame.clearGame();
        // next level
        int newLevelId = _levelManager.CurrentLevelId % _levelManager.LevelCount +1;
        bool b = _levelManager.ChangeLevel(newLevelId);
        if (!b) return;
        _puzzelGame.Init(_levelManager.CurrentLevelId);

        await UniTask.DelayFrame(1);
        await _puzzelGame.StartGame();

        SaveSystem.Save("SaveData", new SaveData(_levelManager.CurrentLevelId).ToJson());
    }
    private void GameLose()
    {
        _puzzelGame.clearGame();
        // show ui lose
        _uiManager.ShowPanel(PanelId.LosePanelId);
    }

    private void GameStart()
    {
        _puzzelGame.Init(_levelManager.CurrentLevelId);
        _puzzelGame.StartGame().Forget();
    }
    private void GameReset()
    {
        _puzzelGame.clearGame();
        _puzzelGame.StartGame().Forget();
    }


}
