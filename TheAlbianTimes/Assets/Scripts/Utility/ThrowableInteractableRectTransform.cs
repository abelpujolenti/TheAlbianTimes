using System;
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
        
        Vector3 dragVelocity;
        
        Coroutine slideCoroutine;
        Coroutine setRotationCoroutine;

        protected bool _rotate = true;
        
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

        private IEnumerator SetRotationCoroutine(float zRotation, float t)
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

        private IEnumerator SlideCoroutine()
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

        private float Deg2Rad(float angle)
        {
            return angle * ((float)Math.PI / 180f);
        }
        
        private static Vector3 RotationX
        {
            get
            {
                Vector3 vector;
                vector.x = 1;
                vector.y = 0;
                vector.z = 0;
                return vector;
            }
        }
        
        private static Vector3 RotationY
        {
            get
            {
                Vector3 vector;
                vector.x = 0;
                vector.y = 1;
                vector.z = 0;
                return vector;
            }
        }

        private static Vector3 RotationZ
        {
            get
            {
                Vector3 vector;
                vector.x = 0;
                vector.y = 0;
                vector.z = 1;
                return vector;
            }
        }
        
        private Quaternion Rotate(Quaternion currentRotation, Vector3 axis, float angle)
        {
            angle /= 2;

            Quaternion quaternionRotationZ = Quaternion.identity;
            quaternionRotationZ.w = (float)Math.Cos(Deg2Rad(angle) * axis.z);
            quaternionRotationZ.z = (float)Math.Sin(Deg2Rad(angle) * axis.z);

            Quaternion quaternionRotationY = Quaternion.identity;
            quaternionRotationY.w = (float)Math.Cos(Deg2Rad(angle) * axis.y);
            quaternionRotationY.y = (float)Math.Sin(Deg2Rad(angle) * axis.y);

            Quaternion quaternionRotationX = Quaternion.identity;
            quaternionRotationX.w = (float)Math.Cos(Deg2Rad(angle) * axis.x);
            quaternionRotationX.x = (float)Math.Sin(Deg2Rad(angle) * axis.x);

            Quaternion result = quaternionRotationZ * quaternionRotationX * quaternionRotationY;

            return currentRotation * result;
        }
    }
}
