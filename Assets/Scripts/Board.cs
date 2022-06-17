using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour

{
    public int width; // define the board width
    public int height; // define the board height

    public GameObject bgTilePreFab; // what object to use for the BG tile

    public Bag[] bags; // array for the bags that are spawned

    public Bag[,] allBags; // 2d array for all of the bags so we can keep track of bag positions

    public float gemSpeed; // how fast the gems will move when they are swapped

    [HideInInspector]
    public MatchFinder matchFind; // GameObject object of class MatchFinder so we can access the matchfinder script

    private void Awake()
    {
        matchFind = FindObjectOfType<MatchFinder>(); // find the matchfinder script on awake
    }

    // Start is called before the first frame update
    void Start()
    {

        allBags = new Bag[width, height]; // define the all bags array so that we can store each bags width and height location

        setup(); // build the board

    }

    private void Update()
    {
        matchFind.FindAllMatches(); // Run the find all matches function from the MatchFinder class to continually see if we have any matches
    }

    private void setup()
    {
        /* 
        
        build the board with loops. One for X going to the width value, then one for the Y going to the height value.

        The board will be built from lower left. Then go in the Y till we have reached the height. Then repeat, building the next column over until we have built all bags in that column.
        
        */

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y); //define a new vector 2 to store the position value for EACH BG tile on the board
                GameObject bgTile = Instantiate(bgTilePreFab, pos, Quaternion.identity); // instantiate the BG tiles
                bgTile.transform.parent = transform; //set the spawned BG tiles as children of the Board gameobject
                bgTile.name = "Bg Tile - " + x + ", " + y; // give each bg tile a name that has the X, Y position


                int bagToUse = Random.Range(0, bags.Length); // pick a random bag to index ID from the bag array

                while (MatchesAt(new Vector2Int(x,y), bags[bagToUse]))
                {
                    bagToUse = Random.Range(0, bags.Length); // pick a random bag to index ID from the bag array
                }


                SpawnBag(new Vector2Int(x, y), bags[bagToUse]); // invoke the SpawnBag function, sending the x & y position and the index ID of the bag to spawn from the bag array
            }
        }
    }

    private void SpawnBag(Vector2Int pos, Bag bagToSpawn)
    {
        Bag bag = Instantiate(bagToSpawn, new Vector3(pos.x, pos.y, 0f), Quaternion.identity); // instantiate each bag 
        bag.transform.parent = transform;
        bag.name = "Bag - " + pos.x + ", " + pos.y;

        allBags[pos.x, pos.y] = bag;

        bag.SetupBag(pos, this);
    }

    bool MatchesAt(Vector2Int posToCheck, Bag bagToCheck)
    {

        // This function continually checks to see if we have a matching bag one the X or the Y from the current bag. This is used during the intial board setup to prevent any matching bags on setup.

        if (posToCheck.x > 1)
        {
            if (allBags[posToCheck.x - 1, posToCheck.y].type == bagToCheck.type && allBags[posToCheck.x - 2, posToCheck.y].type == bagToCheck.type)
            {
                return true;
            }
        }

        if (posToCheck.y > 1)
        {
            if (allBags[posToCheck.x, posToCheck.y - 1].type == bagToCheck.type && allBags[posToCheck.x, posToCheck.y - 2].type == bagToCheck.type)
            {
                return true;
            }
        }

        return false;
    }
}