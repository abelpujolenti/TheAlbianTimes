using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtility
{
    public static IEnumerator SetPositionCoroutine(Transform gameObjectToDrag, float start, float end, float t)
    {
        float elapsedT = 0f;
        while (elapsedT <= t)
        {
            Vector3 newPos = gameObjectToDrag.transform.position;
            newPos.x = Mathf.SmoothStep(start, end, Mathf.Pow(Mathf.Min(1f, elapsedT / t), 2));
            gameObjectToDrag.transform.position = newPos;
            yield return new WaitForFixedUpdate();
            elapsedT += Time.fixedDeltaTime;
        }
        Vector3 endPos = gameObjectToDrag.transform.position;
        endPos.x = end;
        gameObjectToDrag.transform.position = endPos;
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
}
