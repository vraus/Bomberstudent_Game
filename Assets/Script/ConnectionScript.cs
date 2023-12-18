using System;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;

public class ConnectionScript : MonoBehaviour
{
    private TcpClient tcpClient;
    private int port = 42069;
    private StreamReader reader;
    private NetworkStream networkStream;
    private string jsonResponse;
    private MapList mapList;
    private GameList gameList;
    private GameData gameData;

    [SerializeField] Button connect;


    [Header("List Elements")]
    public GameObject entryPrefab;
    [SerializeField] GameObject pnlMapList;
    [SerializeField] GameObject pnlGamesList;
    [SerializeField] GameObject pnlJoin;
    [SerializeField] GameObject pnlCreate;


    void Start() {
        pnlMapList.gameObject.SetActive(false);
        pnlGamesList.gameObject.SetActive(false);
        pnlJoin.gameObject.SetActive(false);
        pnlCreate.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (reader != null) reader.Close();
        if (networkStream != null) networkStream.Close();
        if (tcpClient != null) tcpClient.Close();
    }

    public void OnConnect()
    {
        try
        {
            tcpClient = new TcpClient("127.0.0.1", port);
            networkStream = tcpClient.GetStream();

            Debug.Log("Connected to the server!");
            connect.gameObject.SetActive(false);
            pnlMapList.gameObject.SetActive(false);
            pnlGamesList.gameObject.SetActive(false);
            pnlJoin.gameObject.SetActive(false);
            pnlCreate.gameObject.SetActive(false);

            OnMap();
            OnGame();
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to the server!" + e.Message);
        }
    }


    public void OnMap() {
        try
        {
            if (networkStream != null)
            {
                //Send the request to the server
                string mapRequest = "GET maps/list\n";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(mapRequest);
                networkStream.Write(data, 0, data.Length);

                System.Threading.Thread.Sleep(100); //sleep to wait the server answer
                if (networkStream != null && networkStream.DataAvailable) {
                    reader = new StreamReader(networkStream, System.Text.Encoding.UTF8);
                    if (reader != null) {
                        jsonResponse = null;
                        while (reader.Peek() > -1)
                        {
                            jsonResponse += reader.ReadLine() + '\n';
                        }
                        reader = null;
                        pnlMapList.gameObject.SetActive(true);
                        if (!string.IsNullOrEmpty(jsonResponse))
                        {
                            Debug.Log(jsonResponse.ToString());

                            mapList = JsonUtility.FromJson<MapList>(jsonResponse); // access the data in 'mapList' object
                            if (mapList != null && mapList.maps != null) {
                                foreach (Map map in mapList.maps) {
                                    Debug.Log($"Map ID: {map.id}, Width: {map.width}, Height: {map.height}, Content: {map.content}");
                                    GameObject entry = Instantiate(entryPrefab, pnlMapList.transform);
                                    TextMeshProUGUI nameEntry = entry.transform.Find("EntryName").GetComponent<TextMeshProUGUI>();
                                    nameEntry.text = "Id :" + map.id;
                                    TextMeshProUGUI nbPlayer = entry.transform.Find("EntryNbPlayers").GetComponent<TextMeshProUGUI>();
                                    nbPlayer.text = "";

                                    Button button = entry.GetComponentInChildren<Button>();
                                    button.GetComponentInChildren<TextMeshProUGUI>().GetComponent<TextMeshProUGUI>().text = "Create from map";
                                    button.onClick.AddListener(() => OnButtonClick(false, "", map.id));
                                }
                            }
                            else {
                                Debug.LogError("Failed to deserialize JSON into MapList object.");
                            }
                        }
                    }
                }
            }
            else
                Debug.LogError("NetworkStream is not initialized.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending message!" + e.Message);
        }
    }


