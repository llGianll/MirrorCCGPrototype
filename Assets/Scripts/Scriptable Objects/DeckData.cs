using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Deck")]
public class DeckData : ScriptableObject
{
    [SerializeField] List<CardData> _deck = new List<CardData>();

    public List<CardData> deck => _deck;
    
}
