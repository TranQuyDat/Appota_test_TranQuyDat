using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BlockDir {up , right , left , down }
[Serializable]
public class BlockData
{
    public Vector2 grid;
    public Color color;
    public BlockDir dir;
}

[Serializable]
public class LevelData
{ 
    public int levelId;
    public int Width;
    public int Height;
    public BlockData[] blocks;
    public int moveCount;

    public string ToJson()
    {
        string json = JsonUtility.ToJson(this, true);
        Debug.Log(json);
        return json;
    }
    public void FromJson(string json)
    {
        LevelData data = JsonUtility.FromJson<LevelData>(json);
        levelId = data.levelId;
        blocks = data.blocks;
        Width = data.Width;
        Height = data.Height;
        moveCount = data.moveCount;
    }
}
