using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[CreateAssetMenu(fileName = "SettingData",menuName = "Data/SettingData")]
public class SettingData : ScriptableObject
{
    public bool isMuted = true;
    public string ToJson() => JsonUtility.ToJson(this);
    public void FromJson(string json)
    {
        SettingDataRaw s = JsonUtility.FromJson<SettingDataRaw>(json);
        isMuted = s.isMuted;
    }

    [System.Serializable]
    private class SettingDataRaw
    {
        public bool isMuted;

        public SettingDataRaw() { }

        public SettingDataRaw(SettingData data)
        {
            isMuted = data.isMuted;
        }
    }
}
