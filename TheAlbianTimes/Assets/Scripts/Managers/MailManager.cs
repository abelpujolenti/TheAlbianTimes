using System;
using System.Collections.Generic;
using System.IO;
using Mail;
using Mail.Content;
using UnityEngine;

namespace Managers
{
    public class MailManager : MonoBehaviour
    {
        private static MailManager _instance;
        
        public static MailManager Instance => _instance;
        
        private const string PATH_ENVELOPES_CONTAINER = "/Json/Mail/EnvelopesContainer.json";
        private const string PATH_ADS_CONTAINER = "/Json/Mail/AdsContainer.json";
        private const string PATH_BIASES_CONTAINER = "/Json/Mail/BiasesContainer.json";
        private const string PATH_BRIBES_CONTAINER = "/Json/Mail/BribesContainer.json";
        private const string PATH_LETTERS_CONTAINER = "/Json/Mail/LettersContainer.json";

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

        #region SaveToJson

        public void SaveEnvelopesToJson(GameObject[] envelopes, GameObject[] envelopesContent, bool async = false)
        {
            EnvelopesContainer envelopesContainer = new EnvelopesContainer
            {
                contentEnvelopes = new EnvelopeData[envelopes.Length]
            };

            Envelope envelope;
            
            for (int i = 0; i < envelopesContainer.contentEnvelopes.Length; i++)
            {
                envelope = envelopes[i].GetComponent<Envelope>();

                envelopesContainer.contentEnvelopes[i] = new EnvelopeData
                {
                    jointId = envelope.GetJointId(),
                    envelopeContentType = envelope.GetEnvelopeContentType()
                };
            }
            
            SaveBaseContentToJson(envelopesContainer, envelopesContainer.GetContainerPath(), async);
            
            SaveEnvelopeContentToJson(envelopesContent, async);
        }

        private void SaveEnvelopeContentToJson(GameObject[] envelopesContent, bool async = false)
        {
            List<Ad> ads = new List<Ad>();
            List<Bias> biases = new List<Bias>();
            List<Bribe> bribes = new List<Bribe>();
            List<Letter> letters = new List<Letter>();
            
            foreach (GameObject envelopeContent in envelopesContent)
            {
                switch (envelopeContent.GetComponent<EnvelopeContent>().GetEnvelopeContentType())
                {
                    case EnvelopeContentType.AD:
                        ads.Add(envelopeContent.GetComponent<Ad>());
                        break;
                    
                    case EnvelopeContentType.BIAS:
                        biases.Add(envelopeContent.GetComponent<Bias>());
                        break;
                    
                    case EnvelopeContentType.BRIBE:
                        bribes.Add(envelopeContent.GetComponent<Bribe>());
                        break;
                    
                    case EnvelopeContentType.LETTER:
                        letters.Add(envelopeContent.GetComponent<Letter>());
                        break;
                }
            }

            if (ads.Count != 0)
            {
                SaveAdsToJson(ads.ToArray(), async);
            }

            if (biases.Count != 0)
            {
                SaveBiasesToJson(biases.ToArray(), async);
            }

            if (bribes.Count != 0)
            {
                SaveBribesToJson(bribes.ToArray(), async);
            }

            if (letters.Count != 0)
            {
                SaveLettersToJson(letters.ToArray(), async);
            }
            
        }

        private void SaveAdsToJson(Ad[] ads, bool async)
        {
            AdsContainer adsContainer = new AdsContainer
            {
                contentAds = new ContentAd[ads.Length]
            };

            for (int i = 0; i < adsContainer.contentAds.Length; i++)
            {
                adsContainer.contentAds[i] = new ContentAd
                {
                    jointId = ads[i].GetJointId()
                };
            }
            
            SaveBaseContentToJson(adsContainer, adsContainer.GetContainerPath(), async);
        }

        private void SaveBiasesToJson(Bias[] biases, bool async)
        {
            BiasesContainer biasesContainer = new BiasesContainer
            {
                contentBiases = new ContentBias[biases.Length]
            };

            for (int i = 0; i < biasesContainer.contentBiases.Length; i++)
            {
                biasesContainer.contentBiases[i] = new ContentBias
                {
                    jointId = biases[i].GetJointId()
                };
            }
            
            SaveBaseContentToJson(biasesContainer, biasesContainer.GetContainerPath(), async);
        }

