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
    public TMP_Text testText;
    public State Indicator;
    private static bool ManagedConnection;
    private Thread ClientConnection;
    private List<string> LeaderboardList = null;
    private string usernameToCheck;
    private string PlayerUsername;
    private string PlayerScore;
    private void Awake()
    {
        ManagedConnection = false;
    }

    #region Add new Username

    public void AddUserStart()
    {
        ManagedConnection = false;
        PlayerUsername = PlayerPrefs.GetString("Username");
        StartCoroutine("TryAddUser");
    }
    
    IEnumerator TryAddUser()
    {
        while (!ManagedConnection)
        {
            ClientConnection = new Thread(AddUserProcess);
            ClientConnection.Start();
            yield return new WaitForSeconds(5f);
        }
    }
   
    private void AddUserProcess()
    {
        try
        {
            Socket Connection = GetConnectionToServer();
            try
            {
                // Send the data through the socket.
                SendMessage(Connection, "@@Insert@@"+PlayerUsername);
                // Release the socket.
                Connection.Shutdown(SocketShutdown.Both);
                Connection.Close();
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

    #endregion
    
    #region Get Leaderboards 

    public void GetLeaderboardStart()
    {
        ManagedConnection = false;
        StartCoroutine("TryGetLeaderboard");
    }
    
    IEnumerator TryGetLeaderboard()
    {
        LeaderboardList = null;
        while (!ManagedConnection)
        {
            ClientConnection = new Thread(GetLeaderboardProcess);
            ClientConnection.Start();
            yield return new WaitForSeconds(5f);
        }
    }
    
    private void GetLeaderboardProcess()
    {
        try
        {
            Socket Connection = GetConnectionToServer();
            try
            {
                // Send the data through the socket.
                SendMessage(Connection, "@@Leaderboard@@"+usernameToCheck);
                // Receive the response from the remote device.
                string data = ReceiveMessage(Connection);
                print("Leaderboards Received");
                List<string> UsersAndScores = data.Split('\n').ToList();
                UsersAndScores.Remove("{");
                UsersAndScores.Remove("}");
                // Release the socket.
                Connection.Shutdown(SocketShutdown.Both);
                Connection.Close();
                LeaderboardList = UsersAndScores;
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
   
    public bool CheckLeaderboardList()
    {
        return LeaderboardList != null;
    }
    public List<string> GetLeaderboardList()
    {
        return LeaderboardList;
    }

    #endregion

    #region Update Score

    public void PostScoreStart()
    {
        ManagedConnection = false;
        PlayerUsername = PlayerPrefs.GetString("Username");
        PlayerScore = Player.GetScore().ToString();
        StartCoroutine("TryPostScore");
    }
    
    IEnumerator TryPostScore()
    {
        while (!ManagedConnection)
        {
            ClientConnection = new Thread(PostScoreProcess);
            ClientConnection.Start();
            yield return new WaitForSeconds(5f);
        }
    }
   
    private void PostScoreProcess()
    {
        try
        {
            Socket Connection = GetConnectionToServer();
            try
            {
                // Send the data through the socket.
                SendMessage(Connection, "@@Update@@"+PlayerUsername+":"+PlayerScore);
                // Release the socket.
                Connection.Shutdown(SocketShutdown.Both);
                Connection.Close();
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

    #endregion
   
    #region Username Availability Check
   public void CheckUsernameStart(string UsernameToCheck)
   {
       Indicator = State.Waiting;
       usernameToCheck = UsernameToCheck;
       ManagedConnection = false;
       StartCoroutine("TryCheckUsername");
   }
    
   IEnumerator TryCheckUsername()
   {
       while (!ManagedConnection)
       {
           ClientConnection = new Thread(CheckUsernameProcess);
           ClientConnection.Start();
           yield return new WaitForSeconds(5f);
       }
   }
   
   private void CheckUsernameProcess()
   {
       try
       {
           Socket Connection = GetConnectionToServer();
           try
           {
               // Send the data through the socket.
               SendMessage(Connection, "@@CheckUsername@@"+usernameToCheck);
               // Receive the response from the remote device.
               string data = ReceiveMessage(Connection);
               print("Text received :"+ data);
               if (data.Contains("True"))
                   Indicator = State.True;
               else
                   Indicator = State.False;
               // Release the socket.
               Connection.Shutdown(SocketShutdown.Both);
               Connection.Close();
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
   

   #endregion

    #region Basic Functions

    private Socket GetConnectionToServer()
   {
      try
        {
            // Connect to a Remote server
            // Get Host IP Address that is used to establish a connection
            // In this case, we get one IP address of localhost that is IP : 127.0.0.1
            // If a host has multiple addresses, you will get a list of addresses
            //IPAddress ipAddress = IPAddress.Parse("10.100.102.6");
            IPHostEntry host = Dns.GetHostEntry("ec2-54-196-252-133.compute-1.amazonaws.com");
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 2020);
            // Create a TCP/IP  socket.
            Socket sender = new Socket(ipAddress.AddressFamily,SocketType.Stream, ProtocolType.Tcp);
            // Connect the socket to the remote endpoint. Catch any errors.
            try
            {
                // Connect to Remote EndPoint
                sender.Connect(remoteEP);
                print("Socket connected to "+sender.RemoteEndPoint.ToString());
                ManagedConnection = true;
                return sender;
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
      return null;
   }
   
   private void SendMessage(Socket handler, string Message)
   {
       print("Sending out - "+Message);
       byte[] msg = Encoding.ASCII.GetBytes(Message+"<EOF>");
       handler.Send(msg);
   }
        
   private string ReceiveMessage(Socket handler)
   {
       byte[] bytes = null;
       string data = "";
       while (true)
       {
           bytes = new byte[1024];
           int bytesRec = handler.Receive(bytes);
           data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
           if (data.IndexOf("<EOF>") > -1)
           {
               break;
           }
       }

       return data;
   }

   #endregion
  
    
   public bool CheckState()
   {
       return Indicator != State.Waiting;
   }

   public enum State
   {
       Waiting,
       True,
       False
   }
}  