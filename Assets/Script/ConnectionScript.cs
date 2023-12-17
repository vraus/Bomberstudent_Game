using System;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectionScript : MonoBehaviour
{
    private TcpClient tcpClient;
    private int port = 42069;
    private StreamReader reader;
    private NetworkStream networkStream;
    private string jsonResponse;
    private MapList mapList;
    private GameList gameList;

    [SerializeField] Button connect;
    [SerializeField] Button map;
    [SerializeField] Button game;
    [SerializeField] GameObject panel;


    void Start() {
        panel.gameObject.SetActive(false);
        map.gameObject.SetActive(false);
        game.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (reader != null)
        {
            reader.Close();
        }
        if (networkStream != null)
        {
            networkStream.Close();
        }
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
    }

    public void OnConnect()
    {
        try
        {
            tcpClient = new TcpClient("127.0.0.1", port);
            networkStream = tcpClient.GetStream();

            Debug.Log("Connected to the server!");
            connect.gameObject.SetActive(false);
            map.gameObject.SetActive(true);
            game.gameObject.SetActive(true);
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
                    if (reader != null){
                        jsonResponse = null;
                        while (reader.Peek() > -1) {
                            jsonResponse += reader.ReadLine() + '\n';
                        }
                        reader = null;
                        map.gameObject.SetActive(false);

                        if (!string.IsNullOrEmpty(jsonResponse))
                        {
                            Debug.Log(jsonResponse.ToString());
                            
                            mapList = JsonUtility.FromJson<MapList>(jsonResponse); // access the data in 'mapList' object
                            if (mapList != null && mapList.maps != null) {
                                foreach (Map map in mapList.maps){
                                    Debug.Log($"Map ID: {map.id}, Width: {map.width}, Height: {map.height}, Content: {map.content}");
                                }
                            }
                            else{
                                Debug.LogError("Failed to deserialize JSON into MapList object.");
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("NetworkStream is not initialized.");
            } 
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
                    if (reader != null){
                        jsonResponse = null;

                        while (reader.Peek() > -1) {
                            jsonResponse += reader.ReadLine() + '\n';
                        }
                        reader = null;
                        if (!string.IsNullOrEmpty(jsonResponse))
                        {
                            Debug.Log(jsonResponse.ToString());
                            
                            gameList = JsonUtility.FromJson<GameList>(jsonResponse); // access the data in 'gameList' object
                            if (gameList != null && gameList.games != null) {
                                foreach (Game game in gameList.games){
                                    Debug.Log($"name: {game.name}, Nb Player:{game.nbPlayer}, Map ID: {game.mapId}");
                                }
                            }
                            else{
                                Debug.LogError("Failed to deserialize JSON into MapList object.");
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("NetworkStream is not initialized.");
            } 
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending message!" + e.Message);
        }
    }

 

    public NetworkStream GetNetworkStream()
    {
        return networkStream;
    }
}
