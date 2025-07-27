using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LosePanel : MonoBehaviour ,IPanel
{
    public UIManager _uiManager;
    public Button _btnReset;
    public int GetId() => (int)PanelId.LosePanelId;
    public void Show()
    {
        this.gameObject.SetActive(true);
        _btnReset.onClick.AddListener(btnReset);
    }
    public void Hide()
    {
        _btnReset.onClick.RemoveListener(btnReset);
        this.gameObject.SetActive(false);
    }

    private void btnReset() 
    {
        _uiManager.BtnReset();
        SoundManager.Instance.PlaySFX(SFXType.Click);
    }
}
