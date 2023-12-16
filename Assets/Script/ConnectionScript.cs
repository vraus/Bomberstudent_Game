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

    [SerializeField] Button connect;
    [SerializeField] Button map;
    [SerializeField] Button game;

    void Start() {
        map.gameObject.SetActive(false);
        game.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
        if (networkStream != null)
        {
            networkStream.Close();
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
                string mapRequest = "GET maps/list" + '\n';
                byte[] data = System.Text.Encoding.UTF8.GetBytes(mapRequest);
                networkStream.Write(data, 0, data.Length);  

                System.Threading.Thread.Sleep(100); //sleep to wait the server answer
                if (networkStream != null && networkStream.DataAvailable) {
                    reader = new StreamReader(networkStream, System.Text.Encoding.UTF8);
                    if (reader != null){
                        while (reader.Peek() > -1) {
                            jsonResponse += reader.ReadLine();
                            jsonResponse += "\n";
                        }
                        if (!string.IsNullOrEmpty(jsonResponse))
                        {
                            Debug.Log(jsonResponse.ToString());
                            reader = null;
                            
                            mapList = JsonUtility.FromJson<MapList>(jsonResponse); // access the data in 'mapList' object
                            if (mapList != null) {
                                Debug.Log(mapList.maps);
                                foreach (Map map in mapList.maps) //PROBLEME mapList.maps est Null donc bug
                                {
                                    Debug.Log("non");
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



    public NetworkStream GetNetworkStream()
    {
        return networkStream;
    }
}
