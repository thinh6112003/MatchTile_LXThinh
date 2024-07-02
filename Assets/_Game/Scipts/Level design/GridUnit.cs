using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUnit : MonoBehaviour
{
    private int x;
    private int y;
    // getter and setter for x , y 
    public int GetX { get { return x; } }
    public int GetY { get { return y; } }
    public void setPos(int X, int Y)
    {
        x = X;
        y = Y;
    }
    public (int,int) getPos()
    {
        return (x, y);
    }
}
