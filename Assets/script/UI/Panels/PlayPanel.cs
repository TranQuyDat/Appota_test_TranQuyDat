using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : MonoBehaviour, IPanel
{
    public SettingData _settingData;
    public UIManager _uiManager;
    public PanelEvents _panelEvents;
    public Image _iconMute;
    public Sprite _UnMute;
    public Sprite _Mute;
    public TextMeshProUGUI _txtLevel;
    public TextMeshProUGUI _txtMoveCount;
    public Button _btnMute;

    public int GetId()=>(int)PanelId.PlayPanelId;
    public void Show()
    {
        this.gameObject.SetActive(true);
        _btnMute.onClick.AddListener(btnMute);
        _panelEvents.SetTextLevel += SetTextLevel;
        _panelEvents.SetTextMoveCount += SetTextMoveCount;

        _iconMute.sprite = _settingData.isMuted ? _UnMute : _Mute;
        SaveSystem.Save("SettingData", _settingData.ToJson());
    }

    public void Hide()
    {
        _btnMute.onClick.RemoveListener(btnMute);
        _panelEvents.SetTextLevel -= SetTextLevel;
        _panelEvents.SetTextMoveCount -= SetTextMoveCount;
        this.gameObject.SetActive(false);
    }
    public void btnMute()
    {
        SoundManager.Instance.PlaySFX(SFXType.Click);
        _settingData.isMuted = !_settingData.isMuted;
        _iconMute.sprite = _settingData.isMuted? _UnMute: _Mute;
        SaveSystem.Save("SettingData",_settingData.ToJson());
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
