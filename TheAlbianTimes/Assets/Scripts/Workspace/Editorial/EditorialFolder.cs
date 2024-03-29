using System.Collections;
using Managers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Workspace.Editorial;

public class EditorialFolder : MonoBehaviour
{
    private Image folderImage;
    [SerializeField] Transform messagePostitPrefab;
    void Start()
    {
        folderImage = GetComponent<Image>();
        folderImage.enabled = false;

        StartCoroutine(Reveal());
    }

    private void CreateMessagePostits()
    {
        string file = FileManager.LoadJsonFile("/Json/MessagePostits/postits.json");
        var postits = JsonConvert.DeserializeObject<MessagePostitData[]>(file);
        foreach (MessagePostitData postit in postits)
        {
            if (postit.round != GameManager.Instance.GetRound() || !postit.ConditionsFulfilled()) continue;
            SpawnPostit(postit);
        }
    }

    public void SpawnPostit(MessagePostitData data)
    {
        MessagePostit postit = Instantiate(messagePostitPrefab, transform).GetComponent<MessagePostit>();
        postit.Setup(data);
        ((RectTransform)postit.transform).anchoredPosition = new Vector3(Random.Range(20f, 85f), Random.Range(-5f, -90f), transform.position.z);
        postit.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, Random.Range(-5f, 5f)));
    }

    private IEnumerator Reveal()
    {
        yield return new WaitForSeconds(EditorialNewsLoader.loadDelay - 1f);
        folderImage.enabled = true;
        CreateMessagePostits();
        yield return TransformUtility.SetPositionCoroutine(transform, transform.position + new Vector3(0f, -14f, 0f), transform.position, 1f);
    }
}
