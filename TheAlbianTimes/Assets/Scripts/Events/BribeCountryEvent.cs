using System.Collections.Generic;
using Managers;
using Workspace.Mail.Content;

public class BribeCountryEvent : CountryEvent
{
    public int bribeAmount;

    public override void Run()
    {
        base.Run();

        Dictionary<EnvelopeContentType, BaseMailContainer> mailToSend = new Dictionary<EnvelopeContentType, BaseMailContainer>();
        MailContentBribe mailContent = new MailContentBribe();
        mailContent.totalMoney = bribeAmount;
        BribesMailContainer mailContainer = new BribesMailContainer();
        mailContainer.SetContent(new MailContentBribe[] { mailContent });

        mailToSend.Add(EnvelopeContentType.BRIBE, mailContainer);
        MailManager.Instance.SendEnvelopes(mailToSend);
    }
}
