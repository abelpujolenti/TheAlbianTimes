using System;
using System.Collections.Generic;
using Mail;
using Mail.Content;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;
using Random = UnityEngine.Random;

public class TESTAddMail : InteractableRectTransform
{
    [SerializeField] private GameObject[] _envelopePrefabs;

    [SerializeField] private MailData _mailData;

    [SerializeField] private GameObject[] _envelopeContents;

    [SerializeField] private RectTransform _envelopesContainer;

    private Dictionary<EnvelopeContentType, Action<EnvelopeContent>> _fillContentsDictionary;

    protected override void PointerClick(BaseEventData data)
    {
        EnvelopeContentType envelopeContentType = (EnvelopeContentType)Random.Range(0, (int)EnvelopeContentType.ENUM_SIZE);
        
        //MailManager.Instance.SendEnvelope(envelopeContentType);
    }
}
