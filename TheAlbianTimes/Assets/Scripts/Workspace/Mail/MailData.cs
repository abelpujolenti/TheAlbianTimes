using System.Collections.Generic;
using UnityEngine;

namespace Workspace.Mail
{
    [CreateAssetMenu(fileName = "MailData", menuName = "MailData")]
    public class MailData : ScriptableObject
    {
        public List<GameObject> envelopes;
        public List<GameObject> envelopeContents;
    }
}
