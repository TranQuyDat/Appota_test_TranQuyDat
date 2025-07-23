using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : MonoBehaviour, IPanel
{
    public UIManager _uiManager;
    public PanelEvents _panelEvents;

    public TextMeshProUGUI _txtLevel;
    public TextMeshProUGUI _txtMoveCount;
    public Button _btnPause;
    public Button _btnMute;

    public int GetId()=>(int)PanelId.PlayPanelId;
    public void Show()
    {
        this.gameObject.SetActive(true);
        _btnPause.onClick.AddListener(btnPause);
        _btnMute.onClick.AddListener(btnMute);
        _panelEvents.SetTextLevel += SetTextLevel;
        _panelEvents.SetTextMoveCount += SetTextMoveCount;
    }

    public void Hide()
    {
        _btnPause.onClick.RemoveListener(btnPause);
        _btnMute.onClick.RemoveListener(btnMute);
        _panelEvents.SetTextLevel -= SetTextLevel;
        _panelEvents.SetTextMoveCount -= SetTextMoveCount;
        this.gameObject.SetActive(false);
    }

    public void btnPause()
    {
        _uiManager.ShowPanel(PanelId.PausePanelId);
    }
    public void btnMute()
    {
        Debug.Log("Mute");
    }

    public void SetTextLevel(int level)
    {
        _txtLevel.text = "Level: " + level;
    }

    public void SetTextMoveCount(int moveCount)
    {
        _txtMoveCount.text = "Moves: " + moveCount;
    }

}
