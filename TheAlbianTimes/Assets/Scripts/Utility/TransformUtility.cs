using System.Collections;
using UnityEngine;

public class TransformUtility
{
    public static IEnumerator SetPositionCoroutine(Transform gameObjectToDrag, Vector3 start, Vector3 end, float t)
    {
        float elapsedT = 0f;
        while (elapsedT <= t)
        {
            Vector3 newPos = gameObjectToDrag.transform.position;
            float sst = Mathf.Pow(Mathf.Min(1f, elapsedT / t), 2);
            newPos.x = Mathf.SmoothStep(start.x, end.x, sst);
            newPos.y = Mathf.SmoothStep(start.y, end.y, sst);
            newPos.z = Mathf.SmoothStep(start.z, end.z, sst);
            gameObjectToDrag.transform.position = newPos;
            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        gameObjectToDrag.transform.position = end;
    }

    public static IEnumerator SetRotationCoroutine(Transform gameObjectToDrag, float zRotation, float t)
    {
        float elapsedT = 0f;
        Vector3 startRotation = gameObjectToDrag.transform.rotation.eulerAngles;
        while (elapsedT <= t)
        {
            float z = Mathf.LerpAngle(startRotation.z, zRotation, elapsedT / t);
            gameObjectToDrag.transform.rotation = Quaternion.Euler(new Vector3(gameObjectToDrag.transform.rotation.x, gameObjectToDrag.transform.rotation.y, z));
            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        gameObjectToDrag.transform.rotation = Quaternion.Euler(new Vector3(gameObjectToDrag.transform.rotation.x, gameObjectToDrag.transform.rotation.y, zRotation));

    }

    public static IEnumerator SetScaleCoroutine(Transform gameObjectToDrag, Vector3 end, float t)
    {
        Vector3 start = gameObjectToDrag.transform.localScale;
        float elapsedT = 0f;
        while (elapsedT <= t)
        {
            Vector3 newScale = gameObjectToDrag.transform.localScale;
            float sst = Mathf.Pow(Mathf.Min(1f, elapsedT / t), 2);
            newScale.x = Mathf.SmoothStep(start.x, end.x, sst);
            newScale.y = Mathf.SmoothStep(start.y, end.y, sst);
            newScale.z = Mathf.SmoothStep(start.z, end.z, sst);
            gameObjectToDrag.transform.localScale = newScale;
            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        gameObjectToDrag.transform.localScale = end;
    }
}
