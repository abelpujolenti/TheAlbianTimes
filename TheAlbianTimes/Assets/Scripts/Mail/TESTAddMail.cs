using Mail;
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
        
        _mailData.envelopes.Add(envelopeGameObject);        

        EventsManager.OnAddEnvelope(envelopeGameObject);
    }

    private GameObject CreateEnvelope()
    {
        GameObject envelopeGameObject = Instantiate(_envelope, _envelopesContainer);
        
        Vector3 position = envelopeGameObject.transform.position;

        envelopeGameObject.transform.position = new Vector3(position.x, position.y, 0);
        
        GameObject envelopeContentGameObject = CreateEnvelopeContent(envelopeGameObject);

        Envelope envelopeComponent = envelopeGameObject.GetComponent<Envelope>();

        envelopeComponent.SetEnvelopeContent(envelopeContentGameObject);

        return envelopeGameObject;
    }

    private GameObject CreateEnvelopeContent(GameObject envelopeGameObject)
    {
        GameObject envelopeContentGameObject = Instantiate(_envelopeContents[Random.Range(0, _envelopeContents.Length)], 
            envelopeGameObject.transform);
        
        envelopeContentGameObject.SetActive(false);

        return envelopeContentGameObject;
    }
}
