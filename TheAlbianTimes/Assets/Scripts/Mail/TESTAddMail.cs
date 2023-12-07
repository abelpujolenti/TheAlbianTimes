using Mail;
using Mail.Content;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

public class TESTAddMail : InteractableRectTransform
{
    [SerializeField] private GameObject _envelope;

    [SerializeField] private MailData _mailData;

    [SerializeField] private GameObject[] _envelopeContents;

    [SerializeField] private RectTransform _envelopesContainer;
    
    
    protected override void PointerClick(BaseEventData data)
    {
        GameObject envelopeGameObject = CreateEnvelope();

        GameObject envelopeContentGameObject = CreateEnvelopeContent(envelopeGameObject);

        Envelope envelopeComponent = envelopeGameObject.GetComponent<Envelope>();
        
        EnvelopeContent envelopeContentComponent = envelopeContentGameObject.GetComponent<EnvelopeContent>();
        
        envelopeComponent.SetJointId(envelopeGameObject.GetInstanceID());
        envelopeComponent.SetEnvelopeContentType(envelopeContentComponent.GetEnvelopeContentType());
        envelopeComponent.SetEnvelopeContent(envelopeContentGameObject);
        
        envelopeContentComponent.SetJointId(envelopeGameObject.GetInstanceID());
        
        _mailData.envelopes.Add(envelopeGameObject);        

        EventsManager.OnAddEnvelope(envelopeGameObject);
        
        EventsManager.OnAddEnvelopeContentToList(envelopeContentGameObject, false);
    }

    private GameObject CreateEnvelope()
    {
        GameObject envelopeGameObject = Instantiate(_envelope, _envelopesContainer);
        
        Debug.Log(envelopeGameObject.GetInstanceID());
        
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
}
