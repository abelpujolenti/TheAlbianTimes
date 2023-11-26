using Mail;
using Mail.Content;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers
{
    public class MailManager : MonoBehaviour
    {
        private static MailManager _instance;
        
        public static MailManager Instance => _instance;
        
        private const string PATH_ENVELOPES = "Mail/Envelopes";
        private const string PATH_ADS = "Mail/Ads";
        private const string PATH_BIASES = "Mail/Biases";
        private const string PATH_BRIBES = "Mail/Bribes";
        private const string PATH_LETTERS = "Mail/Letters";

        [SerializeField] private GameObject _envelope;

        [SerializeField] private RectTransform _envelopesContainer;

        [SerializeField] private GameObject[] _envelopeContents;

        private int _totalSavedEnvelopes;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        public GameObject[] CreateEnvelopesFromJson()
        {
            string json = FileManager.LoadJsonFile(PATH_ENVELOPES);
            EnvelopesContainer envelopesContainer = JsonUtility.FromJson<EnvelopesContainer>(json);

            ContentEnvelope[] contentEnvelopes = envelopesContainer.contentEnvelopes; 

            GameObject[] envelopes = new GameObject[contentEnvelopes.Length];

            for (int i = 0; i < contentEnvelopes.Length; i++)
            {
                GameObject envelopeGameObject = Instantiate(_envelope, _envelopesContainer);
        
                Vector3 position = envelopeGameObject.transform.position;
                envelopeGameObject.transform.position = new Vector3(position.x, position.y, 0);
                
                Envelope envelopeComponent = envelopeGameObject.AddComponent<Envelope>();
                envelopeComponent.SetJointId(contentEnvelopes[i].jointId);
                envelopeComponent.SetEnvelopeContentType(contentEnvelopes[i].envelopeContentType);
                
                LoadEnvelopeContentInEnvelope(i, contentEnvelopes, envelopeGameObject, envelopeComponent);

                envelopes[i] = envelopeGameObject;
            }

            return envelopes;
        }

        private void LoadEnvelopeContentInEnvelope(int i, ContentEnvelope[] contentEnvelopes, GameObject envelopeGameObject, 
            Envelope envelopeComponent)
        {
            GameObject envelopeContent = null;
                
            switch (contentEnvelopes[i].envelopeContentType)
            {
                case EnvelopeContentType.AD:
                    envelopeContent = AddAdFromJson(contentEnvelopes[i].jointId, envelopeGameObject);
                    break;
                    
                case EnvelopeContentType.BIAS:
                    envelopeContent = AddBiasFromJson(contentEnvelopes[i].jointId, envelopeGameObject);
                    break;
                    
                case EnvelopeContentType.BRIBE:
                    envelopeContent = AddBribeFromJson(contentEnvelopes[i].jointId, envelopeGameObject);
                    break;
                    
                case EnvelopeContentType.LETTER:
                    envelopeContent = AddLetterFromJson(contentEnvelopes[i].jointId, envelopeGameObject);
                    break;
            }
                
            envelopeComponent.SetEnvelopeContent(envelopeContent);
        }

        private GameObject AddAdFromJson(int jointId, GameObject envelopeGameObject)
        {
            string json = FileManager.LoadJsonFile(PATH_ADS);
            AdsContainer adsContainer = JsonUtility.FromJson<AdsContainer>(json);

            GameObject envelopeContentGameObject = Instantiate(_envelopeContents[0], 
                envelopeGameObject.transform);
            envelopeContentGameObject.SetActive(false);
            
            foreach (ContentAd contentAd in adsContainer.contentAds)
            {
                if (contentAd.jointId != jointId)
                {
                    continue;
                }

                Ad ad = envelopeGameObject.AddComponent<Ad>();
                ad.SetJointId(jointId);
                
                break;
            }

            return envelopeContentGameObject;
        }
        
        private GameObject AddBiasFromJson(int jointId, GameObject envelopeGameObject)
        {
            string json = FileManager.LoadJsonFile(PATH_BIASES);
            BiasesContainer biasesContainer = JsonUtility.FromJson<BiasesContainer>(json);
            
            GameObject envelopeContentGameObject = Instantiate(_envelopeContents[1], 
                envelopeGameObject.transform);
            envelopeContentGameObject.SetActive(false);
            
            foreach (ContentBias contentBias in biasesContainer.contentBiases)
            {
                if (contentBias.jointId != jointId)
                {
                    continue;
                }

                Bias bias = envelopeGameObject.AddComponent<Bias>();
                bias.SetJointId(jointId);

                break;
            }
            
            return envelopeContentGameObject;
        }
        
        private GameObject AddBribeFromJson(int jointId, GameObject envelopeGameObject)
        {
            string json = FileManager.LoadJsonFile(PATH_BRIBES);
            BribesContainer bribesContainer = JsonUtility.FromJson<BribesContainer>(json);
            
            GameObject envelopeContentGameObject = Instantiate(_envelopeContents[0], 
                envelopeGameObject.transform);
            envelopeContentGameObject.SetActive(false);
            
            foreach (ContentBribe contentBribe in bribesContainer.contentBribes)
            {
                if (contentBribe.jointId != jointId)
                {
                    continue;
                }

                Bribe bribe = envelopeGameObject.AddComponent<Bribe>();
                bribe.SetJointId(jointId);
                bribe.SetTotalMoney(contentBribe.totalMoney);
                
                break;
            }
            
            return envelopeContentGameObject;
        }
        
        private GameObject AddLetterFromJson(int jointId, GameObject envelopeGameObject)
        {
            string json = FileManager.LoadJsonFile(PATH_LETTERS);
            LettersContainer lettersContainer = JsonUtility.FromJson<LettersContainer>(json);
            
            GameObject envelopeContentGameObject = Instantiate(_envelopeContents[3], 
                envelopeGameObject.transform);
            envelopeContentGameObject.SetActive(false);
            
            foreach (ContentLetter contentLetter in lettersContainer.contentLetters)
            {
                if (contentLetter.jointId != jointId)
                {
                    continue;
                }

                Letter letter = envelopeGameObject.AddComponent<Letter>();
                letter.SetJointId(jointId);
                letter.SetLetterText(contentLetter.letterText);
                
                break;
            }
            
            return envelopeContentGameObject;
        }

        public void AddToTotalSavedEnvelopes()
        {
            _totalSavedEnvelopes++;
        }

        public void SubtractToTotalSavedEnvelopes()
        {
            _totalSavedEnvelopes--;
        }

        public int GetTotalSavedEnvelopes()
        {
            return _totalSavedEnvelopes;
        }
    }
}
