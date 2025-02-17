using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager instance;
    public string gameVersion;
    public bool isAuthented = false;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (instance != null && instance != this)
        {
            Debug.LogError("PhotonManager have 2");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickLogoutButton()
    {
        PhotonNetwork.Disconnect();
    }
    public void ConnectToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToServer");
        //SceneManager.LoadScene("Home");

        if (isAuthented)
        {
            if (UIManager.instance.isWatingMatch)
            {
                UIManager.instance.TurnOnChooseDeckScene();
            }
            //else if (UIManager.instance.isHome)
            //{
            //    StartCoroutine(GameData.instance.LoadingGameProcess());

            //}
        }
        else
        {
            if (UIManager.instance.isSignIn || UIManager.instance.isSignUp)
            {
                isAuthented = true;
                StartCoroutine(GameData.instance.LoadingGameProcess());

            }
        }

    }


    public void HandlerFriendUpdate(List<PlayFab.ClientModels.FriendInfo> friendInfos)
    {
        if (friendInfos.Count > 0)
        {
            string[] friendDisplayName = friendInfos.Select(f=>f.Username).ToArray();
            //PhotonNetwork.FindFriends(friendDisplayName);
            ChatManager.instance.chatClient.AddFriends(friendDisplayName);
        }
    }

    public override void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        print("OnFriendListUpdate");
        base.OnFriendListUpdate(friendList);
        foreach(FriendInfo a in friendList)
        {
            print(a.UserId +"Status: "+a.IsOnline);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isAuthented = false;
        print(cause.ToString());
    }
}
