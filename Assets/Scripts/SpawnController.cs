using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Malee;

public class SpawnController : MonoBehaviour
{
    public enum Arrows {Arrow, Antiarrow, Inverse, Charging, Smart, Orbital}
    public enum Positions {Right, Left, Up, Down, Down_Right, Down_Left, Up_Right, Up_Left}

    [Header("Attak Editor")]
    [SerializeField]
    [Reorderable(paginate = true, pageSize = 0)]
    public Attacks attacks;

    #if UNITY_EDITOR
    [Header("Json Values")]
    public string filePath;
    string jsonString;
    #endif

    [System.Serializable]
    public class Spawn
    {
        public Arrows arrow;
        public Positions position;
        public float spawnTime = 1;
        public float speed = 2;
        public float actionDistance = 1.5f;
    }

    [System.Serializable]
    public class Spawns
    {
        [Reorderable]
        public SpawnList SpawnList;
    }

    #if UNITY_EDITOR
    public void WriteJson ()
    {
        jsonString = JsonUtility.ToJson(attacks);
        File.WriteAllText(filePath, jsonString);
    }

    void Start ()
    {
        jsonString = File.ReadAllText(filePath);
        if (jsonString != "")
        {
            attacks = JsonUtility.FromJson<Attacks>(jsonString);
        }
    }
    #endif

    [System.Serializable]
    public class SpawnList : ReorderableArray<Spawn> {}
    [System.Serializable]
    public class Attacks : ReorderableArray<Spawns> {}
}