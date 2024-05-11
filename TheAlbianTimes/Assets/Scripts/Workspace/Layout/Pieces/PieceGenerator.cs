using System.Collections.Generic;
using Managers;
using NoMonoBehavior;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Workspace.Layout.Pieces
{
    public class PieceGenerator
    {
        private Sprite metalPiece;
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

            if (metalPiece == null)
            {
                metalPiece = Resources.Load<Sprite>("Images/Layout/metal_piece");
            }

            bool newTMP = false;
            int textLength = 0;

            for (int i = 0; i < pieceCoords.Length; i++)
            {
                Vector2 v = pieceCoords[i];
                GameObject os = FakeInstantiate.Instantiate(op.transform);
                Image subPieceImage = os.AddComponent<Image>();
                subPieceImage.color = ColorUtil.SetBrightness(ColorUtil.SetSaturation(subPieceColor, 0.3f), 0.8f);
                NewsHeadlineSubPiece subPiece = os.AddComponent<NewsHeadlineSubPiece>();

                subPiece.SetCoordinates(v);
                subPiece.SetPositionFromCoordinates();
                subPiece.SetNewsHeadlinePiece(newsHeadlinePiece);

                subPieceImage.sprite = metalPiece;

                if (v == Vector2.zero)
                {
                    subPieces.Insert(0, subPiece);
                }
                else
                {
                    subPieces.Add(subPiece);
                }
                
                if (i < pieceCoords.Length - 1) {
                    newTMP |= pieceCoords[i + 1].y != v.y;
                    newTMP |= pieceCoords[i + 1].y == v.y && pieceCoords[i + 1].x - v.x > subPieceImage.rectTransform.rect.width * 1.5f;
                }
                else
                {
                    newTMP = true;
                }

                if (newTMP)
                {
                    GameObject h = FakeInstantiate.Instantiate(subPiece.transform);
                    TextMeshProUGUI headline = h.AddComponent<TextMeshProUGUI>();

                    if (GameManager.Instance.GetRound() == 0)
                    {
                        headline.text = "<font=cour SDF>" + "Drag it to mold";
                    }
                    else
                    {
                        headline.text = "<font=cour SDF>" + newsData.biases[0].headline;    
                    }
                    
                    headline.color = new Color(0.8f, .8f, .8f);
                    headline.fontSize = 9.5f;
                    headline.rectTransform.anchorMax = Vector2.one;
                    headline.rectTransform.anchorMin = Vector2.zero;
                    headline.rectTransform.pivot = new Vector2(.5f, .5f);
                    headline.rectTransform.anchoredPosition = Vector2.zero;
                    headline.rectTransform.sizeDelta = Vector2.zero;
                    headline.rectTransform.offsetMax = new Vector2(-2 + textLength * subPieceImage.rectTransform.rect.width, -2);
                    headline.rectTransform.offsetMin = new Vector2(2, 2);
                    headline.overflowMode = TextOverflowModes.Linked;
                    subPieceTexts.Add(headline);

                    newTMP = false;
                    textLength = 0;
                }
                else
                {
                    textLength++;
                }
            }

            /*int pivotIndex = PieceData.pivotIndex[(int)newsData.type];
            var pivotText = subPieceTexts.First();
            subPieceTexts.RemoveAt(0);
            subPieceTexts.Insert(pivotIndex, pivotText);*/
            for (int i = 1; i < subPieceTexts.Count; i++)
            {
                subPieceTexts[i].linkedTextComponent = subPieceTexts[i - 1];
            }

            newsHeadlinePiece.SetSubPieces(subPieces.ToArray());

            return newsHeadlinePiece;
        }
    }
}
