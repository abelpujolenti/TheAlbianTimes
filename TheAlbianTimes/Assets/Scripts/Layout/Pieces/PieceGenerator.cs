using Layout;
using Managers;
using NoMonoBehavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieceGenerator
{
    public NewsHeadlinePiece Generate(NewsType type, Transform newsHeadlinesPiecesContainer)
    {
        PieceData pieceData = new PieceData((int)type, PieceData.pivots[(int)type]);
        Vector2[] pieceCoords = pieceData.ConvertToRelativeTileCoordinates();
        List<NewsHeadlineSubPiece> subPieces = new List<NewsHeadlineSubPiece>();
        GameObject op = FakeInstantiate(newsHeadlinesPiecesContainer);
        NewsHeadlinePiece newsHeadlinePiece = op.AddComponent<NewsHeadlinePiece>();
        
        foreach (Vector2 v in pieceCoords)
        {
            GameObject os = FakeInstantiate(op.transform);
            os.AddComponent<Image>();
            NewsHeadlineSubPiece subPiece = os.AddComponent<NewsHeadlineSubPiece>();
            subPiece.SetCoordinates(v);
            subPiece.SetNewsHeadlinePiece(newsHeadlinePiece);
            subPiece.draggable = true;
            subPieces.Add(subPiece);
        }
        newsHeadlinePiece.SetSubPieces(subPieces.ToArray());
        return newsHeadlinePiece;
    }

    private GameObject FakeInstantiate(Transform parent)
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(parent);
        obj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        obj.transform.localScale = Vector3.one;
        return obj;
    }
}
