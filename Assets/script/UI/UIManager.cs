using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelId
{
    PlayPanelId = 0,
    LosePanelId = 1,
}
public class UIManager : MonoBehaviour
{
    public IPanel[] _uiPanels;
    public IPanel _currentPanel = null;
    public Transform _parentOfPanels;
    private bool _isResetClicked = false;
    private void Awake()
    {
        _uiPanels = new IPanel[_parentOfPanels.childCount];
        foreach (Transform child in _parentOfPanels)
        {
            IPanel panel = child.GetComponent<IPanel>();
            if (panel != null)
            {
                _uiPanels[panel.GetId()] = panel;
            }
        }
    }
    private void Start()
    {
        ShowPanel(PanelId.PlayPanelId);
    }
    public void ShowPanel(PanelId panelId)
    {
        if(_currentPanel !=null) _currentPanel.Hide();
        if (_uiPanels[(int)panelId] != null)
        {
            _currentPanel = _uiPanels[(int)panelId];
            _currentPanel.Show();
        }
    }
    public void BtnReset()=> _isResetClicked = true;
    public bool CheckAndHandleResetClick(Action funct = null)
    {
        if (!_isResetClicked) return false;
        funct?.Invoke();
        _isResetClicked = false;
        ShowPanel(PanelId.PlayPanelId);
        return true;
    }

    public IPanel CurrentPanel => _currentPanel;

}
