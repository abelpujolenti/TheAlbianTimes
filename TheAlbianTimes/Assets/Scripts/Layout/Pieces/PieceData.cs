using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceData
{
    public const int size = 5;
    public static readonly int[] shapes = { 35, 1057, 3138, 99, 1123, 199, 486, 68705, 101473, 7239, 3567, 9711, 7399, 6383 };
    public static readonly int[] pivots = { 0, 5, 6, 0, 5, 1, 6, 5, 10, 6, 6, 6, 6, 6 };
    public int shape;
    public int pivot;

    public PieceData(int typeIndex, int pivot = 0)
    {
        shape = shapes[typeIndex];
        this.pivot = pivot;
    }
    public Vector2[] ConvertToRelativeTileCoordinates()
    {
        List<Vector2> ret = new List<Vector2>();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int index = i * size + j;
                int p = (1 << index);
                int bit = (shape & p) / p;
                if (bit == 1)
                {
                    ret.Add(new Vector2(j, i) - new Vector2(pivot % size, pivot / size));
                }
            }
        }
        return ret.ToArray();
    }
    /*public Vector2[] ConvertToAbsoluteCoordinates(Vector2 pivotCoordinate, Vector2 pieceSize)
    {
        List<Vector2> ret = new List<Vector2>();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int index = i * size + j;
                int p = (1 << index);
                int bit = (shape & p) / p;
                if (bit == 1)
                {
                    ret.Add(pivotCoordinate + new Vector2(pieceSize.x * j, pieceSize.y * i));
                } 
            }
        }
        return ret.ToArray();
    }*/
    public int DebugCreate(int[] shape)
    {
        int res = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int index = i * size + j;
                res += shape[index] * (1 << index);
            }
        }
        Debug.Log(res);
        return res;
    }
    public void DebugTest()
    {
        string d = "";
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int index = i * size + j;
                int p = (1 << index);
                int bit = (shape & p) / p;
                d += bit;
            }
            d += "\n";
        }
        Debug.Log(d);
    }
}
