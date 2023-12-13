using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectionScript : MonoBehaviour
{
    private TcpClient tcpClient;
    private int port = 42069;


    private NetworkStream networkStream;

    [SerializeField] Button connect;

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
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to the server!" + e.Message);
        }
    }

    public NetworkStream GetNetworkStream()
    {
        return networkStream;
    }
}
