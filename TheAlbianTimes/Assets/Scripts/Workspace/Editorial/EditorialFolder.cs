using System.Collections;
using Managers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Workspace.Editorial
{
    public class EditorialFolder : MonoBehaviour
    {
        private const string BOSS_POST_IT_SOUND = "Boss Post it";
        
        private Image folderImage;
        [SerializeField] Transform messagePostitPrefab;
        [SerializeField] private Transform _diegeticElementsCanvas;
        
        private float postitInitialScale = 3f;
        private float postitScaleAnimationTime = .2f;
        private float postitAppearDelay = 2.5f;
        
        void Start()
        {
            folderImage = GetComponent<Image>();
            folderImage.enabled = false;

            StartCoroutine(Reveal());
        }

        private IEnumerator Reveal()
        {
            yield return new WaitForSeconds(EditorialNewsLoader.loadDelay - 1f);
            folderImage.enabled = true;
            CreateMessagePostits();
            yield return TransformUtility.SetPositionCoroutine(transform, transform.position + new Vector3(0f, -14f, 0f), transform.position, 1f);
        }

        private void CreateMessagePostits()
        {
            string file = FileManager.LoadJsonFile("/Json/MessagePostits/postits.json");
            var postits = JsonConvert.DeserializeObject<MessagePostitData[]>(file);
            foreach (MessagePostitData postit in postits)
            {
                if (postit.round != GameManager.Instance.GetRound() || !postit.ConditionsFulfilled()) continue;
                StartCoroutine(SpawnPostit(postit));
            }
        }

        private IEnumerator SpawnPostit(MessagePostitData data)
        {
            yield return new WaitForSeconds(postitAppearDelay);
            
            MessagePostit postit = Instantiate(messagePostitPrefab, _diegeticElementsCanvas).GetComponent<MessagePostit>();
            postit.Setup(data);
            Transform postitTransform = postit.transform;
            RectTransform postitRectTransform = (RectTransform)postitTransform;
            Vector3 sizeDelta = postitRectTransform.sizeDelta;
            postitRectTransform.anchoredPosition = new Vector3(
                Random.Range(275f + sizeDelta.x / 2, 430f - sizeDelta.x / 2), 
                Random.Range(-100 + sizeDelta.y / 2, -145 - sizeDelta.y / 2), 
                transform.position.z);
            
            postit.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(-5f, 5f)));
            yield return ScaleAnimationCoroutine(postitTransform, postitScaleAnimationTime, postitInitialScale, 1);
        }
        
        private IEnumerator ScaleAnimationCoroutine(Transform transformToChange, float t, float start, float end)
        {
            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                float s = Mathf.Lerp(start, end, Mathf.Pow(elapsedT / t, 2.5f));
                transformToChange.localScale = new Vector3(s, s, s);
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            transformToChange.localScale = new Vector3(end, end, end);
            AudioManager.Instance.Play3DSound(BOSS_POST_IT_SOUND, 5, 100, transformToChange.position);
        }
    }
}
