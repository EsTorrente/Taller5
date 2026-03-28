using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrrelevantClue : Clue
{
    public IrrelevantClue(string description) : base(description) {}

    public override bool IsCorrect()
    {
        return currentState == ClueState.Irrelevant;
    }
}
