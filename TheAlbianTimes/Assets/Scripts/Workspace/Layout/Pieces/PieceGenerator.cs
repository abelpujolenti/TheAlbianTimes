using System.Collections.Generic;
using System.Linq;
using NoMonoBehavior;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Workspace.Layout.Pieces
{
    public class PieceGenerator
    {
        public NewsHeadlinePiece Generate(NewsData newsData, Transform newsHeadlinesPiecesContainer)
        {
            NewsType type = newsData.type;
            PieceData pieceData = new PieceData((int)type, PieceData.pivots[(int)type]);
            Vector2[] pieceCoords = pieceData.ConvertToRelativeTileCoordinates();
            Vector2 pieceSize = pieceData.GetPieceSize();
            List<NewsHeadlineSubPiece> subPieces = new List<NewsHeadlineSubPiece>();
            List<TextMeshProUGUI> subPieceTexts = new List<TextMeshProUGUI>();
            GameObject op = FakeInstantiate.Instantiate(newsHeadlinesPiecesContainer);
            NewsHeadlinePiece newsHeadlinePiece = op.AddComponent<NewsHeadlinePiece>();

            Color subPieceColor = PieceData.newsTypeColors[(int)newsData.type];

            foreach (Vector2 v in pieceCoords)
            {
                GameObject os = FakeInstantiate.Instantiate(op.transform);
                Image subPieceImage = os.AddComponent<Image>();
                NewsHeadlineSubPiece subPiece = os.AddComponent<NewsHeadlineSubPiece>();

                subPiece.SetCoordinates(v);
                subPiece.SetPositionFromCoordinates();
                subPiece.SetNewsHeadlinePiece(newsHeadlinePiece);

                subPieceImage.color = new Color(0.4f, 0.4f, 0.4f);

                subPieces.Add(subPiece);

                RectMask2D mask = subPiece.gameObject.AddComponent<RectMask2D>();
                mask.padding = new Vector4(2f, 2f, 2f, 2f);

                GameObject h = FakeInstantiate.Instantiate(subPiece.transform);
                TextMeshProUGUI headline = h.AddComponent<TextMeshProUGUI>();

                headline.text = "<font=cour SDF>" + newsData.biases[0].headline;
                headline.color = new Color(0.245f, 0.245f, 0.245f);
                headline.fontSize = 9f;
                headline.rectTransform.anchorMax = Vector2.one;
                headline.rectTransform.anchorMin = Vector2.zero;
                headline.rectTransform.pivot = new Vector2(.5f, .5f);
                headline.rectTransform.anchoredPosition = Vector2.zero;
                headline.rectTransform.sizeDelta = Vector2.zero;
                headline.rectTransform.offsetMax = new Vector2(-2, -2);
                headline.rectTransform.offsetMin = new Vector2(2, 2);
                headline.overflowMode = TextOverflowModes.Linked;
                subPieceTexts.Add(headline);
            }

            for (int i = 0; i < subPieceTexts.Count; i++)
            {
                Debug.Log(subPieceTexts[i].transform.position);
            }
            int pivotIndex = PieceData.pivotIndex[(int)newsData.type];
            var pivotText = subPieceTexts.First();
            subPieceTexts.RemoveAt(0);
            subPieceTexts.Insert(pivotIndex, pivotText);
            for (int i = 1; i < subPieceTexts.Count; i++)
            {
                subPieceTexts[i].linkedTextComponent = subPieceTexts[i- 1];
            }
            Debug.Log("after:");
            for (int i = 0; i < subPieceTexts.Count; i++)
            {
                Debug.Log(subPieceTexts[i].transform.position);
            }
            Debug.Log("--------------\nbefore:");

            newsHeadlinePiece.SetSubPieces(subPieces.ToArray());



            return newsHeadlinePiece;
        }
    }
}
