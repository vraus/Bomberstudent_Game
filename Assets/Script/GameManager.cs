using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Map Info")]
    [SerializeField] GameObject mapPrefab;
    [SerializeField] ConnectionScript connectionScript;
    private string map_content;
    int mapId;

    private void GetMapContent() => map_content = connectionScript.GetMapList().maps[mapId].content;

    private void InstantiateMap()
    {
        foreach(char info in map_content) Debug.Log("New entry: " + info);
    }

    public void GameInitialization(int mapId)
    {
        Instantiate(mapPrefab);
        GetMapContent();
        InstantiateMap();
    }
}
