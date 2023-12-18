using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int id;
    public string pos;
}

[System.Serializable]
public class PlayerDetails
{
    public int life;
    public float speed;
    public int nbClassicBomb;
    public int nbMine;
    public int nbRemoteBomb;
    public int impactDist;
    public bool invincible;
}

[System.Serializable]
public class GameData
{
    public string action;
    public string statut;
    public string message;
    public int nbPlayers;
    public PlayerData[] players;
    public string startPos;
    public PlayerDetails player;
}