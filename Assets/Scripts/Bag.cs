using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{

    [HideInInspector]
    public Vector2Int posIndex;
    [HideInInspector]
    public Board board;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    private bool mousePressed;
    private float swipeAngle = 0;

    private Bag otherBag;

    public enum BagType { blue, green, red, orange, purple }; // specify the values to use in the enum
    public BagType type; // expose the enum options as type in the editor
    public bool isMatched;
    [HideInInspector]
    public Vector2Int previousPosition;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        // Animate the gem movement based on the movepieces logic
        if (Vector2.Distance(transform.position, posIndex) > .01f)
        {
            transform.position = Vector2.Lerp(transform.position, posIndex, board.gemSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
            board.allBags[posIndex.x, posIndex.y] = this; // update the board
        }


        if (mousePressed == true && Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // find main camera and convert cam space to world space
            //Debug.Log("finalTouchPosition: " + finalTouchPosition);
            CalculateAngle();
        }
    }

    public void SetupBag(Vector2Int pos, Board theBoard)
    {
        posIndex = pos;
        board = theBoard;
    }

    private void OnMouseDown()
    {
        //Debug.Log("pressed - " + name);
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // find main camera and convert cam space to world space
        //Debug.Log("firstTouchPosition: " + firstTouchPosition);
        mousePressed = true;
    }

    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x); // returns a radian value between 0 and PI
        swipeAngle = swipeAngle * 180 / Mathf.PI; //convert the radians above to degrees
        //Debug.Log("swipeAngle:" + swipeAngle);

        if (Vector3.Distance(firstTouchPosition, finalTouchPosition) > .5f) //only move pieces if distance between click points is greater than half the distance of an individual bag tile
        {
            MovePieces();
        }


    }

    private void MovePieces()
    {
        previousPosition = posIndex; //stopre the current bag position before any moving is done

        // Swiping Right?
        if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1)
        {
            otherBag = board.allBags[posIndex.x + 1, posIndex.y];
            otherBag.posIndex.x--;
            posIndex.x++;
        }
        // Swiping Upwards?
        else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1)
        {
            otherBag = board.allBags[posIndex.x, posIndex.y + 1];
            otherBag.posIndex.y--;
            posIndex.y++;
        }
        // Swiping Down?
        else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)
        {
            otherBag = board.allBags[posIndex.x, posIndex.y - 1];
            otherBag.posIndex.y++;
            posIndex.y--;
        }
        else if (swipeAngle > 135 || swipeAngle < -135 && posIndex.x > 0)
        {
            otherBag = board.allBags[posIndex.x - 1, posIndex.y];
            otherBag.posIndex.x++;
            posIndex.x--;
        }

        board.allBags[posIndex.x, posIndex.y] = this;
        board.allBags[otherBag.posIndex.x, otherBag.posIndex.y] = otherBag;

        StartCoroutine(CheckMoveCo());
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f); // wait

        board.matchFind.FindAllMatches(); //invoke the Matchfinder function

        if (otherBag != null)
        {
            if (!isMatched && !otherBag.isMatched)
            {
                otherBag.posIndex = posIndex;
                posIndex = previousPosition;

                board.allBags[posIndex.x, posIndex.y] = this;
                board.allBags[otherBag.posIndex.x, otherBag.posIndex.y] = otherBag;
            }
        }
    }
}
