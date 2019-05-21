using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridPicture 
{
    //they don't pay Xiaoxun enough to deal with me at code helpdesk. 

    //6. Get a picture and slap it on the puzzle. 
    public static Texture2D[,] GetSlice(Texture2D img, int quadsPerLine)
    {
        //    set the w and h of the img
        int imgSize = Mathf.Min(img.width, img.height);
        int quadSize = imgSize / quadsPerLine;

        //    make sure the texture2d is the same as the quad w and h
        Texture2D[,] quads = new Texture2D[quadsPerLine, quadsPerLine];

        
        for (int y = 0; y < quadsPerLine; y++)
        {
            for (int x = 0; x < quadsPerLine; x++)
            {
                Texture2D quad = new Texture2D(quadSize, quadSize);

                quad.wrapMode = TextureWrapMode.Clamp;
                
                quad.SetPixels(img.GetPixels(x * quadSize, y * quadSize, quadSize, quadSize));
                quad.Apply();
                quads[x, y] = quad; 
            }
        }
        return quads; 
    }
}
