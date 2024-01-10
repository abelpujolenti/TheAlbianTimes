using Mail;
using Mail.Content;
using Managers;
using System.Collections.Generic;
using UnityEngine;

public class BribeCountryEvent : CountryEvent
{
    public int bribeAmount;

    public override void Run()
    {
        base.Run();

        Dictionary<EnvelopeContentType, BaseContainer> mailToSend = new Dictionary<EnvelopeContentType, BaseContainer>();
        ContentBribe content = new ContentBribe();
        content.totalMoney = bribeAmount;
        BribesContainer container = new BribesContainer();
        container.SetContent(new ContentBribe[] { content });

        mailToSend.Add(EnvelopeContentType.BRIBE, container);
        MailManager.Instance.SendEnvelopes(mailToSend);
        Debug.Log(bribeAmount);
    }
}
