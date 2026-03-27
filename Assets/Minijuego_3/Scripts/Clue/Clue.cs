using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Clue
{
    public string description;
    protected ClueState currentState = ClueState.None;

    public Clue(string description)
    {
        this.description = description;
    }

    public void CycleState()
    {
        currentState = (ClueState)(((int)currentState + 1) % 3);
    }

    public ClueState GetState()
    {
        return currentState;
    }

    public abstract bool IsCorrect();
}
