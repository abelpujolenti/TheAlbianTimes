using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utility
{
    public class ThrowableInteractableRectTransform : InteractableRectTransform
    {
        [SerializeField] float speed = 260f;
        [SerializeField] float minVelocity = 6f;
        [SerializeField] float maxVelocity = 30f;
        [SerializeField] float deceleration = -60f;
        [SerializeField] float maxAngularUnscaledVelocity = 1.5f;
        [SerializeField] float spinFactor = 30f;
        [SerializeField] float grabRotationSnapBackTime = .2f;
        
        protected Vector3 dragVelocity;
        
        Coroutine slideCoroutine;
        Coroutine setRotationCoroutine;

        [SerializeField] protected bool _rotate = true;
        
        protected override void Drag(BaseEventData data)
        {
            Vector3 prevPosition = transform.position;
            base.Drag(data);
            Vector3 currPosition = transform.position;
            dragVelocity = currPosition - prevPosition;
            dragVelocity = dragVelocity.magnitude > minVelocity / speed ? dragVelocity : Vector3.zero;
        }
        protected override void BeginDrag(BaseEventData data)
        {
            if (slideCoroutine != null)
            {
                StopCoroutine(slideCoroutine);
            }
            SlideToRotation(0f, grabRotationSnapBackTime);
            base.BeginDrag(data);
        }

        protected override void EndDrag(BaseEventData data)
        {
            if (setRotationCoroutine != null)
            {
                StopCoroutine(setRotationCoroutine);
            }
            if (slideCoroutine != null)
            {
                StopCoroutine(slideCoroutine);
            }
            
            if (_rotate)
            {
                slideCoroutine = StartCoroutine(SlideCoroutine());    
            }
            
            base.EndDrag(data);
        }

        protected void SlideToRotation(float rotation, float t)
        {
            if (setRotationCoroutine != null)
            {
                StopCoroutine(setRotationCoroutine);
            }
            setRotationCoroutine = StartCoroutine(SetRotationCoroutine(rotation, t));
        }

        protected virtual IEnumerator SlideCoroutine()
        {
            float slideVelocity = Mathf.Min(maxVelocity, dragVelocity.magnitude * speed);
            Vector3 direction = dragVelocity.normalized;

            float initialAngularUnscaledVelocity = Mathf.Min(maxAngularUnscaledVelocity, (1 - Vector3.Dot(_vectorOffset.normalized, direction)) * _vectorOffset.magnitude * (100f / (rectTransform.sizeDelta.x + rectTransform.sizeDelta.y)));
            Vector3 cross = Vector3.Cross(_vectorOffset.normalized, direction);
            float spinDirection = cross.z != 0 ? -cross.z / Mathf.Abs(cross.z) : 1f;

            while (slideVelocity > 0)
            {
                gameObjectToDrag.transform.position += direction * slideVelocity * Time.fixedDeltaTime;
                slideVelocity += deceleration * Time.fixedDeltaTime;

                float angularVelocity = slideVelocity * initialAngularUnscaledVelocity * spinDirection * spinFactor;
                Vector3 rotation = gameObjectToDrag.transform.rotation.eulerAngles;
                rotation.z += angularVelocity * Time.fixedDeltaTime;

                gameObjectToDrag.transform.rotation = Quaternion.Euler(rotation);

                yield return new WaitForFixedUpdate();
            }
        }
    }
}
