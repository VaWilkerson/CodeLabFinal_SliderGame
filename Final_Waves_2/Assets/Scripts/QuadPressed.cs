using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadPressed : MonoBehaviour
{
    
    //    Figure out if the square is being pressed
    public event System.Action<QuadPressed> OnQuadPressed;
    //    this was horrible to figure out. Thanks Google.
    //    what I gathered is that actions are delegates,
    //    and delegates are like variables that are actually functions. 
    //    I'm sure there's a better and easier way to do this but this is how it's getting done. 
    public event System.Action OnFinishedMoving;
    //7. 
    public Vector2Int coord;
    private Vector2Int startingCoord;

    
    public void Init(Vector2Int startingCoord, Texture2D img)
    {
        this.startingCoord = startingCoord;
        coord = startingCoord;
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Quad");
        GetComponent<MeshRenderer>().material.mainTexture = img;
    }

    
    public void MoveToPosition(Vector2 target, float duration)
    {
        StartCoroutine(AnimateMove(target, duration));
    }
    
    
    //2.5???
    void OnMouseDown()
    {
        if (OnQuadPressed != null)
        {
            OnQuadPressed(this);
        }
    }

    
    //#???    I lost track of what number I was on. 
    //        smooths out the quad movement.
    //        Xiaoxun was really patient while I asked him to explain every single part of this to my one brain cell.
    IEnumerator AnimateMove(Vector2 target, float duration)
    {
        Vector2 initialPosition = transform.position;
        float percent = 0;
        
        while (percent < 1)
        {
            percent += Time.deltaTime / duration;
            transform.position = Vector2.Lerp(initialPosition, target, percent);
            yield return null; 
        }

        if (OnFinishedMoving != null)
        {
            OnFinishedMoving();
        }
    }


    public bool IsAtStartingCoord()
    {
        return coord == startingCoord;
    }
}