        private void SaveBribesToJson(Bribe[] bribes, bool async)
        {
            BribesContainer bribesContainer = new BribesContainer
            {
                contentBribes = new ContentBribe[bribes.Length]
            };

            for (int i = 0; i < bribesContainer.contentBribes.Length; i++)
            {
                bribesContainer.contentBribes[i] = new ContentBribe
                {
                    jointId = bribes[i].GetJointId(),
                    totalMoney = bribes[i].GetTotalMoney()
                };
            }
            
            SaveBaseContentToJson(bribesContainer, bribesContainer.GetContainerPath(), async);
        }

        private void SaveLettersToJson(Letter[] letter, bool async)
        {
            LettersContainer lettersContainer = new LettersContainer
            {
                contentLetters = new ContentLetter[letter.Length]
            };

            for (int i = 0; i < lettersContainer.contentLetters.Length; i++)
            {
                lettersContainer.contentLetters[i] = new ContentLetter
                {
                    jointId = letter[i].GetJointId(),
                    letterText = letter[i].GetLetterText()
                };
            }
            
            SaveBaseContentToJson(lettersContainer, lettersContainer.GetContainerPath(), async);
        }

        private void SaveBaseContentToJson <TContainer> (TContainer container, string path, bool async)
        {
            string json = JsonUtility.ToJson(container);
            path = Application.streamingAssetsPath + path;
            
            SaveToJson(json, path, async);
        }

        private void SaveToJson(string json, string path, bool async)
        {
            if (async)
            {
                File.WriteAllTextAsync(path, json);
            }
            else
            {
                File.WriteAllText(path, json);    
            }
        }

        #endregion

        #region LoadFromJson

        private void LoadEnvelopesContentIntoList(List<GameObject> envelopesContentList,
            GameObject[] envelopesContentArray)
        {
            if (envelopesContentArray == null)
            {
                return;
            }
            
            foreach (GameObject envelopeContent in envelopesContentArray)
            {
                envelopesContentList.Add(envelopeContent);
            }
        }

        public GameObject[] LoadEnvelopesFromJson(bool async = false)
        {
            GameObject[] envelopes = InstantiateEnvelopes();
            
            GameObject[] ads = LoadAdsFromJson(true, async);
            GameObject[] biases = LoadBiasesFromJson(true, async);
            GameObject[] bribes = LoadBribesFromJson(true, async);
            GameObject[] letters = LoadLettersFromJson(true, async);
            
            FindContentByJointId(envelopes, ads);
            FindContentByJointId(envelopes, biases);
            FindContentByJointId(envelopes, bribes);
            FindContentByJointId(envelopes, letters);

            EnvelopesContainer envelopesContainer = new EnvelopesContainer
            {
                contentEnvelopes = Array.Empty<EnvelopeData>()
            };
            
            SaveBaseContentToJson(envelopesContainer, envelopesContainer.GetContainerPath(), async);
            
            return envelopes;
        }

        private GameObject[] LoadAdsFromJson(bool fromEnvelope, bool async)
        {
            string json = LoadFromJson(PATH_ADS_CONTAINER);

            if (json == null)
            {
                return null;
            }

            AdsContainer adsContainer = JsonUtility.FromJson<AdsContainer>(json);

            List<GameObject> ads = new List<GameObject>();

            List<ContentAd> contentAds = new List<ContentAd>();

            GameObject adGameObject;

            Ad adComponent;

            ContentAd contentAd;

            for (int i = 0; i < adsContainer.contentAds.Length; i++)
            {
                contentAd = adsContainer.contentAds[i];

                if (contentAd.jointId == 0 && fromEnvelope)
                {
                    continue;
                }
                
                adGameObject = Instantiate(_envelopeContents[0]);
                adComponent = adGameObject.GetComponent<Ad>();
                adComponent.SetJointId(contentAd.jointId);
                adComponent.SetEnvelopeContentType(EnvelopeContentType.AD);

                ads.Add(adGameObject);
            }

            adsContainer.contentAds = contentAds.ToArray();
            
            SaveBaseContentToJson(adsContainer, adsContainer.GetContainerPath(), async);

            return ads.ToArray();
        }

