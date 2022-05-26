using Firesplash.UnityAssets.SocketIO;
using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

[Serializable]
public class PlayerTransformInfo
{
    public Vector3 position;
    public Vector3 rotation;
    public string playerId;
}

//initial info to validate lobby creation 
[Serializable]
public class LobbyCreationInfo
{
    //used to verify NFT belongs to owner
    public string mintHash;

    //used to verify request owner
    public string ownerPublicKey;
    public string message;
    public string signedMessage;

    public LobbyCreationInfo(string mintHashToSet, string ownerPublicKeyToSet, string messageToSet, string signedMessageToSet)
    {
        mintHash = mintHashToSet;
        ownerPublicKey = ownerPublicKeyToSet;
        message = messageToSet;
        signedMessage = signedMessageToSet;
    }
}

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private SocketIOCommunicator socket;

    [SerializeField]
    private GameObject localPlayer;

    [SerializeField]
    private GameObject playerTemplate;

    [SerializeField]
    private GravityAttractor gravityAttractor;

    private Dictionary<string, GameObject> otherPlayers = new Dictionary<string, GameObject>();

    private static NetworkManager _instance;

    public static NetworkManager Instance { get { return _instance; } }

    private PlayerController controller;

    private PlayerTransformInfo playerTransformInfo = new PlayerTransformInfo();

    private bool hasSentIdleEvent = false;

    Coroutine TransformRoutine;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        //Set initial player info values
        controller = localPlayer.GetComponent<PlayerController>();

        try
        {
            //self connect
            socket.Instance.On("connect", (string data) => {
                Debug.Log("connect");
                playerTransformInfo.playerId = socket.Instance.SocketID;
                TransformRoutine = StartCoroutine(SendTransformInfo());
                //TODO: send authentication event with data
            });

            //self disconnect
            socket.Instance.On("disconnect", (string payload) => {

                StopCoroutine(TransformRoutine);
                Debug.Log("disconnect");
            });

            //receive other player info from server
            socket.Instance.On("PlayerTransformInfos", (string payload) =>
            {
                Debug.Log("Received player transform infos: " + payload);
                PlayerTransformInfo[] infos = JsonConvert.DeserializeObject<PlayerTransformInfo[]>(payload);
                for(int i = 0; i < infos.Length; i++)
                {
                    HandlePlayerTransformInfo(ref infos[i]);
                }
            });

            socket.Instance.On("PlayerTransformInfo", (string payload) =>
            {
                PlayerTransformInfo info = JsonConvert.DeserializeObject<PlayerTransformInfo>(payload);

                Debug.Log("Received player transform info: " + info.playerId);
                HandlePlayerTransformInfo(ref info);
            });

            socket.Instance.On("PlayerJump", (string payload) =>
            {
                GameObject otherPlayer;
                if (otherPlayers.TryGetValue(payload, out otherPlayer))
                {
                    NetworkedPlayerController controller = otherPlayer.GetComponent<NetworkedPlayerController>();
                    controller.SetShouldJump(true);
                }
            });

            //payload is disconnected player id
            socket.Instance.On("PlayerDisconnected", (string payload) =>
            {
                Debug.Log("Player Disconnected: " + payload);
                if(otherPlayers.ContainsKey(payload))
                {
                    otherPlayers.Remove(payload);
                }
                else
                {
                    Debug.LogError("Player disconnecting with Id that is not stored. THIS SHOULD NEVER HAPPEN!");
                }
            });

            socket.autoReconnect = true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        //StartCoroutine(Ping());
    }

    private IEnumerator SendTransformInfo()
    {
        while(socket.Instance.IsConnected())
        {
            playerTransformInfo.position = localPlayer.transform.position;
            playerTransformInfo.rotation = localPlayer.transform.eulerAngles;
            socket.Instance.Emit("PlayerTransformInfo", JsonUtility.ToJson(playerTransformInfo), false);
            yield return new WaitForSecondsRealtime(1);
        }

    }

    void HandlePlayerTransformInfo(ref PlayerTransformInfo info)
    {
        if (!otherPlayers.ContainsKey(info.playerId))
        {
            GameObject otherPlayer = Instantiate(playerTemplate);
            NetworkedPlayerController controller = otherPlayer.GetComponent<NetworkedPlayerController>();
            controller.SetPosition(info.position);
            controller.SetRotation(info.rotation);
            otherPlayers.Add(info.playerId, otherPlayer);
        }
        else
        {
            GameObject otherPlayer;
            if (otherPlayers.TryGetValue(info.playerId, out otherPlayer))
            {
                NetworkedPlayerController controller = otherPlayer.GetComponent<NetworkedPlayerController>();
                controller.SetPosition(info.position);
                controller.SetRotation(info.rotation);
            }
            else
            {
                Debug.Log("Could not get value that was added. THIS SHOULD NEVER HAPPEN");
            }
        }
    }

    public void Jump()
    {
        if(socket.Instance.IsConnected())
        {
            socket.Instance.Emit("PlayerJump");
        }
    }

    private IEnumerator Ping()
    {
        if(socket.Instance.IsConnected())
        {
            socket.Instance.Emit("Ping");
        }

        yield return new WaitForSecondsRealtime(30);
    }

    public void Connect()
    {

        Debug.Log("connecting to server");

        try
        {
            socket.Instance.Connect();
        } 
        catch (Exception e)
        {
            Debug.Log(e);
        }
    
    }

    void CreateLobby()
    {
        if(!socket.Instance.IsConnected())
        {
            Debug.Log("Trying to create a lobby without connection");
            return;
        }
        string mintHash = NFTController.Instance.GetMintHash();
        string ownerPublicKey = NFTController.Instance.GetOwnerPublicKey();
        string signedMessage = NFTController.Instance.GetSignedMessage();
        string message = NFTController.Instance.GetMessage();
        if(string.IsNullOrEmpty(mintHash) || string.IsNullOrEmpty(ownerPublicKey) || string.IsNullOrEmpty(signedMessage) || string.IsNullOrEmpty(message))
        {
            
            return;
        }
        LobbyCreationInfo info = new LobbyCreationInfo(mintHash, ownerPublicKey, message, signedMessage);
        //TODO: pass token id, owner public key, message text and signed message for verification
        socket.Instance.Emit("CreateLobby", JsonUtility.ToJson(info), false);

    }

    public void Disconnect()
    {
        if (socket.Instance.IsConnected())
        {
            Debug.Log("disconnecting from server");
            socket.Instance.Close();
        }
    }

    private void OnDisable()
    {
        Disconnect();
    }

}
