using System.Collections;
using Countries;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BalanceSheet : MonoBehaviour
{
    [SerializeField] float unrollDelay = 1.5f;
    [SerializeField] float unrollTime = 1f;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextMeshProUGUI amounts;
    [SerializeField] Image bg;
    [SerializeField] Image fold;

    void Start()
    {
        text.gameObject.SetActive(false);
        amounts.gameObject.SetActive(false);
        StartCoroutine(UnrollCoroutine(unrollTime));
    }

    private IEnumerator UnrollCoroutine(float t)
    {
        float start = fold.rectTransform.sizeDelta.y;
        float end = bg.rectTransform.sizeDelta.y;
        bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, start);
        yield return new WaitForSeconds(unrollDelay);
        float elapsedT = 0f;
        while (elapsedT < t)
        {
            float bgh = Mathf.SmoothStep(start, end, Mathf.Pow((elapsedT / t), 2f));
            bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, bgh);
            float fh = Mathf.SmoothStep(start, 0f, Mathf.Pow((elapsedT / t), 2f));
            fold.rectTransform.sizeDelta = new Vector2(fold.rectTransform.sizeDelta.x, fh);
            elapsedT += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        bg.rectTransform.sizeDelta = new Vector2(bg.rectTransform.sizeDelta.x, end);

        text.gameObject.SetActive(true);
        amounts.gameObject.SetActive(true);
        SetValues();
    }

    void SetValues ()
    {
        text.text = "";
        amounts.text = "";

        AddLine("Savings:", GameManager.Instance.prevGameState.playerData.money, 19f);
        float revenue = PlayerDataManager.Instance.CalculateRevenue();
        AddLine("Revenue:", revenue, 19f);

        float[] values = PlayerDataManager.Instance.CalculateRevenueComponents();
        for (int i = 0; i < values.Length; i++)
        {
            AddLine(Country.names[i] + ":", values[i], 16f);
        }

        float cost = PlayerDataManager.Instance.CalculateCosts(GameManager.Instance.prevGameState);
        AddLine("Costs:", -cost, 19f);
        AddLine("Total:", GameManager.Instance.gameState.playerData.money, 22f);
    }

    void AddLine(string name, float value, float size)
    {
        text.text += "<size=" + size + ">" + GetColor(value) + name + "\n";
        amounts.text += "<size=" + size + ">" + GetColor(value) + value.ToString("F2") + "$" + "\n";
    }

    string GetColor(float value)
    {
        Color color = value < 0 ? Color.red : Color.white;
        return "<color=#" + color.ToHexString() + ">";
    }
}
