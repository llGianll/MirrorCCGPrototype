using UnityEngine;

[CreateAssetMenu(menuName = "CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] string _cardName;
    [SerializeField] string _cardDescription;
    [SerializeField] CardStats _cardStats;
    [SerializeField] int _cardCost;

    public string cardName => _cardName;    
    public string cardDescription => _cardDescription;
    public CardStats cardStats => _cardStats;
    public int cardCost => _cardCost;
}

[System.Serializable]
public class CardStats
{
    public int health;
    public int speed;
    public int attack;
    public int armor;

    public CardStats() { } //default constructor

    public CardStats(CardStats stats) //copy constructor
    {
        health = stats.health;
        speed = stats.speed;
        attack = stats.attack;
        armor = stats.armor;
    }
}
