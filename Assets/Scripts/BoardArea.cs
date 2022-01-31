using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardArea : MonoBehaviour
{
    [SerializeField] Transform _dropArea;

    public Transform dropArea => _dropArea;
}
