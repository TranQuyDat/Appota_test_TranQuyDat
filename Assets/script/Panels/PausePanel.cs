using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour, IPanel
{
    public UIManager _uiManager;
    public Button _btnResume;
    public int GetId() => (int)PanelId.PausePanelId;

    public void Show()
    {
        this.gameObject.SetActive(true);
        _btnResume.onClick.AddListener(btnResume);
    }
    public void Hide()
    {
        _btnResume.onClick.RemoveListener(btnResume);
        this.gameObject.SetActive(false);
    }


    private void btnResume()
    {
        _uiManager.ShowPanel(PanelId.PlayPanelId);
    }
}
