using UnityEngine;
using Mirror;
using System;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] GameObject _waitingPanel;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //same implementation of player instantiation and networkserver player registration
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);

        if (numPlayers >= 2)
            GameManager.instance.StartGame();
    }

}