    public void OnGame() {
        try
        {
            if (networkStream != null)
            {
                //Send the request to the server
                string mapRequest = "GET game/list\n";
                byte[] data = System.Text.Encoding.UTF8.GetBytes(mapRequest);
                networkStream.Write(data, 0, data.Length);

                System.Threading.Thread.Sleep(100); //sleep to wait the server answer
                if (networkStream != null && networkStream.DataAvailable) {
                    reader = new StreamReader(networkStream, System.Text.Encoding.UTF8);
                    if (reader != null) {
                        jsonResponse = null;
                        pnlGamesList.gameObject.SetActive(true);

                        while (reader.Peek() > -1)
                        {
                            jsonResponse += reader.ReadLine() + '\n';
                        }
                        reader = null;
                        if (!string.IsNullOrEmpty(jsonResponse))
                        {
                            Debug.Log(jsonResponse.ToString());


                            gameList = JsonUtility.FromJson<GameList>(jsonResponse); // access the data in 'gameList' object
                            if (gameList != null && gameList.games != null) {
                                foreach (Game game in gameList.games) {
                                    Debug.Log($"name: {game.name}, Nb Player:{game.nbPlayer}, Map ID: {game.mapId}");
                                    GameObject entry = Instantiate(entryPrefab, pnlGamesList.transform);
                                    TextMeshProUGUI nameEntry = entry.transform.Find("EntryName").GetComponent<TextMeshProUGUI>();
                                    nameEntry.text = "Name: " + game.name;
                                    TextMeshProUGUI nbPlayer = entry.transform.Find("EntryNbPlayers").GetComponent<TextMeshProUGUI>();
                                    nbPlayer.text = game.nbPlayer + "/4";
                                    Button button = entry.GetComponentInChildren<Button>();
                                    button.GetComponentInChildren<TextMeshProUGUI>().GetComponent<TextMeshProUGUI>().text = "Join";
                                    button.onClick.AddListener(() => OnButtonClick(true, game.name, game.mapId));
                                }
                            }
                            else {
                                Debug.LogError("Failed to deserialize JSON into MapList object.");
                            }
                        }
                    }
                }
            }
            else
                Debug.LogError("NetworkStream is not initialized.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending message!" + e.Message);
        }
    }

    public void OnButtonClick(bool isJoin, string gameName, int mapId)
    {
        if(isJoin)
        {
            Debug.Log(isJoin);
        }
        else
        {
            Debug.Log(mapId);
            pnlCreate.SetActive(true);

            TMP_InputField nameInput = pnlCreate.GetComponentInChildren<TMP_InputField>();
            nameInput.text = "Game " + mapId.ToString();

            TextMeshProUGUI mapIdTxt = pnlCreate.transform.Find("MapId").GetComponent<TextMeshProUGUI>();
            mapIdTxt.text = "Map Id: " + mapId.ToString();

            Button btnCreate = pnlCreate.GetComponentInChildren<Button>();
            btnCreate.onClick.AddListener(() => OnButtonClickCreate(nameInput.text, mapId));
        }
    }
    

    public void OnButtonClickCreate(string gameName, int mapId)
    {
        try
        {
            if (networkStream != null)
            {
                Debug.Log(gameName + " " + mapId);
                string gameCreateRequest = "POST game/create {\"name\":\"" + gameName + "\", \"mapId\":" + mapId + "}";
                Debug.Log(gameCreateRequest);
                byte[] data = System.Text.Encoding.UTF8.GetBytes(gameCreateRequest);
                networkStream.Write(data, 0, data.Length);

                pnlCreate.SetActive(false);
                pnlJoin.SetActive(true);
                System.Threading.Thread.Sleep(100); //sleep to wait the server answer
                if (networkStream != null && networkStream.DataAvailable) {
                    reader = new StreamReader(networkStream, System.Text.Encoding.UTF8);
                    if (reader != null) {
                        jsonResponse = null;
                        while (reader.Peek() > -1)
                        {
                            jsonResponse += reader.ReadLine() + '\n';
                        }
                        reader = null;
                        if (!string.IsNullOrEmpty(jsonResponse))
                        {
                            Debug.Log(jsonResponse.ToString());

                            gameData = JsonUtility.FromJson<GameData>(jsonResponse); // access the data in 'mapList' object
                            if (gameData != null && gameData.players != null && gameData.player != null) {
                                foreach (PlayerData playerData in gameData.players) {
                                    Debug.Log($"ID: {playerData.id}, pos: {playerData.pos}");
                                }
                                Debug.Log($"life: {gameData.player.life}, speed: {gameData.player.speed}, nbClassicBomb: {gameData.player.nbClassicBomb}, nbMine : {gameData.player.nbMine}, nbRemoteBomb: {gameData.player.nbRemoteBomb}, impactDist: {gameData.player.impactDist}, invincible: {gameData.player.invincible}");
                            }
                            else {
                                Debug.LogError("Failed to deserialize JSON into MapList object.");
                            }
                        }
                    }
                }
                OnGame();
            }
            else
                Debug.LogError("NetworkStream is not initialized.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending message!" + e.Message);
        }
    }



    public NetworkStream GetNetworkStream() { return networkStream; }
}

