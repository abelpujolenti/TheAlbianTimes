using System.Collections;
using Countries;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Stats
{
    public class MapDisplay : MonoBehaviour
    {
        private const string MAP_UNFOLD = "Map Unfold";

        [SerializeField] private Sprite[] mapStages;
        [SerializeField] private GameObject statsRoot;
        [SerializeField] private GameObject mapRoot;
        [SerializeField] private Image mapImage;
        [SerializeField] private Image fadeImage;
        private Image[] mapFolds;
        [SerializeField] private GameObject[] statsDisplayObjects;
        [SerializeField] private TextMeshProUGUI[] _reputations;
        [SerializeField] private Image[] _statModificationIcon;
        [SerializeField] private TextMeshProUGUI loseText;
        [SerializeField] private Button restartButton;
        const int folds = 4;

        [SerializeField] private float unfoldTime = .8f;
        [SerializeField] private float unfoldStartAngle = 80f;
        [SerializeField] private float unfoldDelay = 2f;
        [SerializeField] private float foldMinBrightness = .2f;
        [SerializeField] private float showStatsTime = .3f;

        private void Start()
        {
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

        }

        private void SetMapStage()
        {
            int index;
            switch (GameManager.Instance.GetRound())
            {
                case 0:
                    index = 1;
                    statsDisplayObjects[0].SetActive(false);
                    statsDisplayObjects[1].SetActive(false);
                    statsDisplayObjects[3].SetActive(false);
                    statsDisplayObjects[4].SetActive(false);
                    statsDisplayObjects[7].SetActive(false);
                    statsDisplayObjects[8].SetActive(false);
                    break;
                case 1:
                case 2:
                case 3:
                    index = 2;
                    statsDisplayObjects[0].SetActive(false);
                    statsDisplayObjects[1].SetActive(false);
                    statsDisplayObjects[4].SetActive(false);
                    statsDisplayObjects[7].SetActive(false);
                    statsDisplayObjects[8].SetActive(false);
                    break;
                case 4:
                    index = 3;
                    statsDisplayObjects[4].SetActive(false);
                    statsDisplayObjects[7].SetActive(false);
                    statsDisplayObjects[8].SetActive(false);
                    break;
                case 5:
                    index = 4;
                    statsDisplayObjects[7].SetActive(false);
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

            AudioManager.Instance.Play2DSound(MAP_UNFOLD);

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
                Country country = GameManager.Instance.gameState.countries[i];
                float value = country.GetValueChange("reputation");
                Sprite statModificationIcon;
                if (value == 0)
                {
                    statModificationIcon = Resources.Load<Sprite>("Images/Icons/StatNeutral");
                }
                else if (value > 0)
                {
                    statModificationIcon = Resources.Load<Sprite>("Images/Icons/StatIncrease");
                }
                else
                {
                    statModificationIcon = Resources.Load<Sprite>("Images/Icons/StatDecrease");
                }

                _statModificationIcon[i].sprite = statModificationIcon;
                _statModificationIcon[i].type = Image.Type.Simple;
                _reputations[i].text = "Rep: <b>" + country.GetReputation().ToString("p0") + "</b> " + StatFormat.FormatValueChange(value);
            }
        } 

        private IEnumerator ShowStatsCoroutine(float t)
        {
            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                foreach (Image image in mapFolds)
                {
                    Color color = image.color;
                    color.a = 1f - elapsedT / t;
                    image.color = color;
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
            StartCoroutine(FinishCoroutine(1f));
        }
        private IEnumerator FinishCoroutine(float t)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.color = new Color(0f, 0f, 0f, 0f);

            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                fadeImage.color = ColorUtil.Alpha(Color.black, Mathf.Pow(elapsedT / t, 2f));

                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            fadeImage.color = Color.black;
            
            AudioManager.Instance.StopAllAudios();
            GameManager.Instance.musicAudioId = -1;

            if (GameManager.Instance.gameState.playerData.money <= 0)
            {
                Lose();
            }
            else
            {
                GameManager.Instance.LoadScene(ScenesName.DIALOGUE_SCENE);
            }
        }

        private void Lose()
        {
            loseText.gameObject.SetActive(true);
            loseText.text = "<size=30><b>The Albian Times has run out of funds.</b>\n<size=25>You and your " + GameManager.Instance.gameState.playerData.staff + " staff will need to find new jobs.";
            restartButton.gameObject.SetActive(true);
        
        }

        public void Restart()
        {
            GameManager.Instance.GetSaveManager().DeleteSaveFile();
            GameManager.Instance.sceneLoader.SetScene(ScenesName.MAIN_MENU);
        }
    }
}
 