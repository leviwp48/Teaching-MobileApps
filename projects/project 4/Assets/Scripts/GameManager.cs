using UnityEngine;
using System.Collections;

namespace Checkers
{
    public class GameManager : MonoBehaviour
    {
        public Board boardScript;
        public static bool playerOneTurn = true;
        public static bool playerTwoTurn = false;
        public static GameManager instance = null;
        public static bool isPlaced = false;
        public static bool isShowMoves = false;
        public static GameObject moveInstance;
        public static GameObject moveInstance2;
        public static GameObject moveInstance3;
        public static int counter = 2;

        //called before any Start function
        void Awake()
        {
            //Check if instance already exists
            if(instance == null)
            {
                //if not, set instance to this
                instance = this;
            }
            //if instance already exists and it's not this:'
            else if(instance != this)
            {
                //then destroy this. this enforces our singleton 
                Destroy(gameObject);
            }
            //sets this to not be destroyed when reloading scene
            DontDestroyOnLoad(gameObject);

            //grabs the board script component
            boardScript = GetComponent<Board>();
            InitGame();
        }

        void InitGame()
        {
            counter = 0;

            boardScript.SetupScene();
        }

        void Update()
        {
            //what do i need here?
            //anything that I need to manage? time maybe? moves?
            //but not selection, this is to just keep things going.
            //keep track of players turns.
            
            //have a counter that ticks
            //depening on even or odd, the players turns change.
            if(counter % 2 == 0)
            {
                playerOneTurn = true;
                playerTwoTurn = false;
            }
            else
            {
                playerOneTurn = false;
                playerTwoTurn = true;
            }
        }
    }
}