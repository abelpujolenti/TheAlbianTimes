using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        
    }

    public void SpawnPostit(string text)
    {
        MessagePostit postit = Instantiate(messagePostitPrefab, transform).GetComponent<MessagePostit>();
        postit.SetText(text);
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
