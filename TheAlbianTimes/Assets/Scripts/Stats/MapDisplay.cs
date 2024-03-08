using System.Collections;
using Countries;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapDisplay : MonoBehaviour
{
    private const string MAP_UNFOLD = "Map Unfold";

    [SerializeField] private Sprite[] mapStages;
    [SerializeField] private GameObject statsRoot;
    [SerializeField] private GameObject mapRoot;
    [SerializeField] private Image mapImage;
    [SerializeField] private Image fadeImage;
    private Image[] mapFolds;
    private GameObject[] statsDisplayObjects;
    const int folds = 4;

    [SerializeField] private float unfoldTime = .8f;
    [SerializeField] private float unfoldStartAngle = 80f;
    [SerializeField] private float unfoldDelay = 2f;
    [SerializeField] private float foldMinBrightness = .2f;
    [SerializeField] private float showStatsTime = .3f;

    private AudioSource _audioSourceMapUnfold;

    private void Start()
    {
        GameObject statsDisplayRoot = mapImage.transform.Find("Stats").gameObject;
        statsDisplayObjects = new GameObject[statsDisplayRoot.transform.childCount];
        for (int i = 0; i < statsDisplayObjects.Length; i++)
        {
            statsDisplayObjects[i] = statsDisplayRoot.transform.GetChild(i).gameObject;
        }

        SetMapStage();

        mapImage.gameObject.SetActive(false);

        GenerateFolds();
        StartCoroutine(OpenMapAnimation());

        /*Debug.Log("Events:");
        foreach (KeyValuePair<float, CountryEvent> ev in CountryEventManager.Instance.currentEvents)
        {
            Debug.Log(ev.Value.id);
            ev.Value.Run();
        }*/

        _audioSourceMapUnfold = gameObject.AddComponent<AudioSource>();
        (AudioSource, string)[] tuples = {
                (_audioSourceMapUnfold, MAP_UNFOLD)
            };

        SoundManager.Instance.SetMultipleAudioSourcesComponents(tuples);

    }

    private void SetMapStage()
    {
        int index;
        switch (GameManager.Instance.GetRound())
        {
            case 0:
            case 1:
            case 2:
                index = 1;
                statsDisplayObjects[0].SetActive(false);
                statsDisplayObjects[1].SetActive(false);
                statsDisplayObjects[3].SetActive(false);
                statsDisplayObjects[4].SetActive(false);
                statsDisplayObjects[7].SetActive(false);
                statsDisplayObjects[8].SetActive(false);
                break;
            case 3:
            case 4:
            case 5:
                index = 2;
                statsDisplayObjects[0].SetActive(false);
                statsDisplayObjects[1].SetActive(false);
                statsDisplayObjects[4].SetActive(false);
                statsDisplayObjects[7].SetActive(false);
                statsDisplayObjects[8].SetActive(false);
                break;
            case 6:
                index = 3;
                statsDisplayObjects[4].SetActive(false);
                statsDisplayObjects[7].SetActive(false);
                statsDisplayObjects[8].SetActive(false);
                break;
            case 7:
                index = 4;
                statsDisplayObjects[4].SetActive(false);
                break;
            default:
                index = 0;
                break;
        }
        mapImage.sprite = mapStages[index];
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
            UnfoldAnimationUpdate(mapFolds[i].rectTransform, i, unfoldTime, 0, unfoldStartAngle);
        }

        yield return new WaitForSeconds(unfoldDelay);

        //Sound

        for (int i = 0; i < mapFolds.Length; i++)
        {
            StartCoroutine(UnfoldAnimationCoroutine(i, unfoldTime, unfoldStartAngle));
        }
        yield return new WaitForSeconds(unfoldTime);

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
            if (!statsDisplayObjects[i].activeSelf) continue;
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

    public void Finish()
    {
        GameManager.Instance.sceneLoader.SetScene("DialogueScene");
    }
}
 