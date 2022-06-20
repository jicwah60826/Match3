using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    private Board board;
    public List<Bag> currentMatches = new List<Bag>();

    private void Awake()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {

        currentMatches.Clear();

        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Bag currentBag = board.allBags[x, y];
                if (currentBag != null)
                {
                    if (x > 0 && x < board.width - 1)
                    {
                        Bag leftBag = board.allBags[x - 1, y];
                        Bag rightBag = board.allBags[x + 1, y];
                        if (leftBag != null && rightBag != null)
                        {
                            if (leftBag.type == currentBag.type && rightBag.type == currentBag.type && currentBag.type != Bag.BagType.stone)
                            {
                                currentBag.isMatched = true;
                                leftBag.isMatched = true;
                                rightBag.isMatched = true;

                                currentMatches.Add(currentBag);
                                currentMatches.Add(leftBag);
                                currentMatches.Add(rightBag);

                            }
                        }
                    }
                    if (y > 0 && y < board.height - 1)
                    {
                        Bag topBag = board.allBags[x, y + 1];
                        Bag bottomBag = board.allBags[x, y - 1];
                        if (topBag != null && bottomBag != null)
                        {
                            if (topBag.type == currentBag.type && bottomBag.type == currentBag.type && currentBag.type != Bag.BagType.stone)
                            {
                                currentBag.isMatched = true;
                                topBag.isMatched = true;
                                bottomBag.isMatched = true;

                                currentMatches.Add(currentBag);
                                currentMatches.Add(topBag);
                                currentMatches.Add(bottomBag);
                            }
                        }
                    }
                }
            }
        }
        // De-dup matches and load only distincts into the list
        if (currentMatches.Count > 0)
        {
            currentMatches = currentMatches.Distinct().ToList();
        }
        CheckForBombs();
    }

    public void CheckForBombs()
    {
        for (int i = 0; i < currentMatches.Count; i++)
        {
            Bag bag = currentMatches[i];

            int x = bag.posIndex.x;
            int y = bag.posIndex.y;

            if (bag.posIndex.x > 0)
            {
                //check all bags on the x axis
                if (board.allBags[x - 1, y] != null)
                {
                    if (board.allBags[x - 1, y].type == Bag.BagType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x - 1, y), board.allBags[x - 1, y]);
                    }
                }
            }
            if (bag.posIndex.x < board.width - 1)
            {
                //check all bags on the x axis
                if (board.allBags[x + 1, y] != null)
                {
                    if (board.allBags[x + 1, y].type == Bag.BagType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x + 1, y), board.allBags[x + 1, y]);
                    }
                }
            }
            if (bag.posIndex.y > 0)
            {
                //check all bags on the x axis
                if (board.allBags[x, y - 1] != null)
                {
                    if (board.allBags[x, y - 1].type == Bag.BagType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y - 1), board.allBags[x, y - 1]);
                    }
                }
            }
            if (bag.posIndex.y < board.height - 1)
            {
                //check all bags on the x axis
                if (board.allBags[x, y + 1] != null)
                {
                    if (board.allBags[x, y + 1].type == Bag.BagType.bomb)
                    {
                        MarkBombArea(new Vector2Int(x, y + 1), board.allBags[x, y + 1]);
                    }
                }
            }

        }
    }

    public void MarkBombArea(Vector2Int bombPos, Bag theBomb)
    {
        for (int x = bombPos.x - theBomb.blastSize; x <= bombPos.x + theBomb.blastSize; x++)
        {
            for (int y = bombPos.y - theBomb.blastSize; y <= bombPos.y + theBomb.blastSize; y++)
            {
                if (x >= 0 && x < board.width && y >= 0 && y < board.height)
                {
                    if (board.allBags[x, y] != null)
                    {
                        board.allBags[x, y].isMatched = true;
                        currentMatches.Add(board.allBags[x, y]);
                    }
                }
            }
        }

        currentMatches = currentMatches.Distinct().ToList();
    }
}
