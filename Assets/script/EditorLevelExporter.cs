using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorLevelExporter : MonoBehaviour
{
    public LevelData levelData;
    private string path = "Assets/Data";
    public bool isExporting = false;
    // Update is called once per frame
    private void OnValidate()
    {

        if (!isExporting) return;
        ExportLevel();
        isExporting = false;
    }

    public void ExportLevel()
    {
        if (levelData == null)
        {
            Debug.LogError("Level data is null!");
            return;
        }
        string json = levelData.ToJson();
        string filePath = $"{path}/level_{levelData.levelId}.json";
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log($"Level data exported to {filePath}");
    }

}
