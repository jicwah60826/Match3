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

    public int maxIterationsAllowed;
    public float waitToRefillBoard;
    public float postRefillMatchCheckWait;
    public float waitToDestroyAutoMatches;

    [HideInInspector]
    public MatchFinder matchFind; // GameObject object of class MatchFinder so we can access the matchfinder script

    public enum BoardState { move, wait };
    public BoardState currentState = BoardState.move;

    public Bag bomb;
    public float bombChance;


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
        // matchFind.FindAllMatches(); // Run the find all matches function from the MatchFinder class to continually see if we have any matches

        if (Input.GetKeyDown(KeyCode.S))
        {
            ShuffleBoard();
        }
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
                int iterations = 0;
                while (MatchesAt(new Vector2Int(x, y), bags[bagToUse]) && iterations < maxIterationsAllowed)
                {
                    bagToUse = Random.Range(0, bags.Length); // pick a random bag to index ID from the bag array
                    iterations++;
                }


                SpawnBag(new Vector2Int(x, y), bags[bagToUse]); // invoke the SpawnBag function, sending the x & y position and the index ID of the bag to spawn from the bag array
            }
        }
    }

    private void SpawnBag(Vector2Int pos, Bag bagToSpawn)
    {

        if (Random.Range(0f, 100f) < bombChance)
        {
            bagToSpawn = bomb;
        }


        Bag bag = Instantiate(bagToSpawn, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity); // instantiate each bag, but spawn it in at the top of the board so that it slides into position
        bag.transform.parent = transform;
        bag.name = "Bag - " + pos.x + ", " + pos.y;

        allBags[pos.x, pos.y] = bag;

        bag.SetupBag(pos, this);
    }

    bool MatchesAt(Vector2Int posToCheck, Bag bagToCheck)
    {

        /* This function continually checks to see if we have a matching bag one the X or the Y from the current bag. This is used during the intial board setup to prevent any matching bags on setup. */

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

    private void DestroyMatchedBagAt(Vector2Int pos) // take in a vector2 int for the bag position to be destroyed
    {
        if (allBags[pos.x, pos.y] != null)
        {
            if (allBags[pos.x, pos.y].isMatched) // if the bag has been marked as matched
            {
                Instantiate(allBags[pos.x, pos.y].destroyEffect, new Vector2(pos.x, pos.y), Quaternion.identity);

                Destroy(allBags[pos.x, pos.y].gameObject); // destroy bag game object at given pos.x and pos.y
                allBags[pos.x, pos.y] = null; // null the x & y position for that baf as it no longer exists.
            }
        }
    }

    public void DestroyMatches()
    {
        // loop through matchfinder using the current matrches list
        for (int i = 0; i < matchFind.currentMatches.Count; i++) // loop through all bags in list that we need to destroy
        {
            if (matchFind.currentMatches[i] != null) /* check if the matched bags list is not null */
            {
                DestroyMatchedBagAt(matchFind.currentMatches[i].posIndex); /* pull in the index ID of the bag in the bag list and get the position index. This is fed into the DestroyMatchedBagAt function above  */
            }
        }

        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(.2f); // wait before moving bags

        // track number of empty spaces per column

        int nullCounter = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // check that gem space is null
                if (allBags[x, y] == null)
                {
                    nullCounter++; //iterate blank spot counter if the x,y spoace for a given bag is null
                }
                else if (
                    nullCounter > 0)
                {
                    allBags[x, y].posIndex.y -= nullCounter; // move the bag down the Y axis by the number os spots we find that are null
                    allBags[x, y - nullCounter] = allBags[x, y];
                    allBags[x, y] = null;

                }
            }
            nullCounter = 0; // reset the null counter after we do each column loop
        }
        StartCoroutine(FillBoardCo()); // invoke coroutine to refill the board
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(waitToRefillBoard);
        RefillBoard(); //refill the board
        yield return new WaitForSeconds(postRefillMatchCheckWait); // wait a bit until we invoke the mechanism that checks for any matches after the board has been refilled
        matchFind.FindAllMatches();
        if (matchFind.currentMatches.Count > 0)
        {
            // if we have matches again into the matchfinder list, then we invoke the process to destroy these matches
            yield return new WaitForSeconds(waitToDestroyAutoMatches);
            DestroyMatches();
        }
        else
        {
            // if no more auto-matches can be found
            yield return new WaitForSeconds(.5f);
            currentState = BoardState.move;//allow move once bags switch bag to current positions
        }
    }

    private void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if ((allBags[x, y] == null))
                {
                    // spawn bags for only those spots that are empty
                    int bagToUse = Random.Range(0, bags.Length); //pick a random bag from the array
                    SpawnBag(new Vector2Int(x, y), bags[bagToUse]); // spawn bag
                }

            }
        }

        CheckMisPlacedBags(); // check for any mis-placed bags
    }

    private void CheckMisPlacedBags()
    {
        // find all the bags that exist in the scene atr the moment
        List<Bag> foundBags = new List<Bag>();
        foundBags.AddRange(FindObjectsOfType<Bag>());

        //loop through all the bags on the board

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (foundBags.Contains(allBags[x, y]))
                {
                    foundBags.Remove(allBags[x, y]);
                }
            }
        }

        foreach (Bag b in foundBags)
        {
            Destroy(b.gameObject);
        }
    }

    public void ShuffleBoard()
    {
        if (currentState != BoardState.wait)
        {
            currentState = BoardState.wait;

            // create a list of all the bags on the board

            List<Bag> bagsFromBoard = new List<Bag>();

            // Strip the gems of the board and store into a new list

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bagsFromBoard.Add(allBags[x, y]); //get the gems from the board and into the list
                    allBags[x, y] = null; // that position on the board should now be null. Just removes the reference to the gem on the board.
                }
            }
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // take the gems from the list and put them back into the board into new positions.
                    int bagToUse = Random.Range(0, bagsFromBoard.Count); // pick a random bag from the list

                    int iterations = 0;
                    while (MatchesAt(new Vector2Int(x, y), bagsFromBoard[bagToUse]) && iterations < maxIterationsAllowed && bagsFromBoard.Count > 1)
                    {
                        // if there is a match found during shuffle, then pick a new bag to use
                        bagToUse = Random.Range(0, bagsFromBoard.Count); // pick a random bag from the list
                        iterations++;
                    }

                    // setup the new bag position
                    bagsFromBoard[bagToUse].SetupBag(new Vector2Int(x, y), this); //look at the bag were about to set. Have it go to the new point we're looking at.
                    allBags[x, y] = bagsFromBoard[bagToUse]; // on the actual board itself, put the actual bag into that slot
                    bagsFromBoard.RemoveAt(bagToUse); // remove this bag fropm the bags list so that it can't be picked again
                }
            }
            StartCoroutine(FillBoardCo());
        }
    }
}
