using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="panelEvents" , menuName = "Event/panelEvents")]
public class PanelEvents : ScriptableObject
{
    public Action<int> SetTextLevel;
    public Action<int> SetTextMoveCount;

}
