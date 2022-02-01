using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class CombatManager : NetworkBehaviour
{
    public static CombatManager instance;

    public List<GameObject> _cardsOnCombat = new List<GameObject>();

    [SyncVar] int _readyPlayersCount;

    List<CardManager> _cardManagers = new List<CardManager>(); //player cardManager references

    [Client]
    public void EndMainPhaseBtn(Button endButton)
    {
        endButton.interactable = false;
        CMDUpdateReadyCount();
    }

    [Command(requiresAuthority = false)]
    public void CMDUpdateReadyCount()
    {
        _readyPlayersCount++; 
        if (_readyPlayersCount >= 2)
            CombatEvaluation();
    }

    [Server]
    private void CombatEvaluation()
    {
        //[refactor] unoptimized & unclean code, does the job for now
        Debug.Log("Start Combat");
        if (_cardManagers.Count <= 0)
            _cardManagers = FindObjectsOfType<CardManager>().ToList();

        if (_cardManagers.Count <= 0)
            return;

        _cardsOnCombat.Clear();

        _cardsOnCombat = _cardManagers[0]._cardsOnField.Concat(_cardManagers[1]._cardsOnField)
                                         .OrderByDescending(x => x.GetComponent<Card>().cardStats.speed)
                                         .ToList();
        StartCoroutine(UnitCombat());
    }

    [Server]
    IEnumerator UnitCombat()
    {
        //[Todo] change iteration to backwards when unit death is implemented to avoid list modification exception 
        //[refactor] too much getcomponent calls, there should be a better way to structure this
        //It will be better if the server has its own version of the board state, but for now this unoptmized code will do
        foreach (var card in _cardsOnCombat)
        {
            //identify which is the opponent player 
            CardManager opponentCardManager = new CardManager();
            foreach (var manager in _cardManagers)
            {
                if (manager.GetComponent<Player>() != card.GetComponent<Card>().ownerOnServer)
                {
                    opponentCardManager = manager;
                    break;
                }
            }

            //get first card on field of that opponent 
            Card opponentCard = opponentCardManager._cardsOnField[0].GetComponent<Card>();
            opponentCard.cardStats.health -= card.GetComponent<Card>().cardStats.attack;

            //without using custom serializers, syncvar will not automatically synchronize custom data types values
            //so we instead send an rpc to the opponent card for it to decrease its health
            opponentCard.RPCUpdateStats(opponentCard.cardStats); //rpc to send stat update back to clients 
            Debug.Log(card.GetComponent<Card>().cardName + " attacked " + opponentCard.cardName + ".");
            yield return new WaitForSeconds(2);

        }

    }
}
