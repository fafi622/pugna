using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
public class Network : MonoBehaviour {

    static Socket MainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    static Socket SendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    static Socket RecSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    static IPEndPoint MainAddress = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 25567);
    static IPEndPoint SendAddress = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 25569);
    static IPEndPoint RecAddress = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 25568);

    public List<GameObject> Players = new List<GameObject>();
    static int ID;
    byte[] Recbuff = new byte[1024];

    Vector3 lastPos = new Vector3(0, 0, 0);
    bool SWITCH = false;

    [SerializeField]
    GameObject PlayerPrefab;
    GameObject MainPlayer;
    // Use this for initialization
    void Start () {
        
        MainSocket.Connect(MainAddress);
        SendSocket.Connect(SendAddress);
        RecSocket.Connect(RecAddress);
        byte[] sendbuff = new byte[1024];
        sendbuff = Encoding.Unicode.GetBytes("0|fafy622");
        SendSocket.Send(sendbuff);
        Debug.Log("Connected");
        MainSocket.BeginReceive(Recbuff, 0, Recbuff.Length, SocketFlags.None, OnRec, MainSocket);
        MainPlayer = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        lastPos = MainPlayer.transform.position;
    }
    Vector3 temp = new Vector3(0, 0, 0);

    bool Switch = false;
	// Update is called once per frame
	void Update () {
        if(Switch == true)
        {

        }
        if (Vector3.Distance(lastPos,MainPlayer.transform.position) >= 1)
        {
            byte[] sendbuff = new byte[1024];
            sendbuff = Encoding.Unicode.GetBytes("2|" + ID + "|" + transform.position.x + "$" + transform.position.y + "$" + transform.position.z);
            SendSocket.Send(sendbuff);
            lastPos = MainPlayer.transform.position;
        }
        if(SWITCH == true)
        {
            transform.position = temp;
            SWITCH = false;
            
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            byte[] sendbuff = new byte[1024];
            sendbuff = Encoding.Unicode.GetBytes("1|" + ID);
            SendSocket.Send(sendbuff);
        }
	}
    IEnumerator SpawnPlayer(int id)
    {
        GameObject p = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        p.GetComponent<TEMP>().ID = id;
        Players.Add(p);

        yield return null;
    }
    public void OnRec(IAsyncResult ar)
    {
        Debug.Log("Message recced");
        string a = Encoding.Unicode.GetString(Recbuff).TrimEnd('\0');
        string[] message = a.Split('|');
        Debug.Log(message[1].Length);
        switch (message[0])
        {
            case "0":
                ID = Int32.Parse(message[1]);
                Array.Clear(Recbuff, 0, Recbuff.Length);
                MainSocket.BeginReceive(Recbuff, 0, Recbuff.Length, SocketFlags.None, OnRec, RecSocket);
                Debug.Log("Reached end");
                break;

            case "1":
                Debug.Log("Code 1 used");
               
                temp = new Vector3(float.Parse(message[1]), float.Parse(message[2]), float.Parse(message[3]));
                SWITCH = true;
                Array.Clear(Recbuff, 0, Recbuff.Length);
                MainSocket.BeginReceive(Recbuff, 0, Recbuff.Length, SocketFlags.None, OnRec, RecSocket);
                Debug.Log("Reached end");
                break;
            case "2":
                Debug.Log("Code 2 used");
                StartCoroutine("SpawnPlayer", Int32.Parse(message[1]));
                Array.Clear(Recbuff, 0, Recbuff.Length);
                MainSocket.BeginReceive(Recbuff, 0, Recbuff.Length, SocketFlags.None, OnRec, RecSocket);
                Debug.Log("Reached end");
                break;
        }
      
        
    }
}
