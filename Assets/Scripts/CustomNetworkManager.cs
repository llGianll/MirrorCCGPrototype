using UnityEngine;
using Mirror;
using System;

public class CustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);

        Debug.Log(numPlayers);

        if (numPlayers == 2)
            GameManager.instance.StartGame();
    }

}
