using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublishedNewspaper : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(FlyOffAnimation());
    }

    private IEnumerator FlyOffAnimation()
    {
        yield return new WaitForSeconds(1f);

        float moveTime = 1f;
        float rotateTime = .7f;
        StartCoroutine(SetRotationCoroutine(80f, rotateTime));
        yield return new WaitForSeconds(rotateTime * 0.75f);
        yield return TransformUtility.SetPositionCoroutine(transform, transform.position, transform.position + new Vector3(0f, 0f, 1000f), moveTime);
        Finish();
    }

    private IEnumerator SetRotationCoroutine(float xRotation, float t)
    {
        float elapsedT = 0f;
        Vector3 startRotation = transform.rotation.eulerAngles * Mathf.Rad2Deg;
        while (elapsedT <= t)
        {
            float x = Mathf.LerpAngle(startRotation.x, xRotation, elapsedT / t);
            transform.rotation = Quaternion.Euler(new Vector3(x, transform.rotation.y, transform.rotation.z));
            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        transform.rotation = Quaternion.Euler(new Vector3(xRotation, transform.rotation.y, transform.rotation.z));

    }

    private void Finish()
    {
        GameManager.Instance.sceneLoader.SetScene("StatsScene");
    }
}
