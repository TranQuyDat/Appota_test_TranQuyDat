using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager _uiManager;
    public PuzzelGame _puzzelGame;
    public bool _isPause = false;
    private void Awake()
    {
        _uiManager = GetComponent<UIManager>();
    }
    private void Start()
    {
        GameStart();
    }
    private void Update()
    {
        checkPause();
        if (_puzzelGame.iswin)
            GameWin();
        else if (_puzzelGame.islose)
            GameLose();
        else if (_uiManager.CurrentPanel.GetId() == (int)PanelId.LosePanelId)
            _uiManager.CheckAndHandleResetClick(GameReset);
    }

    private void checkPause()
    {
        if (_uiManager.CurrentPanel.GetId() == (int)PanelId.PausePanelId && !_isPause)
        {
            _isPause = true;
            _puzzelGame.PauseGame(true);
        }
        else if (_uiManager.CurrentPanel.GetId() != (int)PanelId.PausePanelId && _isPause)
        {
            _isPause = false;
            _puzzelGame.PauseGame(false);
        }
    }

    private void GameWin()
    {
        print("game win");
        _puzzelGame.clearGame();
        // next level
    }
    private void GameLose()
    {
        print("game lose");
        _puzzelGame.clearGame();
        // show ui lose
        _uiManager.ShowPanel(PanelId.LosePanelId);
    }

    private void GameStart()
    {
        print("game start");
        _puzzelGame.StartGame();
    }
    private void GameReset()
    {
        print("game reset");
        _puzzelGame.clearGame();
        _puzzelGame.StartGame();
    }


}
