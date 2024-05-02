using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Workspace.Mail;
using Workspace.Mail.Content;
using Letter = Workspace.Mail.Content.Letter;
using Random = UnityEngine.Random;

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

        [SerializeField] private RectTransform _envelopesContainer;

        [SerializeField] private GameObject[] _envelopeContents;
        [SerializeField] private GameObject[] _envelopePrefabs;
        
        [SerializeField] private GameObject _envelope;

        private Dictionary<EnvelopeContentType, Func<BaseMailContent, GameObject>> _loadEnvelopeContentDataDictionary;

        private Dictionary<EnvelopeContentType, Action<BaseMailContainer, EnvelopeContentType>> _sendBaseContainersDictionary;

        private List<int> _jointsIds;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _jointsIds = new List<int>();
                LoadDictionaries();
                CheckFiles();
                LoadJointsIds();
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }

        private void CheckFiles()
        {
            if (!File.Exists(Application.streamingAssetsPath + PATH_ENVELOPES_CONTAINER))
            {
                SaveBaseContentToJson(new EnvelopesMailContainer(), false);
            }
            
            if (!File.Exists(Application.streamingAssetsPath + PATH_ADS_CONTAINER))
            {
                SaveBaseContentToJson(new AdsMailContainer(), false);
            }
            
            if (!File.Exists(Application.streamingAssetsPath + PATH_BIASES_CONTAINER))
            {
                SaveBaseContentToJson(new BiasesMailContainer(), false);
            }
            
            if (!File.Exists(Application.streamingAssetsPath + PATH_BRIBES_CONTAINER))
            {
                SaveBaseContentToJson(new BribesMailContainer(), false);
            }
            
            if (!File.Exists(Application.streamingAssetsPath + PATH_LETTERS_CONTAINER))
            {
                SaveBaseContentToJson(new LettersMailContainer(), false);
            }
        }

        private void LoadDictionaries()
        {
            _loadEnvelopeContentDataDictionary = new Dictionary<EnvelopeContentType, Func<BaseMailContent, GameObject>>
            {
                { EnvelopeContentType.AD, LoadAdData },
                { EnvelopeContentType.BIAS, LoadBiasData },
                { EnvelopeContentType.BRIBE, LoadBribeData },
                { EnvelopeContentType.LETTER, LoadLetterData }
            };

            _sendBaseContainersDictionary = new Dictionary<EnvelopeContentType, Action<BaseMailContainer, EnvelopeContentType>>
            {
                { EnvelopeContentType.AD , SendBaseContent<AdsMailContainer, MailContentAd>},
                { EnvelopeContentType.BIAS , SendBaseContent<BiasesMailContainer, MailContentBias>},
                { EnvelopeContentType.BRIBE , SendBaseContent<BribesMailContainer, MailContentBribe>},
                { EnvelopeContentType.LETTER , SendBaseContent<LettersMailContainer, MailContentLetter>},
            };
        }

        public void SetEnvelopesContainer(RectTransform rectTransform)
        {
            _envelopesContainer = rectTransform;
        }

        #region SaveToJson

        public void SaveEnvelopesToJson(GameObject[] envelopes, GameObject[] envelopesContent, bool isAsync = false)
        {
            EnvelopesMailContainer envelopesMailContainer = new EnvelopesMailContainer
            {
                contentEnvelopes = new EnvelopeData[envelopes.Length]
            };

            Envelope envelope;
            
            for (int i = 0; i < envelopesMailContainer.contentEnvelopes.Length; i++)
            {
                envelope = envelopes[i].GetComponent<Envelope>();

                envelopesMailContainer.contentEnvelopes[i] = new EnvelopeData
                {
                    country = envelope.GetCountry(),
                    jointId = envelope.GetJointId(),
                    envelopeContentType = envelope.GetEnvelopeContentType()
                };
            }
            
            SaveBaseContentToJson(envelopesMailContainer, isAsync);
            
            SaveEnvelopeContentToJson(envelopesContent, isAsync);
        }

        private void SaveEnvelopeContentToJson(GameObject[] envelopesContent, bool isAsync = false)
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
                SaveAdsToJson(ads.ToArray(), isAsync);
            }

            if (biases.Count != 0)
            {
                SaveBiasesToJson(biases.ToArray(), isAsync);
            }

            if (bribes.Count != 0)
            {
                SaveBribesToJson(bribes.ToArray(), isAsync);
            }

            if (letters.Count != 0)
            {
                SaveLettersToJson(letters.ToArray(), isAsync);
            }
            
        }

        private void SaveAdsToJson(Ad[] ads, bool isAsync)
        {
            AdsMailContainer adsMailContainer = new AdsMailContainer
            {
                contentAds = new MailContentAd[ads.Length]
            };

            for (int i = 0; i < adsMailContainer.contentAds.Length; i++)
            {
                adsMailContainer.contentAds[i] = new MailContentAd
                {
                    country = ads[i].GetCountry(),
                    jointId = ads[i].GetJointId()
                };
            }
            
            SaveBaseContentToJson(adsMailContainer, isAsync);
        }

        private void SaveBiasesToJson(Bias[] biases, bool isAsync)
        {
            BiasesMailContainer biasesMailContainer = new BiasesMailContainer
            {
                contentBiases = new MailContentBias[biases.Length]
            };

            for (int i = 0; i < biasesMailContainer.contentBiases.Length; i++)
            {
                biasesMailContainer.contentBiases[i] = new MailContentBias
                {
                    country = biases[i].GetCountry(),
                    jointId = biases[i].GetJointId(),
                    linkId = biases[i].GetLinkId()
                };
            }
            
            SaveBaseContentToJson(biasesMailContainer, isAsync);
        }

        private void SaveBribesToJson(Bribe[] bribes, bool isAsync)
        {
            BribesMailContainer bribesMailContainer = new BribesMailContainer
            {
                contentBribes = new MailContentBribe[bribes.Length]
            };

            for (int i = 0; i < bribesMailContainer.contentBribes.Length; i++)
            {
                bribesMailContainer.contentBribes[i] = new MailContentBribe
                {
                    country = bribes[i].GetCountry(),
                    jointId = bribes[i].GetJointId(),
                    totalMoney = bribes[i].GetTotalMoney()
                };
            }
            
            SaveBaseContentToJson(bribesMailContainer, isAsync);
        }

        private void SaveLettersToJson(Letter[] letter, bool isAsync)
        {
            LettersMailContainer lettersMailContainer = new LettersMailContainer
            {
                contentLetters = new MailContentLetter[letter.Length]
            };

            for (int i = 0; i < lettersMailContainer.contentLetters.Length; i++)
            {
                lettersMailContainer.contentLetters[i] = new MailContentLetter
                {
                    country = letter[i].GetCountry(),
                    jointId = letter[i].GetJointId(),
                    letterText = letter[i].GetLetterText()
                };
            }
            
            SaveBaseContentToJson(lettersMailContainer, isAsync);
        }

        private void SaveBaseContentToJson <TContainer> (TContainer container, bool isAsync)
            where TContainer : BaseMailContainer
        {
            string json = JsonUtility.ToJson(container);
            string path = Application.streamingAssetsPath + container.GetContainerPath();
            
            SaveToJson(json, path, isAsync);
        }
        

        private void SaveToJson(string json, string path, bool isAsync)
        {
            if (isAsync)
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

        private void LoadJointsIds()
        {
            string json = LoadFromJson(PATH_ENVELOPES_CONTAINER);

            if (json == null)
            {
                return;
            }

            EnvelopesMailContainer envelopesMailContainer = JsonUtility.FromJson<EnvelopesMailContainer>(json);

            EnvelopeData[] envelopeDatas = (EnvelopeData[])envelopesMailContainer.GetContent();
            
            foreach (EnvelopeData envelopeData in envelopeDatas)
            {
                _jointsIds.Add(envelopeData.jointId);
            }
        }

        private void LoadEnvelopesContentIntoList(List<GameObject> envelopesContentList,
            GameObject[] envelopesContentArray)
        {
            if (envelopesContentArray == null || envelopesContentArray.Length == 0)
            {
                return;
            }
            
            foreach (GameObject envelopeContent in envelopesContentArray)
            {
                envelopesContentList.Add(envelopeContent);
            }
        }

        public GameObject[] LoadEnvelopesFromJson(bool isAsync = false)
        {
            GameObject[] envelopes = InstantiateEnvelopes(isAsync);

            List<GameObject> envelopesContent = new List<GameObject>();
            
            GameObject[] ads = LoadEnvelopesContentFromJson<AdsMailContainer, MailContentAd>(EnvelopeContentType.AD, 
                PATH_ADS_CONTAINER, true, isAsync);
            GameObject[] biases = LoadEnvelopesContentFromJson<BiasesMailContainer, MailContentBias>(EnvelopeContentType.BIAS, 
                PATH_BIASES_CONTAINER, true, isAsync);
            GameObject[] bribes = LoadEnvelopesContentFromJson<BribesMailContainer, MailContentBribe>(EnvelopeContentType.BRIBE, 
                PATH_BRIBES_CONTAINER, true, isAsync);
            GameObject[] letters = LoadEnvelopesContentFromJson<LettersMailContainer, MailContentLetter>(EnvelopeContentType.LETTER, 
                PATH_LETTERS_CONTAINER, true, isAsync);
            
            LoadEnvelopesContentIntoList(envelopesContent, ads);
            LoadEnvelopesContentIntoList(envelopesContent, biases);
            LoadEnvelopesContentIntoList(envelopesContent, bribes);
            LoadEnvelopesContentIntoList(envelopesContent, letters);
            
            FindContentByJointId(envelopes, envelopesContent.ToArray());
            
            return envelopes;
        }

        private GameObject[] InstantiateEnvelopes(bool isAsync)
        {
            EnvelopesMailContainer envelopesMailContainer = LoadBaseContainer<EnvelopesMailContainer>(PATH_ENVELOPES_CONTAINER);
            
            if (envelopesMailContainer == null)
            {
                return null;
            }
            
            GameObject[] envelopes = new GameObject[envelopesMailContainer.contentEnvelopes.Length];

            Envelope envelopeComponent;

            EnvelopeData envelopeData;

            for (int i = 0; i < envelopesMailContainer.contentEnvelopes.Length; i++)
            {
                envelopeData = envelopesMailContainer.contentEnvelopes[i];

                GameObject envelopeGameObject = Instantiate(_envelope, _envelopesContainer);

                envelopeComponent = envelopeGameObject.GetComponent<Envelope>();

                envelopeComponent.SetCountry(envelopeData.country);
                envelopeComponent.SetJointId(envelopeData.jointId);
                envelopeComponent.SetEnvelopeContentType(envelopeData.envelopeContentType);
                
                envelopes[i] = envelopeGameObject;
            }
            
            SaveBaseContentToJson(new EnvelopesMailContainer(), isAsync);

            return envelopes;
        }

        private GameObject[] LoadEnvelopesContentFromJson <TContainer, TContent>
            (EnvelopeContentType envelopeContentType, string path, bool fromEnvelope, bool isAsync)
            where TContainer : BaseMailContainer
            where TContent : BaseMailContent   
        {
            TContainer container = LoadBaseContainer<TContainer>(path);
            
            if (container == null)
            {
                return null;
            }

            List<GameObject> envelopesContent;

            List<TContent> contents;

            TContent[] baseContents;

            try
            {
                baseContents = (TContent[])container.GetContent();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
                
            LoadEnvelopesContentData(baseContents, out contents, out envelopesContent, envelopeContentType, fromEnvelope);
                
            container.SetContent(contents.ToArray());
            
            SaveBaseContentToJson(container, isAsync);

            return envelopesContent.ToArray();
        }

        private void LoadEnvelopesContentData <TContent> (TContent[] contentsArray, out List<TContent> contentsList, 
            out List<GameObject> envelopesContentGameObjects, EnvelopeContentType envelopeContentType, bool fromEnvelope)
            where TContent : BaseMailContent
        {
            GameObject envelopeContentGameObject;

            envelopesContentGameObjects = new List<GameObject>();

            contentsList = new List<TContent>();
            
            foreach (TContent baseContent in contentsArray)
            {
                if (baseContent.jointId == 0 && fromEnvelope)
                {
                    contentsList.Add(baseContent);
                    continue;
                }

                envelopeContentGameObject = _loadEnvelopeContentDataDictionary[envelopeContentType](baseContent);

                if (envelopeContentGameObject == null)
                {
                    continue;
                }

                if (!fromEnvelope)
                {
                    envelopeContentGameObject.transform.SetParent(_envelopesContainer.transform);
                    envelopeContentGameObject.transform.localScale = Vector3.one;
                }
                
                envelopesContentGameObjects.Add(envelopeContentGameObject);
            }
            
            Array.Clear(contentsArray, 0, contentsArray.Length);
        }

        private GameObject LoadAdData(BaseMailContent mailContent)
        {
            MailContentAd mailContentAd;
            
            try
            {
                mailContentAd = (MailContentAd)mailContent;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
            
            GameObject adGameObject = Instantiate(_envelopeContents[0]);
            Ad adComponent = adGameObject.GetComponent<Ad>();
            adComponent.SetCountry(mailContentAd.country);
            adComponent.SetJointId(mailContentAd.jointId);
            adComponent.SetEnvelopeContentType(EnvelopeContentType.AD);

            return adGameObject;
        }

        private GameObject LoadBiasData(BaseMailContent mailContent)
        {
            MailContentBias mailContentBias;
            
            try
            {
                mailContentBias = (MailContentBias)mailContent;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
            
            GameObject biasGameObject = Instantiate(_envelopeContents[1]);
            Bias biasComponent = biasGameObject.GetComponent<Bias>();
            biasComponent.SetCountry(mailContentBias.country);
            biasComponent.SetJointId(mailContentBias.jointId);
            biasComponent.SetLinkId(mailContentBias.linkId);
            biasComponent.SetEnvelopeContentType(EnvelopeContentType.BIAS);

            return biasGameObject;
        }

        private GameObject LoadBribeData(BaseMailContent mailContent)
        {
            MailContentBribe mailContentBribe;
            
            try
            {
                mailContentBribe = (MailContentBribe)mailContent;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
            
            GameObject bribeGameObject = Instantiate(_envelopeContents[2]);
            Bribe bribeComponent = bribeGameObject.GetComponent<Bribe>();
            bribeComponent.SetCountry(mailContentBribe.country);
            bribeComponent.SetJointId(mailContentBribe.jointId);
            bribeComponent.SetTotalMoney(mailContentBribe.totalMoney);
            bribeComponent.SetEnvelopeContentType(EnvelopeContentType.BRIBE);
                
            return bribeGameObject;
        }

        private GameObject LoadLetterData(BaseMailContent mailContent)
        {
            MailContentLetter mailContentLetter;
            
            try
            {
                mailContentLetter = (MailContentLetter)mailContent;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
            
            GameObject letterGameObject = Instantiate(_envelopeContents[3]);
            Letter letterComponent = letterGameObject.GetComponent<Letter>();
            letterComponent.SetCountry(mailContentLetter.country);
            letterComponent.SetJointId(mailContentLetter.jointId);
            letterComponent.SetLetterText(mailContentLetter.letterText);
            letterComponent.SetEnvelopeContentType(EnvelopeContentType.LETTER);

            return letterGameObject;
        }

        private void FindContentByJointId(GameObject[] envelopes, GameObject[] envelopesContent)
        {
            Envelope envelopeComponent;

            EnvelopeContent envelopeContentComponent;

            foreach (GameObject envelopeGameObject in envelopes)
            {
                envelopeComponent = envelopeGameObject.GetComponent<Envelope>();

                foreach (GameObject envelopeContentGameObject in envelopesContent)
                {
                    envelopeContentComponent = envelopeContentGameObject.GetComponent<EnvelopeContent>();

                    if (envelopeContentComponent.GetJointId() == 0 || envelopeContentComponent.GetJointId() != envelopeComponent.GetJointId())
                    {
                        continue;   
                    }
                    
                    envelopeContentGameObject.SetActive(false);
                    envelopeContentGameObject.transform.SetParent(envelopeGameObject.transform);
                    
                    envelopeComponent.SetEnvelopeContent(envelopeContentGameObject);
                    break;
                }
            }
        }

        public GameObject[] LoadEnvelopesContentFromJson(bool isAsync = false)
        {
            List<GameObject> envelopesContent = new List<GameObject>();
            
            GameObject[] ads = LoadEnvelopesContentFromJson<AdsMailContainer, MailContentAd>(EnvelopeContentType.AD, 
                PATH_ADS_CONTAINER, false, isAsync);
            GameObject[] biases = LoadEnvelopesContentFromJson<BiasesMailContainer, MailContentBias>(EnvelopeContentType.BIAS, 
                PATH_BIASES_CONTAINER, false, isAsync);
            GameObject[] bribes = LoadEnvelopesContentFromJson<BribesMailContainer, MailContentBribe>(EnvelopeContentType.BRIBE, 
                PATH_BRIBES_CONTAINER, false, isAsync);
            GameObject[] letters = LoadEnvelopesContentFromJson<LettersMailContainer, MailContentLetter>(EnvelopeContentType.LETTER, 
                PATH_LETTERS_CONTAINER, false, isAsync);

            if (ads.Length != 0)
            {
                LoadEnvelopesContentIntoList(envelopesContent, ads);
                AdsMailContainer adsMailContainer = new AdsMailContainer();
                SaveBaseContentToJson(adsMailContainer, isAsync);    
            }

            if (biases.Length != 0)
            {
                LoadEnvelopesContentIntoList(envelopesContent, biases);
                BiasesMailContainer biasesMailContainer = new BiasesMailContainer();
                SaveBaseContentToJson(biasesMailContainer, isAsync);
            }

            if (bribes.Length != 0)
            {
                LoadEnvelopesContentIntoList(envelopesContent, bribes);
                BribesMailContainer bribesMailContainer = new BribesMailContainer();
                SaveBaseContentToJson(bribesMailContainer, isAsync);
            }

            if (letters.Length == 0)
            {
                return envelopesContent.ToArray();
            }

            LoadEnvelopesContentIntoList(envelopesContent, letters);
            LettersMailContainer lettersMailContainer = new LettersMailContainer();
            SaveBaseContentToJson(lettersMailContainer, isAsync);

            return envelopesContent.ToArray();
        }

        private TContainer LoadBaseContainer<TContainer>(string path)
        {
            string json = LoadFromJson(path);

            if (json == null)
            {
                return default;
            }

            return JsonUtility.FromJson<TContainer>(json);
        }

        private string LoadFromJson(string path)
        {
            return !File.Exists(Application.streamingAssetsPath + path) ? null : FileManager.LoadJsonFile(path);
        }

        #endregion

        #region Send

        public void SendEnvelopes(Dictionary<EnvelopeContentType, BaseMailContainer> baseContainers)
        {
            foreach (KeyValuePair<EnvelopeContentType, BaseMailContainer> dictionaryIndex in baseContainers)
            {
                _sendBaseContainersDictionary[dictionaryIndex.Key](dictionaryIndex.Value, dictionaryIndex.Key);
            }
        }

        private void SendBaseContent<TContainer, TContent>(BaseMailContainer baseMailContainer, EnvelopeContentType envelopeContentType)
            where TContainer : BaseMailContainer
            where TContent : BaseMailContent
        {
            TContainer sentContainer;
            
            try
            {
                sentContainer = (TContainer)baseMailContainer;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }
            
            TContainer currentContainer =
                LoadBaseContainer<TContainer>(sentContainer.GetContainerPath());

            EnvelopesMailContainer newEnvelopesMailContainer = new EnvelopesMailContainer();
            EnvelopesMailContainer currentEnvelopesMailContainer =
                LoadBaseContainer<EnvelopesMailContainer>(newEnvelopesMailContainer.GetContainerPath());

            List<TContent> contentsList = new List<TContent>();
            List<EnvelopeData> envelopeContents = new List<EnvelopeData>();

            foreach (EnvelopeData envelopeData in currentEnvelopesMailContainer.contentEnvelopes)
            {
                envelopeContents.Add(envelopeData);
            }

            TContent[] contentsArray;

            try
            {
                contentsArray = (TContent[])sentContainer.GetContent();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            foreach (TContent content in contentsArray)
            {
                int jointId = CreateNewJointId();
                envelopeContents.Add(new EnvelopeData
                {
                    jointId = jointId,
                    envelopeContentType = envelopeContentType
                });
                content.jointId = jointId;
                contentsList.Add(content);
            }

            try
            {
                contentsArray = (TContent[])currentContainer.GetContent();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return;
            }

            foreach (TContent content in contentsArray)
            {
                contentsList.Add(content);
            }

            currentEnvelopesMailContainer.contentEnvelopes = envelopeContents.ToArray();
            currentContainer.SetContent(contentsList.ToArray());
                
            SaveBaseContentToJson(currentEnvelopesMailContainer, false);
            SaveBaseContentToJson(currentContainer, false);
        }

        private int CreateNewJointId()
        {
            int[] jointsIds = GetJointsIds();

            bool match = false;

            int newRandomJointId;
            
            do
            {
                newRandomJointId = Random.Range(1, 100000);

                foreach (int jointId in jointsIds)
                {
                    if (jointId != newRandomJointId)
                    {
                        continue;
                    }
                    match = true;
                    break;
                }
            } while (match);
            
            AddJointId(newRandomJointId);

            return newRandomJointId;
        }

        #endregion

        public void AddJointId(int jointId)
        {
            _jointsIds.Add(jointId);
        }

        public void SubtractJointId(int jointId)
        {
            _jointsIds.Remove(jointId);
        }

        public int[] GetJointsIds()
        {
            return _jointsIds.ToArray();
        }
    }
}