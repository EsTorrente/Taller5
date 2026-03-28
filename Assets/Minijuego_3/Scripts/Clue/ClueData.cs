using System;
using UnityEngine;

[Serializable]
public class ClueData
{
    [Header("Clue Info")]
    public string description;

    public bool isActuallyRelevant;

    [Header("Position")]
    public Vector2 position;

    [Header("State (runtime)")]
    public ClueState currentState = ClueState.None;
}