        private GameObject[] LoadBiasesFromJson(bool fromEnvelope, bool async)
        {
            string json = LoadFromJson(PATH_BIASES_CONTAINER);

            if (json == null)
            {
                return null;
            }

            BiasesContainer biasesContainer = JsonUtility.FromJson<BiasesContainer>(json);

            List<GameObject> biases = new List<GameObject>();

            List<ContentBias> contentBiases = new List<ContentBias>();

            GameObject biasGameObject;

            Bias biasComponent;

            ContentBias contentBias;

            for (int i = 0; i < biasesContainer.contentBiases.Length; i++)
            {
                contentBias = biasesContainer.contentBiases[i];

                if (contentBias.jointId == 0 && fromEnvelope)
                {
                    continue;
                }
                
                biasGameObject = Instantiate(_envelopeContents[0]);
                biasComponent = biasGameObject.GetComponent<Bias>();
                biasComponent.SetJointId(contentBias.jointId);
                biasComponent.SetEnvelopeContentType(EnvelopeContentType.BIAS);
                
                biases.Add(biasGameObject);
            }

            biasesContainer.contentBiases = contentBiases.ToArray();
            
            SaveBaseContentToJson(biasesContainer, biasesContainer.GetContainerPath(), async);

            return biases.ToArray();
        }

        private GameObject[] LoadBribesFromJson(bool fromEnvelope, bool async)
        {
            string json = LoadFromJson(PATH_BRIBES_CONTAINER);

            if (json == null)
            {
                return null;
            }

            BribesContainer bribesContainer = JsonUtility.FromJson<BribesContainer>(json);

            List<GameObject> bribes = new List<GameObject>();

            List<ContentBribe> contentBribes = new List<ContentBribe>();

            GameObject bribeGameObject;

            Bribe bribeComponent;

            ContentBribe contentBribe;

            for (int i = 0; i < bribesContainer.contentBribes.Length; i++)
            {
                contentBribe = bribesContainer.contentBribes[i];
                
                if (contentBribe.jointId == 0 && fromEnvelope)
                {
                    contentBribes.Add(contentBribe);
                    continue;
                }
                
                bribeGameObject = Instantiate(_envelopeContents[0]);
                bribeComponent = bribeGameObject.GetComponent<Bribe>();
                bribeComponent.SetJointId(contentBribe.jointId);
                bribeComponent.SetTotalMoney(contentBribe.totalMoney);
                bribeComponent.SetEnvelopeContentType(EnvelopeContentType.BRIBE);

                if (!fromEnvelope)
                {
                    bribeGameObject.transform.SetParent(_envelopesContainer.transform);
                }

                bribes.Add(bribeGameObject);
            }

            bribesContainer.contentBribes = contentBribes.ToArray();
            
            SaveBaseContentToJson(bribesContainer, bribesContainer.GetContainerPath(), async);

            return bribes.ToArray();
        }

        private GameObject[] LoadLettersFromJson(bool fromEnvelope, bool async)
        {
            string json = LoadFromJson(PATH_LETTERS_CONTAINER);

            if (json == null)
            {
                return null;
            }

            LettersContainer lettersContainer = JsonUtility.FromJson<LettersContainer>(json);

            List<GameObject> letters = new List<GameObject>();

            List<ContentLetter> contentLetters = new List<ContentLetter>();

            GameObject letterGameObject;

            Letter letterComponent;

            ContentLetter contentLetter;

            for (int i = 0; i < lettersContainer.contentLetters.Length; i++)
            {
                contentLetter = lettersContainer.contentLetters[i];

                if (contentLetter.jointId == 0 && fromEnvelope)
                {
                    continue;
                }
                
                letterGameObject = Instantiate(_envelopeContents[0]);
                letterComponent = letterGameObject.GetComponent<Letter>();
                letterComponent.SetJointId(contentLetter.jointId);
                letterComponent.SetLetterText(contentLetter.letterText);
                letterComponent.SetEnvelopeContentType(EnvelopeContentType.LETTER);

                letters.Add(letterGameObject);
            }

            lettersContainer.contentLetters = contentLetters.ToArray();
            
            SaveBaseContentToJson(lettersContainer, lettersContainer.GetContainerPath(), async);

            return letters.ToArray();
        }

