using Layout;
using Managers;
using NoMonoBehavior;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PieceGenerator
{
    public NewsHeadlinePiece Generate(NewsData newsData, Transform newsHeadlinesPiecesContainer)
    {
        NewsType type = newsData.type;
        PieceData pieceData = new PieceData((int)type, PieceData.pivots[(int)type]);
        Vector2[] pieceCoords = pieceData.ConvertToRelativeTileCoordinates();
        Vector2 pieceSize = pieceData.GetPieceSize();
        List<NewsHeadlineSubPiece> subPieces = new List<NewsHeadlineSubPiece>();
        GameObject op = FakeInstantiate(newsHeadlinesPiecesContainer);
        NewsHeadlinePiece newsHeadlinePiece = op.AddComponent<NewsHeadlinePiece>();

        Color subPieceColor = PieceData.newsTypeColors[(int)newsData.type];

        foreach (Vector2 v in pieceCoords)
        {
            GameObject os = FakeInstantiate(op.transform);
            Image subPieceImage = os.AddComponent<Image>();
            NewsHeadlineSubPiece subPiece = os.AddComponent<NewsHeadlineSubPiece>();

            subPiece.SetCoordinates(v);
            subPiece.SetPositionFromCoordinates();
            subPiece.SetNewsHeadlinePiece(newsHeadlinePiece);

            subPieceImage.color = subPieceColor;

            subPieces.Add(subPiece);
        }
        newsHeadlinePiece.SetSubPieces(subPieces.ToArray());

        GameObject h = FakeInstantiate(subPieces[subPieces.Count - 1].transform);
        TextMeshProUGUI headline = h.AddComponent<TextMeshProUGUI>();
        
        headline.text = newsData.biases[0].headline;
        headline.color = Color.black;
        headline.fontSize = 7f;
        headline.rectTransform.anchorMax = Vector2.one;
        headline.rectTransform.anchorMin = Vector2.zero;
        headline.rectTransform.pivot = new Vector2(.5f, .5f);
        headline.rectTransform.anchoredPosition = Vector2.zero;
        headline.rectTransform.sizeDelta = Vector2.zero;

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
