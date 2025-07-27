using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorLevelExporter : EditorWindow
{
    private string outputPath = "Assets/Resources/Data/Levels";
    private int totalLevels = 20;
    private int minGrid = 2;
    private int maxGrid = 8;
    private int minBlocks = 4;
    private int maxBlocks = 16;
    private int maxSaws = 8;

    [MenuItem("Tools/Smart Level Exporter")]
    public static void ShowWindow() => GetWindow<EditorLevelExporter>("Smart Level Exporter");

    private void OnGUI()
    {
        GUILayout.Label("Auto Smart Generate Levels", EditorStyles.boldLabel);
        totalLevels = EditorGUILayout.IntField("Total Levels", totalLevels);
        minGrid = EditorGUILayout.IntField("Min Grid Size", minGrid);
        maxGrid = EditorGUILayout.IntField("Max Grid Size", maxGrid);
        minBlocks = EditorGUILayout.IntField("Min Block Count", minBlocks);
        maxBlocks = EditorGUILayout.IntField("Max Block Count", maxBlocks);
        maxSaws = EditorGUILayout.IntField("Max Saw Count", maxSaws);
        outputPath = EditorGUILayout.TextField("Output Path", outputPath);

        if (GUILayout.Button($"Generate {totalLevels} Levels"))
        {
            GenerateAllLevels();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Level Exported", $"Generated {totalLevels} levels to:\n{outputPath}", "OK");
        }

        if (GUILayout.Button("Clear Saved Levels"))
        {
            if (EditorUtility.DisplayDialog("Confirm Delete", "Delete all level JSON files in folder?", "Yes", "No"))
            {
                ClearSavedLevels();
                AssetDatabase.Refresh();
                Debug.Log("Cleared saved level files.");
            }
        }
    }

    private void GenerateAllLevels()
    {
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        for (int i = 1; i <= totalLevels; i++)
        {
            float t = (i - 1f) / (totalLevels - 1f);
            int gridSize = Mathf.RoundToInt(Mathf.Lerp(minGrid, maxGrid, t));
            int blockCount = Mathf.RoundToInt(Mathf.Lerp(minBlocks, maxBlocks, t));
            int sawCount = Mathf.RoundToInt(Mathf.Lerp(0, maxSaws, t));
            int moveCount = blockCount * 2;

            var level = new LevelData
            {
                levelId = i,
                Width = gridSize,
                Height = gridSize,
                moveCount = moveCount,
                blocks = GenerateBlocks(gridSize, gridSize, blockCount),
                sawBlades = GenerateSaws(gridSize, gridSize, sawCount)
            };

            string fileName = Path.Combine(outputPath, $"level_{i}.json");
            File.WriteAllText(fileName, level.ToJson());
            Debug.Log($"Level {i} => Grid: {gridSize}x{gridSize}, Blocks: {blockCount}, Saws: {sawCount}");
        }
    }

    private BlockData[] GenerateBlocks(int width, int height, int count)
    {
        var centerArea = GetCentralArea(width, height, count);
        var list = new List<BlockData>();

        for (int i = 0; i < count && i < centerArea.Count; i++)
        {
            var pos = centerArea[i];
            var dir = GetExitDirection(pos, width, height);
            int unlock = (i == 0) ? 0 : 1;
            list.Add(new BlockData { grid = pos, dir = dir, UnlockCount = unlock });
        }

        return list.ToArray();
    }

    // t?nh h??ng sao cho block ?i ra kh?i bi?n l??i
    private BlockDir GetExitDirection(Vector2Int pos, int width, int height)
    {
        int distLeft = pos.x + 1;
        int distRight = width - pos.x;
        int distDown = pos.y + 1;
        int distUp = height - pos.y;

        int minDist = Mathf.Min(distLeft, distRight, distDown, distUp);
        if (minDist == distLeft) return BlockDir.Left;
        if (minDist == distRight) return BlockDir.Right;
        if (minDist == distDown) return BlockDir.Down;
        return BlockDir.Up;
    }

    private List<Vector2Int> GetCentralArea(int width, int height, int desiredCount)
    {
        var result = new List<Vector2Int>();
        int marginX = Mathf.Max(0, (width - 4) / 2);
        int marginY = Mathf.Max(0, (height - 4) / 2);
        for (int x = marginX; x < width - marginX; x++)
            for (int y = marginY; y < height - marginY; y++)
                result.Add(new Vector2Int(x, y));

        // ch? l?y ?? count, ?? ?? shuffled t?i sinh
        for (int i = result.Count - 1; i > 0; i--)
        {
            int r = UnityEngine.Random.Range(0, i + 1);
            var tmp = result[i]; result[i] = result[r]; result[r] = tmp;
        }

        // c?t danh s?ch v? ?? d?i c?n thi?t
        if (result.Count > desiredCount)
            result.RemoveRange(desiredCount, result.Count - desiredCount);

        return result;
    }

    private SawBladeData[] GenerateSaws(int width, int height, int count)
    {
        var list = new List<SawBladeData>();
        var used = new HashSet<Vector2Int>();
        for (int i = 0; i < count; i++)
        {
            Vector2Int pos;
            int tries = 0;
            do { pos = new Vector2Int(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height)); tries++; }
            while (used.Contains(pos) && tries < 100);
            used.Add(pos);
            list.Add(new SawBladeData { grid = pos });
        }
        return list.ToArray();
    }

    private void ClearSavedLevels()
    {
        var dir = new DirectoryInfo(outputPath);
        if (!dir.Exists) return;
        foreach (var file in dir.GetFiles("level_*.json")) file.Delete();
    }
}
