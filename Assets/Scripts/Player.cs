using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class Player : NetworkBehaviour
{
    [SyncVar] int _currenthealth;
    [SyncVar] int _currentMana;

    public static Player localPlayer;
    public override void OnStartLocalPlayer()
    {
        localPlayer = this;
    }
}
