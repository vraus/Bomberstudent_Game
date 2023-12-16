using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public int id;
    public int width;
    public int height;
    public string content;
}

public class MapList
{
    public string action;
    public string statut;
    public string message;
    public int nbMapsList;
    public Map[] maps;
}
