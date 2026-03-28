using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelevantClue : Clue
{
    public RelevantClue(string description) : base(description) {}

    public override bool IsCorrect()
    {
        return currentState == ClueState.Relevant;
    }
}
