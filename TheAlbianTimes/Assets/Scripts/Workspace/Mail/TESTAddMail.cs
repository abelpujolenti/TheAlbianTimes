using System;
using System.Collections.Generic;
using Managers;
using UnityEngine.EventSystems;
using Utility;
using Workspace.Mail.Content;

public class TESTAddMail : InteractableRectTransform
{
    protected override void PointerClick(BaseEventData data)
    {
        BribesMailContainer bribesMailContainer = new BribesMailContainer
        {
            contentBribes = Array.Empty<MailContentBribe>()
        };

        Dictionary<EnvelopeContentType, BaseMailContainer> dictionary = new Dictionary<EnvelopeContentType, BaseMailContainer>
        {
            { EnvelopeContentType.BRIBE, bribesMailContainer}
        };

        MailContentBribe mailContentBribe = new MailContentBribe
        {
            totalMoney = 20
        };

        MailContentBribe[] contentBribes = { mailContentBribe };
        
        dictionary[EnvelopeContentType.BRIBE].SetContent(contentBribes);
        
        MailManager.Instance.SendEnvelopes(dictionary);
    }
}
