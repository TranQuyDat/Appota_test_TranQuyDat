using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class LevelLoader :MonoBehaviour
{
    public CameraController _cameraCtrl;
    public GameObject _blockPrefab;
    public GameObject _sawBaldePrefab;
    public Transform _content;
    public LevelData[] _levelDatas;
    public void Init(LevelData[] levelDatas)    
    {
        _levelDatas = levelDatas;
    }

    public async UniTask<LevelData> LoadLevel(int levelId)
    {
        LevelData levelData = _levelDatas[levelId - 1];
        if (levelData.levelId != levelId)
        {
            Debug.LogError("Level ID mismatch");
            return null;
        }
        // Create grid
        Grid grid = new Grid(levelData.Width, levelData.Height, new Vector2(0.9f, 0.72f)); 
        grid.CreateGrid(Vector3.zero);
        _cameraCtrl.AdjustCameraSize(grid.Width, grid.Height);
        await UniTask.DelayFrame(1);

        int couter = 0;

        //create saw blades
        foreach (var sawBlade in levelData.sawBlades)
        {
            Node node = grid.GetNode((int)sawBlade.grid.x, (int)sawBlade.grid.y);
            if (node == null) continue;
            GameObject sawBladeObj = ObjectPooling.GetPool("SawBlade", _sawBaldePrefab);
            sawBladeObj.transform.SetParent(_content);
            sawBladeObj.transform.position = node.Pos;
            sawBladeObj.SetActive(true);
            couter++;
        }

        await UniTask.DelayFrame(1);

        // create blocks]
        foreach (var block in levelData.blocks)
        {
            Node node = grid.GetNode((int)block.grid.x, (int)block.grid.y);
            if (node == null) continue;
            GameObject blockObj = ObjectPooling.GetPool("Block",_blockPrefab);
            blockObj.transform.SetParent(_content);
            blockObj.transform.position = node.Pos;

            Block b = blockObj.GetComponent<Block>();
            b.Init(block,grid.NodeSize);
            b.gameObject.SetActive(true);

        }

        return levelData;
    }
}
