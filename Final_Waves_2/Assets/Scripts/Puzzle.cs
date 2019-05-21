using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{    
    //This started out as me trying to make a Rubiks cube and finding out it's way out of my depth. 
    //then I tried to make a 3D puzzle slider and that also failed miserably. 
    //So now I'm going for a super simple 2D puzzle that is also out of my depth
    //but I fear anything simpler than this will hardly qualify as a game. 
    
    //*The naming isn't great, but by the time I figured that out I was too far in and refactoring everything would have confused me more.  
    
    //7. Put a pic on the grid
    public Texture2D img;
    //1. SET max height and width of grid
    public int quadsPerLine = 4;
    //shuffling the quads to set up the level 
    public int shuffleLength = 20;
    public float defaultMoveDuration = .2f;
    public float shuffleMoveDuration = .1f;
    
    //10.??? enum for different game states.
    enum PuzzleState {Solved, Shuffling, InPlay};
    PuzzleState state;

    //3. MAKE an empty space to allow squares room to movew
    QuadPressed emptyQuad;
    //8. truffle shuffle the quads
    QuadPressed[,] quads;
    Queue<QuadPressed> inputs;
    bool quadIsMoving; 
    int shuffleMovesRemaining;
    Vector2Int previousShuffleOffset;
   

    private void Start()
    {
        //makin' mah grid
        CreateGrid();
    }

    
    void Update()
    {
//      //9. button for shuffling. 
//      if (Input.GetKeyDown(KeyCode.Space))
//        {
//            StartShuffle();
//        }
        //    JK now it's this. 
        if (state == PuzzleState.Solved && Input.GetKeyDown(KeyCode.Space))
        {
            StartShuffle(); //!!
        }
    }

    
    void CreateGrid()
    {
        //8.a 
        quads = new QuadPressed[quadsPerLine, quadsPerLine];
        //6.a 
        Texture2D[,] imgSlice = gridPicture.GetSlice(img, quadsPerLine);

        //2.    CREATE puzzle by adding squares on the x/y axes. 
        for (int y = 0; y < quadsPerLine; y++)
        {
            for (int x = 0; x < quadsPerLine; x++)
            {
                GameObject quadObject = GameObject.CreatePrimitive(PrimitiveType.Quad); 
                //    tried with plane. Quad is cleaner. 
                //    center the grid:  
                quadObject.transform.position = -Vector2.one * (quadsPerLine - 1) * 0.5f + new Vector2(x, y);
                quadObject.transform.parent = transform;

                //2.5???    adds the QuadPressed function/action/whatever to the grid we're makin' <-- that's my southern accent.  "makin'"
                QuadPressed quadPressed = quadObject.AddComponent<QuadPressed>();
                
                quadPressed.OnQuadPressed += MoveQuadInput;
                quadPressed.OnFinishedMoving += OnQuadFinishedMoving;
                //7.a    QuadPressed initializes start coord and text2d slice
                quadPressed.Init(new Vector2Int(x, y), imgSlice[x, y]);
                //8.b
                quads[x, y] = quadPressed;
                
                //3.a    if there's an empty space...
                if (y == 0 && x == quadsPerLine - 1)
                {
                    //slidingSquares.SetActive(false);
                    emptyQuad = quadPressed;
                }
                //#IDK?
            } 
        }

        Camera.main.orthographicSize = quadsPerLine * .55f;
        inputs = new Queue<QuadPressed>();
    }


    void MoveQuadInput(QuadPressed quadToMove)
    {
        if (state == PuzzleState.InPlay)
        {
            //    Enqueue puts stuff in the queue
            inputs.Enqueue(quadToMove);
            NextMove();
        }
    }
    
    
    void NextMove()
    {
        while (inputs.Count > 0 && !quadIsMoving)
        {
            //    Dequeue gets stuff out of the queue
            MoveQuad(inputs.Dequeue(), defaultMoveDuration);
        }
    }

    
    //4. SWAP the position of the emptyQuad with the quadPressed
    void MoveQuad(QuadPressed quadToMove, float duration)
    {
        //5. LIMIT swap positions to adjacent quads
        //    wtf how do i even 
        //    ...10 tutorials later... 
        if ((quadToMove.coord - emptyQuad.coord).sqrMagnitude == 1)    
        {            
            //9.b Swap positions in array
            quads[quadToMove.coord.x, quadToMove.coord.y] = emptyQuad;
            quads[emptyQuad.coord.x, emptyQuad.coord.y] = quadToMove;
            
            Vector2Int targetCoord = emptyQuad.coord;
            emptyQuad.coord = quadToMove.coord;
            quadToMove.coord = targetCoord; 
            
            //    position to move the pressedQuad to. 
            Vector2 targetPosition = emptyQuad.transform.position;
            emptyQuad.transform.position = quadToMove.transform.position;
            //    swap the position of the emptyQuad and the quadPressed
            quadToMove.MoveToPosition(targetPosition, duration);  //duration: .3f);
            quadIsMoving = true;
        }
    }


    //    check states 
    void OnQuadFinishedMoving()
    {
        quadIsMoving = false;
        CheckIfSolved();

        if (state == PuzzleState.InPlay)
        {
            NextMove();
        }
        else if (state == PuzzleState.Shuffling)
        {
            if (shuffleMovesRemaining > 0)
            {
                NextShuffle();
            }
            else
            {
                state = PuzzleState.InPlay;
            }
        }
    }

    
    //9. I'm so tired. 
    void StartShuffle()
    {
        state = PuzzleState.Shuffling;
            
        shuffleMovesRemaining = shuffleLength;
        
        emptyQuad.gameObject.SetActive(false);
        
        NextShuffle();    //!!
    }
    
    
    //8. 
    void NextShuffle()
    {
        //    right, left, above, below
        Vector2Int[] offsets = {new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1)};
        int randomIndex = Random.Range(0, offsets.Length);

        for (int i = 0; i < offsets.Length; i++)
        {
            //Vector2Int offset = offsets[randomIndex + i];
            Vector2Int offset = offsets[(randomIndex + i) % offsets.Length];//    % computes the remainder 

            if (offset != previousShuffleOffset * -1)    
            //    spent forever trying to figure out why this line didn't work. It's because I put and "f" after 1. 
            {
                Vector2Int moveQuadCoord = emptyQuad.coord + offset; 
                
                if (moveQuadCoord.x >= 0 && moveQuadCoord.x <= quadsPerLine && moveQuadCoord.y >= 0 &&
                    moveQuadCoord.y < quadsPerLine) //over 3 fucking hours to find that "<=" should have been "<" FML. 
                {
                    MoveQuad(quads[moveQuadCoord.x, moveQuadCoord.y], shuffleMoveDuration);//shuffleMoveDuration); //!!
                    //9.b 
                    shuffleMovesRemaining--;    // -1
                    previousShuffleOffset = offset;
                    break; //    breaks the loop
                }
            }            
        }   
    }


    void CheckIfSolved()
    {
        foreach (QuadPressed quad in quads)
        {
            if (!quad.IsAtStartingCoord())
            {
                return;
            }
        }

        state = PuzzleState.Solved;
        emptyQuad.gameObject.SetActive(true);
    }
}
