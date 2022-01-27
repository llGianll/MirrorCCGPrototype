using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Card : NetworkBehaviour
{
    [SyncVar] public CardStats cardStats;
    [SyncVar] public string cardName;
    [SyncVar] public string cardDescription;
    [SyncVar] public int cardCost;

    public void LoadCardData(CardData cardData)
    {
        cardStats = cardData.cardStats;
        cardName = cardData.cardName;
        cardDescription = cardData.cardDescription;
        cardCost = cardData.cardCost;
    }
}
