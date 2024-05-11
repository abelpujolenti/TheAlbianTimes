using System.Collections;
using UnityEngine;

namespace Utility
{
    public static class TransformUtility
    {
        public static IEnumerator SetPositionCoroutine(Transform gameObjectToDrag, Vector3 start, Vector3 end, float t)
        {
            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                Vector3 newPos = gameObjectToDrag.position;
                float sst = Mathf.Pow(Mathf.Min(1f, elapsedT / t), 2);
                newPos.x = Mathf.SmoothStep(start.x, end.x, sst);
                newPos.y = Mathf.SmoothStep(start.y, end.y, sst);
                newPos.z = Mathf.SmoothStep(start.z, end.z, sst);
                gameObjectToDrag.position = newPos;
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            gameObjectToDrag.position = end;
        }
        
        public static IEnumerator SetPositionYCoroutine(Transform gameObjectToDrag, float start, float end, float t)
        {
            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                Vector3 newPos = gameObjectToDrag.position;
                float sst = Mathf.Pow(Mathf.Min(1f, elapsedT / t), 2);
                newPos.y = Mathf.SmoothStep(start, end, sst);
                gameObjectToDrag.position = newPos;
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            gameObjectToDrag.position = new Vector3(gameObjectToDrag.position.x, end, gameObjectToDrag.position.z);
        }

        public static IEnumerator SetRotationCoroutine(Transform gameObjectToDrag, float zRotation, float t)
        {
            float elapsedT = 0f;
            Vector3 startRotation = gameObjectToDrag.rotation.eulerAngles;
            while (elapsedT <= t)
            {
                float z = Mathf.LerpAngle(startRotation.z, zRotation, elapsedT / t);
                gameObjectToDrag.rotation = Quaternion.Euler(new Vector3(gameObjectToDrag.rotation.x, gameObjectToDrag.rotation.y, z));
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            gameObjectToDrag.rotation = Quaternion.Euler(new Vector3(gameObjectToDrag.rotation.x, gameObjectToDrag.rotation.y, zRotation));

        }

        public static IEnumerator SetScaleCoroutine(Transform gameObjectToDrag, Vector3 end, float t)
        {
            Vector3 start = gameObjectToDrag.localScale;
            float elapsedT = 0f;
            while (elapsedT <= t)
            {
                Vector3 newScale = gameObjectToDrag.localScale;
                float sst = Mathf.Pow(Mathf.Min(1f, elapsedT / t), 2);
                newScale.x = Mathf.SmoothStep(start.x, end.x, sst);
                newScale.y = Mathf.SmoothStep(start.y, end.y, sst);
                newScale.z = Mathf.SmoothStep(start.z, end.z, sst);
                gameObjectToDrag.localScale = newScale;
                yield return new WaitForFixedUpdate();
                elapsedT += Time.fixedDeltaTime;
            }
            gameObjectToDrag.localScale = end;
        }
    }
}
