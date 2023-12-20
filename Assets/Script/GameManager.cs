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

    [Header("Wall Infos")]
    [SerializeField] GameObject unbreakableWallPrefab;
    [SerializeField] GameObject breakableWallPrefab;
    [SerializeField] GameObject target;
    Vector3 firstItemPos = new(-3.5f, 0.5f, 3.5f);

    private void GetMapContent() => map_content = connectionScript.GetMapList().maps[mapId].content;

    private void InstantiateMap()
    {
        foreach(char c in map_content) Debug.Log(c);
        int k = 0;
        for(int i = 0; i < 8; i++)
        {
            Vector3 zOffset = new(0, 0, -i);
            for (int j = 0; j < 8; j++)
            {
                Vector3 xOffset = new(j, 0, 0);
                switch (map_content[k])
                {
                    case '*':
                        GameObject unbreakWall = Instantiate(unbreakableWallPrefab, firstItemPos + xOffset + zOffset, Quaternion.identity);
                        Debug.Log("Content: " + map_content[k]);
                        break;
                    case '=':
                        GameObject breakWall = Instantiate(breakableWallPrefab, firstItemPos + xOffset + zOffset, Quaternion.identity);
                        Debug.Log("Content: " + map_content[k]);
                        break;
                    default:
                        break;
                }
                k++;
            }
        }
    }

    public void GameInitialization(int mapId)
    {
        Instantiate(mapPrefab);
        GetMapContent();
        InstantiateMap();
    }
}
