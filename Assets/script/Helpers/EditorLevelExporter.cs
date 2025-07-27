using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditorLevelExporter : EditorWindow
{
    private string outputPath = "Assets/Resources/Data/Levels";
    private int maxLevelCount = 20;
    private int fromLevel = 1;
    private int tolevel = 20;
    private int minGrid = 2;
    private int maxGrid = 8;
    private int minBlocks = 4;
    private int maxSaws = 8;
    private int levelActive_Saw = 8;
    private int levelActive_Unlockcount = 8;
    BlockCellData[,] cells;

    [MenuItem("Tools/Smart Level Exporter")]
    public static void ShowWindow() => GetWindow<EditorLevelExporter>("Smart Level Exporter");

    private void OnGUI()
    {
        GUILayout.Label("Auto Smart Generate Levels", EditorStyles.boldLabel);
        maxLevelCount = EditorGUILayout.IntField("Max Level Count", maxLevelCount);
        GUILayout.BeginHorizontal();
        tolevel = EditorGUILayout.IntSlider("To Level", tolevel, 1, maxLevelCount);
        fromLevel = EditorGUILayout.IntSlider("From Level", fromLevel, 1, tolevel);
        GUILayout.EndHorizontal();
        minGrid = EditorGUILayout.IntField("Min Grid Size", minGrid);
        maxGrid = EditorGUILayout.IntField("Max Grid Size", maxGrid);
        minBlocks = EditorGUILayout.IntField("Min Block Count", minBlocks);
        maxSaws = EditorGUILayout.IntField("Max Saw Count", maxSaws);
        outputPath = EditorGUILayout.TextField("Output Path", outputPath);

        GUILayout.BeginHorizontal();
        levelActive_Saw = EditorGUILayout.IntSlider("Level Active Saw", levelActive_Saw, 1, maxLevelCount);
        levelActive_Unlockcount = EditorGUILayout.IntSlider("Level Active unlockcount", levelActive_Unlockcount, 1, maxLevelCount);
        GUILayout.EndHorizontal();

        if (GUILayout.Button($"Generate {tolevel - fromLevel + 1} Levels"))
        {
            GenerateAllLevels();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Level Exported", $"Generated lv {fromLevel}->{tolevel} levels to:\n{outputPath}", "OK");
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

        if (GUILayout.Button("Reset all playerPrefs"))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    private void GenerateAllLevels()
    {
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        int tiercount = Mathf.FloorToInt(maxLevelCount / 5);

        for (int i = fromLevel; i <= tolevel; i++)
        {
            int tier = Mathf.FloorToInt((i - 1) / 5);
            int blockCount = 3 * i;
            int gridSize = Mathf.Clamp(Mathf.CeilToInt(Mathf.Sqrt(blockCount)), minGrid, maxGrid);
            blockCount = Mathf.Clamp(blockCount, minBlocks, gridSize * gridSize);
            int sawCount = Mathf.Min(maxSaws, (gridSize * gridSize - blockCount) / 2);
            int moveCount = blockCount + 3;

            createGrid(gridSize, gridSize);
            var level = new LevelData
            {
                levelId = i,
                Width = gridSize,
                Height = gridSize,
                moveCount = moveCount,
                blocks = GenerateBlocks(gridSize, gridSize, blockCount),
                sawBlades = (i >= levelActive_Saw )?GenerateSaws(gridSize, gridSize , maxSaws):null
            };
            if(i >= levelActive_Unlockcount)
                SetUnlockCountFor(level.blocks);

            string fileName = Path.Combine(outputPath, $"level_{i}.json");
            File.WriteAllText(fileName, level.ToJson());
            Debug.Log($"Level {i} => Grid: {gridSize}x{gridSize}, Blocks: {blockCount}, Saws: {sawCount}");
        }
    }
    // tao grid voi cac huong random
    public void createGrid(int width, int height)
    {
        cells = new BlockCellData[width, height];
        var dir = new List<Vector2>(4) { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                cells[i, j] = new BlockCellData(i, j, dir[UnityEngine.Random.Range(0, dir.Count)]);
            }
        }
    }
    // tao cac blockdata da valid 
    private BlockData[] GenerateBlocks(int width, int height, int count)
    {
        List<BlockCellData> cellsEmpty = GetCellEmptyBlock();
        var list = new List<BlockData>();

        BlockDir convertDir(Vector2 v)=>
            v == Vector2.up ? BlockDir.up :
            v == Vector2Int.down ? BlockDir.down :
            v == Vector2Int.left ? BlockDir.left :
            v == Vector2Int.right ? BlockDir.right :
            throw new System.Exception("Invalid direction");
        
        for (int i = 0; i < count; i++)
        {
            if (cellsEmpty.Count == 0)
            {
                Debug.LogWarning("cellsEmpty rỗng - không thể tạo thêm block nữa");
                break;
            }

            int randomcell = UnityEngine.Random.Range(0, cellsEmpty.Count);
            var cell = cellsEmpty[randomcell];
            BlockData b = new BlockData()
            {
                grid = cell.gridindex,
                dir = convertDir(cell.dir),
            };

            cell.isEmptyBlock = false;
            cellsEmpty.RemoveAt(randomcell);
            list.Add(b);

            Validcells(cell, cellsEmpty, width, height);
        }

        return list.ToArray();
    }
    //valid cellsemty (loai tat ca cac cell doi nghich voi cell da random)
    private void Validcells(BlockCellData b, List<BlockCellData> cellsEmpty, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            if (x == b.x) continue;
            var other = cells[x, b.y];
            if (other != null && b.IsCollisionWith(other))
            {
                cellsEmpty.Remove(other);
            }
        }

        for (int y = 0; y < height; y++)
        {
            if (y == b.y) continue;
            var other = cells[b.x, y];
            if (other != null && b.IsCollisionWith(other))
            {
                cellsEmpty.Remove(other);
            }
        }
    }

    //
    private SawBladeData[] GenerateSaws(int width, int height, int maxSaw)
    {
        var cells = GetCellEmptyBlock();
        int count = Mathf.Min(cells.Count / 2, maxSaw);
        var list = new List<SawBladeData>();

        for (int i = 0; i < count; i++)
        {
            var c = cells[UnityEngine.Random.Range(0, cells.Count)];
            list.Add(new SawBladeData { grid = c.gridindex });
            cells.Remove(c); // tránh trùng
        }

        return list.ToArray();
    }


    public void SetUnlockCountFor(BlockData[] blocks)
    {
        List<BlockCellData> cellsNotEmpty = GetCellNotEmptyBlock();
        List<BlockCellData> cellsNearEmpty = GetBlocksNextToEmpty();
        int n = Mathf.FloorToInt(blocks.Length / 2);

        for (int i = 0; i < n; i++)
        {
            int ranid = UnityEngine.Random.Range(0, blocks.Length);
            BlockData b = blocks[ranid];
            BlockCellData cell = cells[(int)b.grid.y, (int)b.grid.x]; // Lưu ý: y là dòng

            if (cellsNearEmpty.Contains(cell) && cellsNearEmpty.Count > 1)
            {
                b.UnlockCount = cellsNearEmpty.Count - 1;
            }
            else
            {
                b.UnlockCount = BlockConnectionCount(cell);
            }
        }
    }

    public int CountConnectedBlocks(BlockCellData start)
    {
        int count = 0;
        var current = start;

        while (true)
        {
            Vector2Int dir = new Vector2Int((int)current.dir.x,(int)current.dir.x);
            Vector2Int nextPos = current.gridindex + dir;

            if (!IsInBounds(nextPos)) break;

            var next = cells[nextPos.y, nextPos.x];

            if (next.isEmptyBlock) break;

            count++;
            current = next;
        }

        return count;
    }
    public List<BlockCellData> GetBlocksNextToEmpty()
    {
        var result = new List<BlockCellData>();
        var cellsNotEmpty = GetCellNotEmptyBlock();

        foreach (var cell in cellsNotEmpty)
        {
            Vector2Int dir =  new Vector2Int((int)cell.dir.x,(int)cell.dir.x);
            Vector2Int nextPos = cell.gridindex + dir;

            if (IsInBounds(nextPos) && cells[nextPos.y, nextPos.x].isEmptyBlock)
            {
                result.Add(cell);
            }
        }

        return result;
    }

    public bool IsInBounds(Vector2Int pos) =>
        pos.x >= 0 && pos.x < cells.GetLength(1) &&
        pos.y >= 0 && pos.y < cells.GetLength(0);

    public int BlockConnectionCount(BlockCellData bcell)
    {
        int count = 0;
        int x = bcell.x;
        int y = bcell.y;
        Vector2 dir = bcell.dir; 

        while (true)
        {
            x += (int)dir.x;
            y += (int)dir.y;

            if (x < 0 || x >= cells.GetLength(1) || y < 0 || y >= cells.GetLength(0))
                break;

            if (cells[y, x].isEmptyBlock)
            {
                count--;
                break;
            }

            count++;
        }

        return count;
    }



    private void ClearSavedLevels()
    {
        var dir = new DirectoryInfo(outputPath);
        if (!dir.Exists) return;

        foreach (var file in dir.GetFiles("level_*.json"))
            file.Delete();
    }

    public List<BlockCellData> GetCellEmptyBlock()
    {
        return cells.Cast<BlockCellData>().Where(c => c.isEmptyBlock).ToList();
    }
    public List<BlockCellData> GetCellNotEmptyBlock()
    {
        return cells.Cast<BlockCellData>().Where(c => !c.isEmptyBlock).ToList();
    }
}

public class BlockCellData
{
    public bool isEmptyBlock = true;
    public int x;
    public int y;
    public Vector2 dir;

    public BlockCellData(int x, int y, Vector2 dir)
    {
        this.x = x;
        this.y = y;
        this.dir = dir;
    }

    public Vector2Int gridindex => new Vector2Int(x, y);

    public BlockCellData GetNeighbor(Vector2Int dir, BlockCellData[,] cells, int w, int h)
    {
        Vector2Int next = gridindex + dir;
        if (next.x < 0 || next.x >= w || next.y < 0 || next.y >= h)
            return null;
        return cells[next.x, next.y];
    }

    public bool IsCollisionWith(BlockCellData other)
    {
        if (gridindex.y == other.gridindex.y)
            return (dir == Vector2.right && other.dir == Vector2.left && other.gridindex.x > gridindex.x)
                || (dir == Vector2.left && other.dir == Vector2.right && other.gridindex.x < gridindex.x);

        if (gridindex.x == other.gridindex.x)
            return (dir == Vector2.up && other.dir == Vector2.down && other.gridindex.y > gridindex.y)
                || (dir == Vector2.down && other.dir == Vector2.up && other.gridindex.y < gridindex.y);

        return false;
    }

}