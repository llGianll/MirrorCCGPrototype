using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    [SerializeField] Board _board;

    private void Awake()
    {
        instance = this;
    }

    public Board board => _board;
}

[System.Serializable]
public class Board
{
    public Transform playerCardArea;
    public Transform playerFrontlineArea;
    public Transform enemyCardArea;
    public Transform enemyFrontlineArea;
}


