using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardLayout : MonoBehaviour
{

    public LayoutRow[] allRows;

    public Bag[,] GetLayout()
    {
        Bag[,] theLayout = new Bag[allRows[0].gemsInRow.Length, allRows.Length];

        for (int y = 0; y < allRows.Length; y++)
        {
            for (int x = 0; x < allRows[y].gemsInRow.Length; x++)
            {
                if (x < allRows[0].gemsInRow.Length)
                {
                    if (x < theLayout.GetLength(0))
                    {
                        if (allRows[y].gemsInRow[x] != null)
                        {
                            theLayout[x, allRows.Length - 1 -y ] = allRows[y].gemsInRow[x];
                        }
                    }
                }
            }
        }

        


        return theLayout;
    }
}

[System.Serializable]
public class LayoutRow

{
    public Bag[] gemsInRow;
}
