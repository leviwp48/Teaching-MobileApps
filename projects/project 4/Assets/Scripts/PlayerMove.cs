using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


//TODO: 
//  Check all areas for pieces
//  highlight move spaces
//  take a piece if moved over one

namespace Checkers
{
    public class PlayerMove : MonoBehaviour
    {

        private BoxCollider2D boxCollider;
        private Rigidbody2D rb2D;

        public LayerMask newMoveHighlight;
        public LayerMask Piece;
        public LayerMask movePiece;
        public GameObject moveHighlight;
        public GameObject newPiece;
        public GameObject newMove;

        private GameManager mManeger;
        private GameObject instance = null;
        private float mousePosX;
        private float mousePosY;
        private Vector2 move1;
        private Vector2 move2;
        private Vector2 move3;

        //initializes components
        protected virtual void Start()
        {
            boxCollider = GetComponent<BoxCollider2D>();

            rb2D = GetComponent<Rigidbody2D>();
        }

        //highlights possible move spaces
        protected void ShowMoves()
        {
            //checks if there are other hightlights on board,
            //if so deletes them
            if (GameManager.isShowMoves == true)
            {
                Destroy(GameManager.moveInstance2);
                Destroy(GameManager.moveInstance3);
                GameManager.isShowMoves = false;
            }

            //checks if move 2 is "null"
            //places move 1 piece
            if (move2 == new Vector2(0, 0))
            {
                GameManager.moveInstance2 =
                    Instantiate(newMove, move1, Quaternion.identity) as GameObject;
            }
            //checks if move 1 is "null"
            //places move 2 piece
            else if (move1 == new Vector2(0, 0))
            {
                GameManager.moveInstance3 =
                Instantiate(newMove, move2, Quaternion.identity) as GameObject;
            }
            //places both pieces
            else
            {
                GameManager.moveInstance2 =
                    Instantiate(newMove, move1, Quaternion.identity) as GameObject;
                GameManager.moveInstance3 =
                    Instantiate(newMove, move2, Quaternion.identity) as GameObject;

                GameManager.isShowMoves = true;
            }
        }

        //Checks our move posiitions for other pieces
        protected void CheckMoves(float x, float y, out RaycastHit2D hit, out RaycastHit2D hit3)
        {

            float start_x = x;
            float start_y = y;
            Vector2 start = new Vector2(start_x, start_y);
            Debug.Log("got x and y");

            //if it is player ones turn, use add one to the x axis
            if (GameManager.playerOneTurn == true)
            {
                if (start_y == 7)
                {
                    move1 = new Vector2(start_x + 1, start_y - 1);
                }
                else if (start_y == 0)
                {
                    move2 = new Vector2(start_x + 1, start_y + 1);
                }
                else
                {
                    move1 = new Vector2(start_x + 1, start_y - 1);
                    move2 = new Vector2(start_x + 1, start_y + 1);
                }
            }
            //if it is player twos turn, use subtract one to the x axis
            else if (GameManager.playerTwoTurn == true)
            {
                if (start_y == 7)
                {
                    move1 = new Vector2(start_x - 1, start_y - 1);
                }
                else if (start_y == 0)
                {
                    move2 = new Vector2(start_x + 1, start_y + 1);
                }
                else
                {
                    move1 = new Vector2(start_x - 1, start_y - 1);
                    move2 = new Vector2(start_x - 1, start_y + 1);
                }
            }


            //disable this boxcollider and highlight collider
            boxCollider.enabled = false;
            moveHighlight.GetComponent<BoxCollider2D>().enabled = false;


            Debug.Log("box collider disabled");

            //check for pieces in move spaces
            hit = Physics2D.Linecast(start, move1, Piece);
            hit3 = Physics2D.Linecast(start, move2, Piece);

            Debug.Log("rays casted");

            //if there are no pieces hit   
            if (hit.collider == null)
            {
                moveHighlight.GetComponent<BoxCollider2D>().enabled = true;
                boxCollider.enabled = true;

                Debug.Log("hit collider was null");
                ShowMoves();
            }
        }

        //Make the selected move
        public void MakeMove(float x, float y)
        {
            Destroy(GameManager.moveInstance);
            Destroy(GameManager.moveInstance2);
            Destroy(GameManager.moveInstance3);
            Vector2 movePos = new Vector2(x, y);
            Destroy(this);
            GameObject makingMove =
                Instantiate(newPiece, movePos, Quaternion.identity) as GameObject;
            GameManager.counter++;
        }

        //if the lift mouse button is clicked
        public void OnMouseDown()
        {
            //if there are highlight pieces down
            if (GameManager.isPlaced == true)
            {
                //grab our click position and disable box colliders
                Debug.Log("is places is true");
                Vector2 moveMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 moveMousePos2D = new Vector2(moveMousePos.x, moveMousePos.y);
                moveHighlight.GetComponent<BoxCollider2D>().enabled = false;
                boxCollider.enabled = false;

                //check if we hit a movehightlight
                RaycastHit2D moveHit = Physics2D.Raycast(moveMousePos2D, Vector2.zero, newMoveHighlight);

                Debug.Log("move ray casted");

                //if we did hit, then make the move
                if (moveHit.collider != null)
                {
                    Debug.Log("move ray not empy");
                    float mousePosX = moveMousePos2D.x;
                    float mousePosY = moveMousePos2D.y;
                    moveHighlight.GetComponent<BoxCollider2D>().enabled = true;
                    boxCollider.enabled = true;

                    MakeMove(mousePosX, mousePosY);
                }
                //if we didn't then delete other highlights and reset 
                else
                {
                    Debug.Log("move ray empyty");
                    Destroy(GameManager.moveInstance);
                    Destroy(GameManager.moveInstance2);
                    Destroy(GameManager.moveInstance3);
                    GameManager.isPlaced = false;
                    boxCollider.enabled = true;
                    moveHighlight.GetComponent<BoxCollider2D>().enabled = true;
                }
            }
            //if there are no highlights 
            else
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit2 = Physics2D.Raycast(mousePos2D, Vector2.zero, Piece);

                if (hit2.collider != null)
                {
                    if (hit2.collider.tag == "player1" && GameManager.playerOneTurn == false)
                    {
                        return;
                    }

                    if (hit2.collider.tag == "player2" && GameManager.playerTwoTurn == false)
                    {
                        return;
                    }

                    //places highlight at selected piece
                    GameManager.moveInstance =
                        Instantiate(moveHighlight, hit2.collider.gameObject.transform.position, Quaternion.identity) as GameObject;
                    RaycastHit2D hit;
                    RaycastHit2D hit3;
                    float mousePosX = hit2.collider.gameObject.transform.position.x;
                    float mousePosY = hit2.collider.gameObject.transform.position.y;
                    GameManager.isPlaced = true;
                    //start checking moves
                    CheckMoves(mousePosX, mousePosY, out hit, out hit3);
                }
            }
        }

        //checks if it is a players turn
        //also looks at what input we are using
        private void Update()
        {
            //If it's not the player's turn, exit the function.
            if (!GameManager.playerOneTurn) return;
            if (!GameManager.playerTwoTurn) return;


        }
    }
}