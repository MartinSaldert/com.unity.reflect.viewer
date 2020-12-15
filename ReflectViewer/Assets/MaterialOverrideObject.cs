using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Material Override", order = 1)]
public class MaterialOverrideObject : ScriptableObject
{
    [SerializeField]
    public List<MaterialOverride> materialOverrides = new List<MaterialOverride>();
}

[Serializable]
public struct MaterialOverride
{
    public string property;
    public Material material;
}