using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Game
{
    public string name;
    public int nbPlayer;
    public int mapId;
}

[System.Serializable]
public class GameList
{
    public string action;
    public string statut;
    public string message;
    public int nbGamesList;
    public Game[] games;
}
