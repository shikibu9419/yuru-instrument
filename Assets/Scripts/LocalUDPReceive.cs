using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine.UI;

public class LocalUDPReceive : MonoBehaviour
{
    static string localIpString = "127.0.0.1";
    static IPAddress localAddress = IPAddress.Parse(localIpString);

    static int localPort = 8887;
    IPEndPoint localEP = new IPEndPoint(localAddress, localPort);
    static int unityPort = 8888;
    IPEndPoint unityEP = new IPEndPoint(localAddress, unityPort);

    static bool isReceiving;
    static UdpClient udpUnity;
    Thread thread;

    static string Rtext = "from";
    static public byte[] data = System.Text.Encoding.GetEncoding("utf-8").GetBytes("abcd");

    void Start()
    {
        udpUnity = new UdpClient(unityEP);
        udpUnity.Client.ReceiveTimeout = 2000;
        // udpUnity.Connect(localEP);
        isReceiving = true;
        thread = new Thread(new ThreadStart(ThreadMethod));
        thread.Start();
        Debug.Log("start");
    }

    void OnApplicationQuit()
    {
        isReceiving = false;
        if (thread != null) thread.Abort();
        if (udpUnity != null) udpUnity.Close();
    }

    private static void ThreadMethod()
    {
        while (isReceiving)
        {
            try
            {
                IPEndPoint remoteEP = null;
                byte[] data = udpUnity.Receive(ref remoteEP);
                Rtext = Encoding.UTF8.GetString(data);
                Debug.Log(Rtext);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }
}