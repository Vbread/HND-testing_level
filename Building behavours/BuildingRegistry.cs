using UnityEngine;


[CreateAssetMenu(fileName = "BuildingRegisty", menuName = "Scriptable Objects/BuildingRegisty")]
public class BuildingRegisty : ScriptableObject
{
    [System.Serializable]
    public struct BuildableObjectsRegisty
    {
        public string Name;
        public Vector3 Size;
        public GameObject Prefab;
    }

    public BuildableObjectsRegisty[] BuildableObjectsRegistry;

}
