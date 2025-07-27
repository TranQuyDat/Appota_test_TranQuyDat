using UnityEngine;
using System.Collections.Generic;

public enum BGMType { PuzzleTheme }
public enum SFXType { Click,touch, Move, Destroy }

[System.Serializable]
public class BGMEntry { public BGMType type; public AudioClip clip; }
[System.Serializable]
public class SFXEntry { public SFXType type; public AudioClip clip; }

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance;
    public SettingData _settingData;
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public List<BGMEntry> bgmList;
    public List<SFXEntry> sfxList;

    private Dictionary<BGMType, AudioClip> bgmDict = new();
    private Dictionary<SFXType, AudioClip> sfxDict = new();
    private bool _isMuted;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        foreach (var entry in bgmList) bgmDict[entry.type] = entry.clip;
        foreach (var entry in sfxList) sfxDict[entry.type] = entry.clip;
    }
    private void Start()
    {
        string json = SaveSystem.Load("SettingData");
        if(json == "")
        {
            _settingData.isMuted = true;
            _isMuted = false;
            return;
        }
        _settingData.FromJson(json);
        if (_settingData.isMuted)
        {
            bgmSource.Stop();
            _isMuted = _settingData.isMuted;
        }
    }
    private void Update()
    {
        if(_isMuted == _settingData.isMuted) return;

        _isMuted = _settingData.isMuted;
        if (_settingData.isMuted)
        {
            bgmSource.Stop();
        }
        else
        {
            PlayBGM(BGMType.PuzzleTheme);
            bgmSource.Play();
        }
    }
    public void PlayBGM(BGMType type)
    {
        if (bgmDict.TryGetValue(type, out var clip))
        {
            if (bgmSource.clip != clip)
            {
                bgmSource.clip = clip;
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }
    }

    public void PlaySFX(SFXType type)
    {
        if (sfxDict.TryGetValue(type, out var clip))
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
