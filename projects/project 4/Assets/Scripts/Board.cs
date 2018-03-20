using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class Board : MonoBehaviour
    {
        public int columns = 8;
        public int rows = 8;
        public GameObject boardTilesBlack;
        public GameObject boardTilesRed;
        public GameObject boardPieceBlack;
        public GameObject boardPieceRed;
        private int counter = 0;
        private int space = 1;
        private Transform boardHolder;
        public List<Vector3> gridPositions = new List<Vector3>();

        //Clears our list gridPositions and prepares it to generate a new board.
        public void InitialiseList()
        {
            //Clear our list gridPositions.
            gridPositions.Clear();

            //Loop through x axis (columns).
            for (int x = 0; x < columns; x++)
            {
                //Within each column, loop through y axis (rows).
                for (int y = 0; y < rows; y++)
                {
                    //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                    gridPositions.Add(new Vector3(x, y, 0f));
                }
            }
        }

        //sets up the board
        void BoardSetup()
        {
            boardHolder = new GameObject("Board").transform;

            for (int x = 0; x < columns; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    GameObject toInstantiate = boardTilesBlack;

                    if (x == 0 && y % 2 == 0)
                    {
                        toInstantiate = boardTilesRed;
                    }
                    else if (x == 1 && y % 2 != 0)
                    {
                        toInstantiate = boardTilesRed;
                    }
                    else if (x == 2 && y % 2 == 0)
                    {
                        toInstantiate = boardTilesRed;
                    }
                    else if (x == 3 && y % 2 != 0)
                    {
                        toInstantiate = boardTilesRed;
                    }
                    else if (x == 4 && y % 2 == 0)
                    {
                        toInstantiate = boardTilesRed;
                    }
                    else if (x == 5 && y % 2 != 0)
                    {
                        toInstantiate = boardTilesRed;
                    }
                    else if (x == 6 && y % 2 == 0)
                    {
                        toInstantiate = boardTilesRed;
                    }
                    else if (x == 7 && y % 2 != 0)
                    {
                        toInstantiate = boardTilesRed;
                    }


                    GameObject instance =
                        Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                    instance.transform.SetParent(boardHolder);
                }
            }
        }

        //Sets up pieces on board
        void PieceSetup()
        {
            GameObject toInstantiate = boardPieceBlack;
            for (int i = 0; i < 32; i++)
            {
            if (i < 12)
                {                  
                    toInstantiate = boardPieceBlack;
                    GameObject instance =
                            Instantiate(toInstantiate, gridPositions[space], Quaternion.identity) as GameObject;

                    instance.transform.SetParent(boardHolder);
                    space = space + 2;
                 
                    if(space == 9)
                    {
                        space = space - 1 ;
                    }

                    if (space == 16)
                    {
                        space++;
                    }

                    if(space == 25)
                    {
                        space = 40;
                    }

                }
                else if (i > 16)
                {                   
                    toInstantiate = boardPieceRed;
                    GameObject instance =
                           Instantiate(toInstantiate, gridPositions[space], Quaternion.identity) as GameObject;

                    instance.transform.SetParent(boardHolder);
                    space = space + 2;

                    if(space == 48)
                    {
                        space++;
                    }

                    if (space == 57)
                    {
                        space = space - 1;
                    }
                }
            }
        }

        public void SetupScene()
        {
            InitialiseList();
            BoardSetup();
            PieceSetup();

        }
    }
}