using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    [Header("Board Area UI")]
    [SerializeField] Board _board;
    [Header("Combatant Stats UI")]
    [SerializeField] CombatantInfo _playerInfo;
    [SerializeField] CombatantInfo _opponentInfo;

    private void Awake()
    {
        instance = this;
    }

    public Board board => _board;
    public CombatantInfo playerInfo => _playerInfo;
    public CombatantInfo opponentInfo => _opponentInfo;
}

[System.Serializable]
public class Board
{
    public BoardArea playerCardArea;
    public BoardArea playerFrontlineArea;
    public BoardArea enemyCardArea;
    public BoardArea enemyFrontlineArea;
}

[System.Serializable]
public class CombatantInfo
{
    [SerializeField] TMP_Text _healthText;
    [SerializeField] TMP_Text _manaText;

    public TMP_Text healthText => _healthText;
    public TMP_Text manaText => _manaText;
}


