using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour

{
    public int width;
    public int height;

    public GameObject bgTilePreFab;

    // Start is called before the first frame update
    void Start()
    {

        setup();

    }

    private void setup()
    {
        /* build the board with loops. One for X going to the width value, then one for the Y going to the height value. */

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(bgTilePreFab, pos, Quaternion.identity);
                bgTile.transform.parent = transform; //set the spawned tiles as children of the Board gameobject
                bgTile.name = "Bg Tile - " + x + ", " + y;
            }
        }
    }
}