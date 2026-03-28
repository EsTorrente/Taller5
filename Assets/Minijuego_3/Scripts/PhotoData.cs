using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhotoData
{
    public Sprite photoSprite;

    public List<ClueData> clues;
    public string finalClue;
}