        private GameObject[] InstantiateEnvelopes()
        {
            string json = LoadFromJson(PATH_ENVELOPES_CONTAINER);

            if (json == null)
            {
                return null;
            }

            EnvelopesContainer envelopesContainer = JsonUtility.FromJson<EnvelopesContainer>(json);
            
            GameObject[] envelopes = new GameObject[envelopesContainer.contentEnvelopes.Length];

            Envelope envelopeComponent;

            EnvelopeData envelopeData;

            for (int i = 0; i < envelopesContainer.contentEnvelopes.Length; i++)
            {
                envelopeData = envelopesContainer.contentEnvelopes[i];

                GameObject envelopeGameObject = Instantiate(_envelope, _envelopesContainer);

                envelopeComponent = envelopeGameObject.GetComponent<Envelope>();

                envelopeComponent.SetJointId(envelopeData.jointId);
                envelopeComponent.SetEnvelopeContentType(envelopeData.envelopeContentType);
                
                envelopes[i] = envelopeGameObject;
            }

            return envelopes;
        }

        private void FindContentByJointId(GameObject[] envelopes, GameObject[] envelopesContent)
        {
            Envelope envelope;

            EnvelopeContent envelopeContent;

            for (int i = 0; i < envelopes.Length; i++)
            {
                envelope = envelopes[i].GetComponent<Envelope>();

                for (int j = 0; j < envelopesContent.Length; j++)
                {
                    envelopeContent = envelopesContent[j].GetComponent<EnvelopeContent>();

                    if (envelopeContent.GetJointId() == 0 || envelopeContent.GetJointId() != envelope.GetJointId())
                    {
                        continue;   
                    }
                    
                    envelopesContent[j].SetActive(false);
                    envelopesContent[j].transform.SetParent(envelopes[i].transform);
                    
                    envelope.SetEnvelopeContent(envelopesContent[j]);
                    break;
                }
            }
        }

        public GameObject[] LoadEnvelopesContentFromJson(bool async = false)
        {
            List<GameObject> envelopesContent = new List<GameObject>();
            
            GameObject[] ads = LoadAdsFromJson(false, async);
            GameObject[] biases = LoadBiasesFromJson(false, async);
            GameObject[] bribes = LoadBribesFromJson(false, async);
            GameObject[] letters = LoadLettersFromJson(false, async);

            if (ads.Length != 0)
            {
                LoadEnvelopesContentIntoList(envelopesContent, ads);
                AdsContainer adsContainer = new AdsContainer
                {
                    contentAds = Array.Empty<ContentAd>()
                };
                SaveBaseContentToJson(adsContainer, adsContainer.GetContainerPath(), async);    
            }

            if (biases.Length != 0)
            {
                LoadEnvelopesContentIntoList(envelopesContent, biases);
                BiasesContainer biasesContainer = new BiasesContainer
                {
                    contentBiases = Array.Empty<ContentBias>()
                };
                SaveBaseContentToJson(biasesContainer, biasesContainer.GetContainerPath(), async);
            }

            if (bribes.Length != 0)
            {
                LoadEnvelopesContentIntoList(envelopesContent, bribes);
                BribesContainer bribesContainer = new BribesContainer
                {
                    contentBribes = Array.Empty<ContentBribe>() 
                };
                SaveBaseContentToJson(bribesContainer, bribesContainer.GetContainerPath(), async);
            }

            if (letters.Length != 0)
            {
                LoadEnvelopesContentIntoList(envelopesContent, letters);
                LettersContainer lettersContainer = new LettersContainer
                {
                    contentLetters = Array.Empty<ContentLetter>()
                };
                SaveBaseContentToJson(lettersContainer, lettersContainer.GetContainerPath(), async);
            }

            return envelopesContent.ToArray();
        }

        private string LoadFromJson(string path)
        {
            return !File.Exists(Application.streamingAssetsPath + path) ? null : FileManager.LoadJsonFile(PATH_LETTERS_CONTAINER);
        }

        #endregion

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
