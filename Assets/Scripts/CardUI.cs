using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class CardUI : NetworkBehaviour
{
    [Header("Card UI Refs")]
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _manaText;
    [SerializeField] TMP_Text _healthText;
    [SerializeField] TMP_Text _armorText;
    [SerializeField] TMP_Text _speedText;
    [SerializeField] TMP_Text _attackText;

    [SerializeField] GameObject _cardBackGO;
 
    [Header("References")]
    [SerializeField] Card _card;

    public void UpdateCardUI()
    {
        _nameText.text      = _card.cardName.ToString();
        _manaText.text      = _card.cardCost.ToString();
        _healthText.text    = _card.cardStats.health.ToString();
        _armorText.text     = _card.cardStats.armor.ToString();
        _speedText.text     = _card.cardStats.speed.ToString();
        _attackText.text    = _card.cardStats.attack.ToString();
    }

    public void enableCardBack(bool enabled)
    {
        _cardBackGO.SetActive(enabled);
    }
}
