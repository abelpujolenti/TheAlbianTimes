using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceData
{
    public const int size = 5;
    public static readonly int[] shapes = { 35, 1057, 3138, 99, 1123, 199, 486, 68705, 101473, 7239, 3567, 9711, 7399, 6383 };
    public static readonly int[] pivots = { 0, 5, 6, 5, 5, 1, 6, 5, 10, 6, 6, 6, 6, 6 };
    public static readonly Color[] newsTypeColors = { new Color32(230, 76, 166, 255), new Color32(76, 137, 230, 255), new Color32(229, 177, 75, 255),
        new Color32(85, 230, 76, 255), new Color32(230, 86, 71, 255), new Color32(85, 71, 230, 255), new Color32(155, 77, 229, 255),
        new Color32(131, 153, 178, 255), new Color32(171, 230, 71, 255), new Color32(71, 202, 230, 255), Color.gray, Color.gray, Color.gray, Color.gray};
    public static readonly Color[] biasColors = { new Color(0.2588235f, 0.8078431f, 0.4039216f, 1f), new Color(0.2862745f, 0.4502465f, 0.8862745f, 1f), new Color(0.8078431f, 0.2588235f, 0.4563914f, 1f), new Color(0.7764706f, 0.7254902f, 0.2313726f, 1f) };
    public static readonly string[] newsTypeName = { "Politics", "Opinion", "Entertainment", "Diplomacy", "Economics", "Military", "Cultural", "Technology", "Labor", "Ideology" };


    public int shape;
    public int pivot;

    public PieceData(int typeIndex, int pivot = 0)
    {
        shape = shapes[typeIndex];
        this.pivot = pivot;
    }
    public Vector2[] ConvertToRelativeTileCoordinates()
    {
        Vector2 pivotCell = default;
        
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
                    Vector2 cell = new Vector2(j, i) - new Vector2(pivot % size, pivot / size);
                    if (cell == Vector2.zero)
                    {
                        pivotCell = cell;
                        continue;
                    }
                    ret.Add(cell);
                }
            }
        }
        ret.Insert(0, pivotCell);
        return ret.ToArray();
    }
    public Vector2 GetPieceSize()
    {
        Vector2 ret = Vector2.zero;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int index = i * size + j;
                int p = (1 << index);
                int bit = (shape & p) / p;
                if (bit == 1)
                {
                    if (ret.x < j)
                    {
                        ret.x = j;
                    }
                    ret.y = i;
                }
            }
        }
        ret += Vector2.one;
        return ret;
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
