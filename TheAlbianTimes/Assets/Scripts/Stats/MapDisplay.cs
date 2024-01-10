using System.Collections;
using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private GameObject statsRoot;
    [SerializeField] private GameObject mapRoot;
    [SerializeField] private Image mapImage;
    private Image[] mapFolds;
    private GameObject[] statsDisplayObjects;
    const int folds = 4;

    [SerializeField] private float unfoldTime = .8f;
    [SerializeField] private float unfoldStartAngle = 80f;
    [SerializeField] private float unfoldDelay = 2f;
    [SerializeField] private float foldMinBrightness = .2f;
    [SerializeField] private float showStatsTime = .3f;

    private void Start()
    {
        GameObject statsDisplayRoot = mapImage.transform.Find("Stats").gameObject;
        statsDisplayObjects = new GameObject[statsDisplayRoot.transform.childCount];
        for (int i = 0; i < statsDisplayObjects.Length; i++)
        {
            statsDisplayObjects[i] = statsDisplayRoot.transform.GetChild(i).gameObject;
        }
        mapImage.gameObject.SetActive(false);
        GenerateFolds();
        StartCoroutine(OpenMapAnimation());

        Debug.Log("Events:");
        foreach (KeyValuePair<float, CountryEvent> ev in CountryEventManager.Instance.currentEvents)
        {
            Debug.Log(ev.Value.id);
            ev.Value.Run();
        }
    }

    private void GenerateFolds()
    {
        Sprite mapSprite = mapImage.sprite;
        mapFolds = new Image[folds];
        for (int i = 0; i < folds; i++)
        {
            int spriteSizeX = mapImage.sprite.texture.width/ folds;
            int spriteSizeY = mapImage.sprite.texture.height;
            float sizeX = mapImage.rectTransform.sizeDelta.x / folds;
            float sizeY = mapImage.rectTransform.sizeDelta.y;

            GameObject fold = FakeInstantiate.Instantiate(mapRoot.transform);
            Image foldImage = fold.AddComponent<Image>();
            mapFolds[i] = foldImage;

            Sprite foldSprite = Sprite.Create(mapSprite.texture, new Rect(i * spriteSizeX, 0f, spriteSizeX, spriteSizeY), Vector2.zero);
            foldImage.sprite = foldSprite;
            foldImage.rectTransform.sizeDelta = new Vector2(sizeX, sizeY);

            float x = sizeX * i - mapImage.rectTransform.sizeDelta.x / 2f + sizeX * ((i + 1) % 2);
            float y = -mapImage.rectTransform.sizeDelta.y / 2f;
            Vector3 foldPosition = new Vector3(x, y, 0f);

            Vector2 pivot = new Vector2((i + 1) % 2, 0);
            foldImage.rectTransform.pivot = pivot;
            fold.transform.localPosition = foldPosition;
        }
    }

    private IEnumerator OpenMapAnimation()
    {
        for (int i = 0; i < mapFolds.Length; i++)
        {
            StartCoroutine(UnfoldAnimationCoroutine(i, unfoldTime, unfoldStartAngle));
        }
        
        yield return new WaitForSeconds(unfoldDelay + unfoldTime);

        mapImage.gameObject.SetActive(true);
        UpdateStatDisplays();
        yield return ShowStatsCoroutine(showStatsTime);
        foreach (Image image in mapFolds)
        {
            image.gameObject.SetActive(false);
        }
    }

    private void UpdateStatDisplays()
    {
        for (int i = 0; i < statsDisplayObjects.Length; i++)
        {
            TextMeshProUGUI reputationText = statsDisplayObjects[i].transform.Find("reputation").GetComponent<TextMeshProUGUI>();
            Country country = GameManager.Instance.gameState.countries[i];
            reputationText.text = "Rep: <b>" + country.GetReputation().ToString("p0") + "</b> " + StatFormat.FormatValueChange(country.GetValueChange("reputation"));
        }
    } 

    private IEnumerator ShowStatsCoroutine(float t)
    {
        float elapsedT = 0f;
        while (elapsedT <= t)
        {
            foreach (Image image in mapFolds)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1f - elapsedT / t);
            }

            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
    }

    private IEnumerator UnfoldAnimationCoroutine(int i, float t, float start)
    {
        RectTransform rt = mapFolds[i].rectTransform;

        UnfoldAnimationUpdate(rt, i, t, 0f, start);

        yield return new WaitForSeconds(unfoldDelay);

        float elapsedT = 0f;
        while (elapsedT <= t)
        {
            UnfoldAnimationUpdate(rt, i, t, elapsedT, start);

            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }

        rt.rotation = Quaternion.Euler(Vector3.zero);
    }

    void UnfoldAnimationUpdate(RectTransform rt, int i, float t, float elapsedT, float start)
    {
        float sizeX = mapImage.rectTransform.sizeDelta.x / folds;
        float sizeY = mapImage.rectTransform.sizeDelta.y;

        float yRotation = Mathf.LerpAngle(start, 0f, Mathf.Pow(elapsedT / t, .7f));
        rt.rotation = Quaternion.Euler(new Vector3(0f, yRotation, 0f));

        float currSizeX = sizeX * Mathf.Cos(yRotation * Mathf.Deg2Rad);
        int index = (mapFolds.Length - 1 - i) / 2;
        float xPosition = currSizeX + currSizeX * -2 * index;
        rt.localPosition = new Vector3(xPosition, rt.localPosition.y, rt.localPosition.z);

        if (i % 2 == 1)
        {
            mapFolds[i].color = ColorUtil.SetBrightness(mapFolds[i].color, Mathf.Lerp(foldMinBrightness, 1f, Mathf.Pow(elapsedT / t, 2f)));
        }
    }
}
 