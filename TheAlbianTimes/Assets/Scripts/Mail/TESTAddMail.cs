using System;
using System.Collections.Generic;
using Mail.Content;
using Managers;
using UnityEngine.EventSystems;
using Utility;

public class TESTAddMail : InteractableRectTransform
{
    protected override void PointerClick(BaseEventData data)
    {
        BribesContainer bribesContainer = new BribesContainer
        {
            contentBribes = Array.Empty<ContentBribe>()
        };

        Dictionary<EnvelopeContentType, BaseContainer> dictionary = new Dictionary<EnvelopeContentType, BaseContainer>
        {
            { EnvelopeContentType.BRIBE, bribesContainer}
        };

        ContentBribe contentBribe = new ContentBribe
        {
            totalMoney = 20
        };

        ContentBribe[] contentBribes = { contentBribe };
        
        dictionary[EnvelopeContentType.BRIBE].SetContent(contentBribes);
        
        MailManager.Instance.SendEnvelopes(dictionary);
    }
}
