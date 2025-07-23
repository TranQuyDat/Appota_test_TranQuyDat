using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelLoader :MonoBehaviour
{
    public CameraController _cameraCtrl;
    public TextAsset _levelJson;
    public int levelId;
    public GameObject _blockPrefab;
    public GameObject _sawBaldePrefab;
    public Transform _content;
    private TextAsset[] _levelJsons;
    private void Start()
    {
        LoadLevel(levelId); // demo
    }
    public void Init(TextAsset[] levelJsons)
    {
        _levelJsons = levelJsons;
    }
    public LevelData LoadLevel(int levelId)
    {
        string json ;
        if (_levelJsons != null)
            json = _levelJsons[levelId].text;
        else json = _levelJson.text;
            LevelData levelData = new LevelData();
        levelData.FromJson(json);
        if (levelData.levelId != levelId)
        {
            Debug.LogError("Level ID mismatch");
            return null;
        }
        // Create grid
        Grid grid = new Grid(levelData.Width, levelData.Height); 
        grid.CreateGrid(Vector3.zero, 1f);
        _cameraCtrl.AdjustCameraSize(grid.Width, grid.Height);
        //create saw blades
        foreach (var sawBlade in levelData.sawBlades)
        {
            Node node = grid.GetNode((int)sawBlade.grid.x, (int)sawBlade.grid.y);
            if (node == null) continue;
            GameObject sawBladeObj = ObjectPooling.GetPool("SawBlade", _sawBaldePrefab);
            sawBladeObj.transform.SetParent(_content);
            sawBladeObj.transform.position = node.Pos;
        }
        // create blocks
        foreach (var block in levelData.blocks)
        {
            Node node = grid.GetNode((int)block.grid.x, (int)block.grid.y);
            if (node == null) continue;

            GameObject blockObj = ObjectPooling.GetPool("Block",_blockPrefab);
            blockObj.transform.SetParent(_content);
            blockObj.transform.position = node.Pos;
            Block blockCtrl = blockObj.GetComponent<Block>();
            blockCtrl.Init(block);
        }

        return levelData;
    }
}
