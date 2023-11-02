using System;
using Layout;
using UnityEngine;

public static class ActionsManager
{
    public static Action<float> OnDragNewsHeadline;
    public static Action <NewsHeadlinePiece, Vector2> OnReleaseNewsHeadline;
    public static Func<NewsHeadlinePiece, Vector2, NewsHeadlinePiece[], Cell[]> OnPreparingCells;
    public static Func <Cell[], Vector2, Vector3> OnSuccessFul;
}
