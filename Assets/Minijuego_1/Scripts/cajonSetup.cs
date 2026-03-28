using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cajonSetup : MonoBehaviour
{

    private cajon board;

    void Awake()
    {
        board = GetComponent<cajon>();
        PositionAllBlocks();
    }

    void PositionAllBlocks()
    {
        objetosCocina[] blocks = FindObjectsOfType<objetosCocina>();
        foreach (var block in blocks)
        {
            Vector3 pos = board.GridToWorldCentered(
                block.gridCol, block.gridRow,
                block.widthInSlots, block.heightInSlots);
            block.transform.position = pos;
        }
    }
}

