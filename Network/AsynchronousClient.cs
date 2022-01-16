using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;  
using System.Net.Sockets;  
using System.Threading;  
using System.Text;
using TMPro;
using UnityEngine;

public class AsynchronousClient : MonoBehaviour
{
    private static bool ManagedConnection;
    public Coroutine coroutine;
    public TMP_Text testText;
    private Thread ClientConnection;
    private char[] Seperators = new char[1];
    private List<string> LeaderboardList = null;
    private void Awake()
    {
        Seperators[0] = ',';
        ManagedConnection = false;
        GetLeaderboards();
    }

    public void GetLeaderboards()
    {
        coroutine = StartCoroutine("TryGetLeaderboard");
    }
    
    IEnumerator TryGetLeaderboard()
    {
        ManagedConnection = false;
        LeaderboardList = null;
        while (!ManagedConnection)
        {
            ClientConnection = new Thread(GetLeaderboard);
            ClientConnection.Start();
            yield return new WaitForSeconds(5f);
        }
        while (LeaderboardList == null)
        {
            yield return new WaitForSeconds(5f);
        }
        yield return LeaderboardList;
    }
    
   private void GetLeaderboard()
    {
        byte[] bytes = new byte[1024];
        try
        {
            // Connect to a Remote server
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            //IPHostEntry host = Dns.GetHostEntry("93.172.240.232");
            IPAddress ipAddress = IPAddress.Parse("10.100.102.6");
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("10.100.102.6"), 11000);

            // Create a TCP/IP  socket.
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                // Connect to Remote EndPoint
                sender.Connect(remoteEP);
                print("Socket connected to "+sender.RemoteEndPoint.ToString());
                ManagedConnection = true;
                // Encode the data string into a byte array.
                byte[] msg = Encoding.ASCII.GetBytes("Leaderboard<EOF>");

                // Send the data through the socket.
                int bytesSent = sender.Send(msg);
            
                // Receive the response from the remote device.
                string data = null;
                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRec = sender.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }
                print("Text received :"+ data);
                testText.text = data;
                List<string> UsersAndScores = data.Split(',').ToList();
                // Release the socket.
                // Encode the data string into a byte array.
                msg = Encoding.ASCII.GetBytes("Bye have a great time<EOF>");

                // Send the data through the socket.
                sender.Send(msg);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                LeaderboardList = UsersAndScores;
                int counter = 0;
                LeaderboardList.ForEach(x =>
                {
                    print(counter +" -> "+x);
                    counter++;
                });
            }
            catch (ArgumentNullException ane)
            {
                print("ArgumentNullException : "+ ane.ToString());
                testText.text = "ArgumentNullException : "+ ane.ToString();
            }
            catch (SocketException se)
            {
                print("SocketException : "+ se.ToString());
                testText.text = "SocketException : "+ se.ToString();
            }
            catch (Exception e)
            {
                print("Unexpected exception : "+ e.ToString());
                testText.text = "Unexpected exception  : "+ e.ToString();
            }
        }
        catch (Exception e)
        {
            testText.text = "Unexpected exception  : "+ e.ToString();
        }
    }
   
   public void updateScore()
   {
       coroutine = StartCoroutine("TryPostScore");
   }
    
   IEnumerator TryPostScore()
   {
       ManagedConnection = false;
       LeaderboardList = null;
       while (!ManagedConnection)
       {
           ClientConnection = new Thread(PostScore);
           ClientConnection.Start();
           yield return new WaitForSeconds(5f);
       }
       while (LeaderboardList == null)
       {
           yield return new WaitForSeconds(5f);
       }
       yield return LeaderboardList;
   }
   
   private void PostScore()
    {
        byte[] bytes = new byte[1024];
        try
        {
            // Connect to a Remote server
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            IPHostEntry host = Dns.GetHostEntry("93.172.240.232");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP  socket.
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                // Connect to Remote EndPoint
                sender.Connect(remoteEP);
                print("Socket connected to "+sender.RemoteEndPoint.ToString());
                ManagedConnection = true;
                // Encode the data string into a byte array.
                byte[] msg = Encoding.ASCII.GetBytes("Update - "+PlayerPrefs.GetString("Username") +" , "+ Player.GetScore() + "<EOF>");

                // Send the data through the socket.
                int bytesSent = sender.Send(msg);
            
                // Receive the response from the remote device.
                string data = null;
                while (true)
                {
                    bytes = new byte[1024];
                    int bytesRec = sender.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        break;
                    }
                }
                print("Text received :"+ data);
                // Release the socket.
                // Encode the data string into a byte array.
                msg = Encoding.ASCII.GetBytes("Bye have a great time<EOF>");

                // Send the data through the socket.
                sender.Send(msg);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane)
            {
                print("ArgumentNullException : "+ ane.ToString());
            }
            catch (SocketException se)
            {
                print("SocketException : "+ se.ToString());
            }
            catch (Exception e)
            {
                print("Unexpected exception : "+ e.ToString());
            }
        }
        catch (Exception e)
        {
            print(e.ToString());
        }
    }
}  