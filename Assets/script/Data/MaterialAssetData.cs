using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MaterialAssetData", menuName = "Data/MaterialAssetData")]
public class MaterialAssetData : ScriptableObject
{
    public enum MaterialType {mat_up = 0, mat_right = 1, mat_left = 2, mat_down = 3, mat_lock = 4 }
    // up = 0, right = 1, left = 2, down = 3 
    [SerializeField] private Material[] _materials;
    public Material GetMaterial(MaterialType mattype) =>_materials[(int)mattype];

}
