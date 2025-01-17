// Receive data form a UDP connection and store it in a string reference

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(StringReference))]
public class UDPReceiver : MonoBehaviour
{
    Thread receiveThread;
    UdpClient client;
    public int port = 5052;
    public bool startReceiving = true;
    public bool printToConsole = false;
    public StringReference data;

    bool isConnecting = false;

    void Start()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    public void ToggleConnection(){
        if(isConnecting){
            if(receiveThread != null)
                receiveThread.Abort();
            if(client != null){
                client.Close();
            }
        }
        else {
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        isConnecting = !isConnecting;
    }

    private void ReceiveData()
    {
        print("pre udp");
        client = new UdpClient(port);
        print("post udp");

        while (startReceiving)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataByte = client.Receive(ref anyIP);
                data.value = Encoding.UTF8.GetString(dataByte);

                if (printToConsole)
                    print(data);
            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }
    }

    void OnSceneUnloaded(Scene scene)
    {
        isConnecting = false;
        if(receiveThread != null)
            receiveThread.Abort();
        if(client != null){
            client.Close();
        }
    }

    void OnDisable() {
        isConnecting = false;
        if(receiveThread != null)
            receiveThread.Abort();
        if(client != null){
            client.Close();
        }
    }
}
