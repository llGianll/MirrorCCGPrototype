using TMPro;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

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

    [Header("Marker References")]
    [SerializeField] GameObject _markerGO;
    [SerializeField] Image _markerImg;
    [SerializeField] Sprite _attackerSprite;
    [SerializeField] Sprite _targetSprite;
 
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

    public void EnableCardBack(bool enabled) => _cardBackGO.SetActive(enabled);

    [ClientRpc]
    public void RPCEnableCombatMarker(bool isAttacker)
    {
        _markerGO.SetActive(true);
        _markerImg.sprite = (isAttacker) ? _attackerSprite : _targetSprite;
        Debug.Log("Enable Combat Marker");
    }

    [ClientRpc]
    public void RPCDisableCombatMarker() => _markerGO.SetActive(false);
}
