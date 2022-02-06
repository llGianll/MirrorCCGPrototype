using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using PlayFab.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientStartUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true,
            CustomId = SystemInfo.deviceUniqueIdentifier
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnPlayFabLoginSuccess, OnLoginError);
    }

    private void OnPlayFabLoginSuccess(LoginResult loginResult)
    {
        Debug.Log("Login Success");
        RequestMultiplayerServer();
    }

    private void RequestMultiplayerServer()
    {
        Debug.Log("Client Request Multiplayer Server");
        RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest
        {
            BuildId = "c0bf8843-0ca0-4ee9-bfb8-d9b818bf45c7",
            SessionId = "28f3e794-1eab-4f07-adc5-890660676012",
            PreferredRegions = new List<string> { "NorthEurope" }
        };

        PlayFabMultiplayerAPI.RequestMultiplayerServer(requestData, OnRequestMultiplayerServer, OnRequestMultiplayerServerError);
    }

    private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
    {
        if (response == null) return;

        Debug.Log("**** These are your details **** -- IP:" + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num);

        UnityNetworkServer.Instance.networkAddress = response.IPV4Address;
        UnityNetworkServer.Instance.GetComponent<kcp2k.KcpTransport>().Port = (ushort)response.Ports[0].Num;

        UnityNetworkServer.Instance.StartClient();
    }

    private void OnRequestMultiplayerServerError(PlayFabError error)
    {
        Debug.Log("An error occured.");
    }

    private void OnLoginError(PlayFabError playFabError)
    {
        Debug.Log("Login Failed!");
    }
}
