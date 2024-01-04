using System;
using System.Collections.Generic;
using Mail;
using Mail.Content;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;
using Random = UnityEngine.Random;

public class SOS_RE_PUTO : InteractableRectTransform
{
    [SerializeField]private List<GameObject> _envelopes;
    [SerializeField]private List<GameObject> _envelopesContent;
    
    [SerializeField] private GameObject[] _envelopePrefabs;

    [SerializeField] private GameObject[] _envelopeContents;

    [SerializeField] private RectTransform _envelopesContainer;

    private void Start()
    {
        GameObject envelopeGameObject = CreateEnvelope((EnvelopeContentType)Random.Range(0, (int)EnvelopeContentType.AMOUNT));

        GameObject envelopeContentGameObject = CreateEnvelopeContent(envelopeGameObject);

        Envelope envelopeComponent = envelopeGameObject.GetComponent<Envelope>();
        
        EnvelopeContent envelopeContentComponent = envelopeContentGameObject.GetComponent<EnvelopeContent>();

        int newRandomJointId = CreateNewJointId();
        
        envelopeComponent.SetJointId(newRandomJointId);
        envelopeComponent.SetEnvelopeContentType(envelopeContentComponent.GetEnvelopeContentType());
        envelopeComponent.SetEnvelopeContent(envelopeContentGameObject);
        
        envelopeContentComponent.SetJointId(newRandomJointId);
        
        _envelopes.Add(envelopeGameObject);
        _envelopesContent.Add(envelopeContentGameObject);
    }

    protected override void PointerClick(BaseEventData data)
    {
        Debug.Log("Click");
        MailManager.Instance.SaveEnvelopesToJson(_envelopes.ToArray(), _envelopesContent.ToArray());
    }
    
    private GameObject CreateEnvelope(EnvelopeContentType type)
    {
        GameObject envelopeGameObject = Instantiate(_envelopePrefabs[(int)type], _envelopesContainer);
        
        Vector3 position = envelopeGameObject.transform.position;

        envelopeGameObject.transform.position = new Vector3(position.x, position.y, 0);

        return envelopeGameObject;
    }

    private GameObject CreateEnvelopeContent(GameObject envelopeGameObject)
    {
        int randomContent = Random.Range(0, _envelopeContents.Length);
        
        GameObject envelopeContentGameObject = Instantiate(_envelopeContents[Random.Range(0, _envelopeContents.Length)], 
            envelopeGameObject.transform);
        
        envelopeContentGameObject.GetComponent<EnvelopeContent>().SetEnvelopeContentType((EnvelopeContentType) 2);
        
        envelopeContentGameObject.SetActive(false);

        return envelopeContentGameObject;
    }

    private int CreateNewJointId()
    {
        int[] jointsIds = MailManager.Instance.GetJointsIds();

        bool match = false;

        int newRandomJointId;
        
        do
        {
            newRandomJointId = Random.Range(0, 100000);

            foreach (int jointId in jointsIds)
            {
                if (jointId != newRandomJointId)
                {
                    continue;
                }
                match = true;
                break;
            }
        } while (match);
        
        MailManager.Instance.AddJointId(newRandomJointId);

        return newRandomJointId;
    }
}
