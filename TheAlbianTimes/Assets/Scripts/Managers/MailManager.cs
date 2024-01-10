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

        private Dictionary<EnvelopeContentType, Func<BaseContent, GameObject>> _loadEnvelopeContentDataDictionary;

        private Dictionary<EnvelopeContentType, Action<BaseContainer, EnvelopeContentType>> _sendBaseContainersDictionary;

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
                SaveBaseContentToJson(new EnvelopesContainer(), false);
            }
            
            if (!File.Exists(Application.streamingAssetsPath + PATH_ADS_CONTAINER))
            {
                SaveBaseContentToJson(new AdsContainer(), false);
            }
            
            if (!File.Exists(Application.streamingAssetsPath + PATH_BIASES_CONTAINER))
            {
                SaveBaseContentToJson(new BiasesContainer(), false);
            }
            
            if (!File.Exists(Application.streamingAssetsPath + PATH_BRIBES_CONTAINER))
            {
                SaveBaseContentToJson(new BribesContainer(), false);
            }
            
            if (!File.Exists(Application.streamingAssetsPath + PATH_LETTERS_CONTAINER))
            {
                SaveBaseContentToJson(new LettersContainer(), false);
            }
        }

        private void LoadDictionaries()
        {
            _loadEnvelopeContentDataDictionary = new Dictionary<EnvelopeContentType, Func<BaseContent, GameObject>>
            {
                { EnvelopeContentType.AD, LoadAdData },
                { EnvelopeContentType.BIAS, LoadBiasData },
                { EnvelopeContentType.BRIBE, LoadBribeData },
                { EnvelopeContentType.LETTER, LoadLetterData }
            };

            _sendBaseContainersDictionary = new Dictionary<EnvelopeContentType, Action<BaseContainer, EnvelopeContentType>>
            {
                { EnvelopeContentType.AD , SendBaseContent<AdsContainer, ContentAd>},
                { EnvelopeContentType.BIAS , SendBaseContent<BiasesContainer, ContentBias>},
                { EnvelopeContentType.BRIBE , SendBaseContent<BribesContainer, ContentBribe>},
                { EnvelopeContentType.LETTER , SendBaseContent<LettersContainer, ContentLetter>},
            };
        }

        public void SetEnvelopesContainer(RectTransform rectTransform)
        {
            _envelopesContainer = rectTransform;
        }

        #region SaveToJson

        public void SaveEnvelopesToJson(GameObject[] envelopes, GameObject[] envelopesContent, bool isAsync = false)
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
            
            SaveBaseContentToJson(envelopesContainer, isAsync);
            
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
            
            SaveBaseContentToJson(adsContainer, isAsync);
        }

        private void SaveBiasesToJson(Bias[] biases, bool isAsync)
        {
            BiasesContainer biasesContainer = new BiasesContainer
            {
                contentBiases = new ContentBias[biases.Length]
            };

            for (int i = 0; i < biasesContainer.contentBiases.Length; i++)
            {
                biasesContainer.contentBiases[i] = new ContentBias
                {
                    jointId = biases[i].GetJointId(),
                    linkId = biases[i].GetLinkId()
                };
            }
            
            SaveBaseContentToJson(biasesContainer, isAsync);
        }

        private void SaveBribesToJson(Bribe[] bribes, bool isAsync)
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
            
            SaveBaseContentToJson(bribesContainer, isAsync);
        }

        private void SaveLettersToJson(Letter[] letter, bool isAsync)
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
            
            SaveBaseContentToJson(lettersContainer, isAsync);
        }

        private void SaveBaseContentToJson <TContainer> (TContainer container, bool isAsync)
            where TContainer : BaseContainer
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

            EnvelopesContainer envelopesContainer = JsonUtility.FromJson<EnvelopesContainer>(json);

            EnvelopeData[] envelopeDatas = (EnvelopeData[])envelopesContainer.GetContent();
            
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
            
            GameObject[] ads = LoadEnvelopesContentFromJson<AdsContainer, ContentAd>(EnvelopeContentType.AD, 
                PATH_ADS_CONTAINER, true, isAsync);
            GameObject[] biases = LoadEnvelopesContentFromJson<BiasesContainer, ContentBias>(EnvelopeContentType.BIAS, 
                PATH_BIASES_CONTAINER, true, isAsync);
            GameObject[] bribes = LoadEnvelopesContentFromJson<BribesContainer, ContentBribe>(EnvelopeContentType.BRIBE, 
                PATH_BRIBES_CONTAINER, true, isAsync);
            GameObject[] letters = LoadEnvelopesContentFromJson<LettersContainer, ContentLetter>(EnvelopeContentType.LETTER, 
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
            EnvelopesContainer envelopesContainer = LoadBaseContainer<EnvelopesContainer>(PATH_ENVELOPES_CONTAINER);
            
            if (envelopesContainer == null)
            {
                return null;
            }
            
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
            
            SaveBaseContentToJson(new EnvelopesContainer(), isAsync);

            return envelopes;
        }

        private GameObject[] LoadEnvelopesContentFromJson <TContainer, TContent>
            (EnvelopeContentType envelopeContentType, string path, bool fromEnvelope, bool isAsync)
            where TContainer : BaseContainer
            where TContent : BaseContent   
        {
            TContainer container = LoadBaseContainer<TContainer>(path);
            
            if (container == null)
            {
                return null;
            }

            List<GameObject> envelopesContent;

            List<TContent> contents;

            try
            {
                TContent[] baseContents = (TContent[])container.GetContent();
                
                LoadEnvelopesContentData(baseContents, out contents, out envelopesContent, envelopeContentType, fromEnvelope);
                
                container.SetContent(contents.ToArray());
            
                SaveBaseContentToJson(container, isAsync);

                return envelopesContent.ToArray();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return null;
            }
        }

        private void LoadEnvelopesContentData <TContent> (TContent[] contentsArray, out List<TContent> contentsList, 
            out List<GameObject> envelopesContentGameObjects, EnvelopeContentType envelopeContentType, bool fromEnvelope)
            where TContent : BaseContent
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

        private GameObject LoadAdData(BaseContent content)
        {
            try
            {
                ContentAd contentAd = (ContentAd)content;
                GameObject adGameObject = Instantiate(_envelopeContents[0]);
                Ad adComponent = adGameObject.GetComponent<Ad>();
                adComponent.SetJointId(contentAd.jointId);
                adComponent.SetEnvelopeContentType(EnvelopeContentType.AD);

                return adGameObject;

            }
            catch (Exception e)
            {
                Debug.Log(e);

                return null;
            }
        }

        private GameObject LoadBiasData(BaseContent content)
        {
            try
            {
                ContentBias contentBias = (ContentBias)content;
                GameObject biasGameObject = Instantiate(_envelopeContents[1]);
                Bias biasComponent = biasGameObject.GetComponent<Bias>();
                biasComponent.SetJointId(contentBias.jointId);
                biasComponent.SetLinkId(contentBias.linkId);
                biasComponent.SetEnvelopeContentType(EnvelopeContentType.BIAS);

                return biasGameObject;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                
                return null;
            }
        }

        private GameObject LoadBribeData(BaseContent content)
        {
            try
            {
                ContentBribe contentBribe = (ContentBribe)content;
                GameObject bribeGameObject = Instantiate(_envelopeContents[2]);
                Bribe bribeComponent = bribeGameObject.GetComponent<Bribe>();
                bribeComponent.SetJointId(contentBribe.jointId);
                bribeComponent.SetTotalMoney(contentBribe.totalMoney);
                bribeComponent.SetEnvelopeContentType(EnvelopeContentType.BRIBE);
                
                return bribeGameObject;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                
                return null;
            }
        }

        private GameObject LoadLetterData(BaseContent content)
        {
            try
            {
                ContentLetter contentLetter = (ContentLetter)content;
                GameObject letterGameObject = Instantiate(_envelopeContents[3]);
                Letter letterComponent = letterGameObject.GetComponent<Letter>();
                letterComponent.SetJointId(contentLetter.jointId);
                letterComponent.SetLetterText(contentLetter.letterText);
                letterComponent.SetEnvelopeContentType(EnvelopeContentType.LETTER);

                return letterGameObject;
            }
            catch (Exception e)
            {
                Debug.Log(e);

                return null;
            }
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
            
            GameObject[] ads = LoadEnvelopesContentFromJson<AdsContainer, ContentAd>(EnvelopeContentType.AD, 
                PATH_ADS_CONTAINER, false, isAsync);
            GameObject[] biases = LoadEnvelopesContentFromJson<BiasesContainer, ContentBias>(EnvelopeContentType.BIAS, 
                PATH_BIASES_CONTAINER, false, isAsync);
            GameObject[] bribes = LoadEnvelopesContentFromJson<BribesContainer, ContentBribe>(EnvelopeContentType.BRIBE, 
                PATH_BRIBES_CONTAINER, false, isAsync);
            GameObject[] letters = LoadEnvelopesContentFromJson<LettersContainer, ContentLetter>(EnvelopeContentType.LETTER, 
                PATH_LETTERS_CONTAINER, false, isAsync);

            if (ads.Length != 0)
            {
                LoadEnvelopesContentIntoList(envelopesContent, ads);
                AdsContainer adsContainer = new AdsContainer();
                SaveBaseContentToJson(adsContainer, isAsync);    
            }

            if (biases.Length != 0)
            {
                LoadEnvelopesContentIntoList(envelopesContent, biases);
                BiasesContainer biasesContainer = new BiasesContainer();
                SaveBaseContentToJson(biasesContainer, isAsync);
            }

            if (bribes.Length != 0)
            {
                LoadEnvelopesContentIntoList(envelopesContent, bribes);
                BribesContainer bribesContainer = new BribesContainer();
                SaveBaseContentToJson(bribesContainer, isAsync);
            }

            if (letters.Length == 0)
            {
                return envelopesContent.ToArray();
            }

            LoadEnvelopesContentIntoList(envelopesContent, letters);
            LettersContainer lettersContainer = new LettersContainer();
            SaveBaseContentToJson(lettersContainer, isAsync);

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

        public void SendEnvelopes(Dictionary<EnvelopeContentType, BaseContainer> baseContainers)
        {
            foreach (KeyValuePair<EnvelopeContentType, BaseContainer> dictionaryIndex in baseContainers)
            {
                _sendBaseContainersDictionary[dictionaryIndex.Key](dictionaryIndex.Value, dictionaryIndex.Key);
            }
        }

        private void SendBaseContent<TContainer, TContent>(BaseContainer baseContainer, EnvelopeContentType envelopeContentType)
            where TContainer : BaseContainer
            where TContent : BaseContent
        {
            try
            {
                TContainer sentContainer = (TContainer)baseContainer;
                TContainer currentContainer =
                    LoadBaseContainer<TContainer>(sentContainer.GetContainerPath());

                EnvelopesContainer newEnvelopesContainer = new EnvelopesContainer();
                EnvelopesContainer currentEnvelopesContainer =
                    LoadBaseContainer<EnvelopesContainer>(newEnvelopesContainer.GetContainerPath());

                List<TContent> contentsList = new List<TContent>();
                List<EnvelopeData> envelopeContents = new List<EnvelopeData>();

                foreach (EnvelopeData envelopeData in currentEnvelopesContainer.contentEnvelopes)
                {
                    envelopeContents.Add(envelopeData);
                }

                TContent[] contentsArray = (TContent[])sentContainer.GetContent();

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

                contentsArray = (TContent[])currentContainer.GetContent();

                foreach (TContent content in contentsArray)
                {
                    contentsList.Add(content);
                }

                currentEnvelopesContainer.contentEnvelopes = envelopeContents.ToArray();
                currentContainer.SetContent(contentsList.ToArray());
                
                SaveBaseContentToJson(currentEnvelopesContainer, false);
                SaveBaseContentToJson(currentContainer, false);

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
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