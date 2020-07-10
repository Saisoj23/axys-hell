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
    [Reorderable(paginate = true)]
    public Attacks attacks;

    [Header("Json Values")]
    public string filePath;
    string jsonString;

    [System.Serializable]
    public class Spawn
    {
        public Arrows arrow;
        public Positions position;
        public float spawnTime = 1;
        public float speed = 2;
        public float actionDistance = 1.5f;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [System.Serializable]
    public class Spawns
    {
        [Reorderable]
        public SpawnList SpawnList;
    }

    public void ReadJson()
    {
        jsonString = File.ReadAllText(filePath);
        if (jsonString != "")
        {
            attacks = JsonUtility.FromJson<Attacks>(jsonString);
        }
    }

    public void WriteJson ()
    {
        jsonString = JsonUtility.ToJson(attacks);
        File.WriteAllText(filePath, jsonString);
    }

    [System.Serializable]
    public class SpawnList : ReorderableArray<Spawn> {}
    [System.Serializable]
    public class Attacks : ReorderableArray<Spawns> {}
}