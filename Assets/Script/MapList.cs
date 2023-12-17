using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Map
{
    public int id;
    public int width;
    public int height;
    public string content;
}

[System.Serializable]
public class MapList
{
    public string action;
    public string statut;
    public string message;
    public int nbMapsList;
    public Map[] maps;
}
