using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Card : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnCardStatsUpdate))] public CardStats cardStats;
    [SyncVar(hook = nameof(OnCardNameUpdate))] public string cardName;
    [SyncVar] public string cardDescription;
    [SyncVar(hook = nameof(OnCardCostUpdate))] public int cardCost;

    [SerializeField] CardUI _cardUI;

    //[refactor] hooks later
    void OnCardStatsUpdate(CardStats oldValue, CardStats newValue) => _cardUI.UpdateCardUI();
    void OnCardNameUpdate(string oldValue, string newValue) => _cardUI.UpdateCardUI();
    void OnCardCostUpdate(int oldValue, int newValue) => _cardUI.UpdateCardUI();

    public void LoadCardData(CardData cardData)
    {
        cardStats = cardData.cardStats;
        cardName = cardData.cardName;
        cardDescription = cardData.cardDescription;
        cardCost = cardData.cardCost;
    }
}
