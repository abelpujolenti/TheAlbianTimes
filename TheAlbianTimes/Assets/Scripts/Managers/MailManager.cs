using System;
using System.Collections.Generic;
using Mail;
using Mail.Content;
using UnityEngine;
using System.IO;

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

        // Start is called before the first frame update
        void Start()
        {
        
        }

        #region SaveToJson

        public void SaveEnvelopesToJson(GameObject[] envelopes, GameObject[] envelopesContent, bool async = false)
        {
            EnvelopesContainer envelopesContainer = new EnvelopesContainer();

            envelopesContainer.contentEnvelopes = new ContentEnvelope[envelopes.Length];

            ContentEnvelope contentEnvelope;

            Envelope envelope;
            
            for (int i = 0; i < envelopesContainer.contentEnvelopes.Length; i++)
            {
                envelope = envelopes[i].GetComponent<Envelope>();

                contentEnvelope.jointId = envelope.GetJointId();
                contentEnvelope.envelopeContentType = envelope.GetEnvelopeContentType();

                envelopesContainer.contentEnvelopes[i] = contentEnvelope;
            }

            string json = JsonUtility.ToJson(envelopesContainer);
            string path = Application.streamingAssetsPath + PATH_ENVELOPES_CONTAINER;
            
            if (async)
            {
                File.WriteAllTextAsync(path, json);
            }
            else
            {
                File.WriteAllText(path, json);    
            }
            
            SaveEnvelopeContentToJson(envelopesContent, async);
            
        }

        public void SaveEnvelopeContentToJson(GameObject[] envelopesContent, bool async = false)
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

        public void SaveAdsToJson(Ad[] ads, bool async)
        {
            AdsContainer adsContainer = new AdsContainer();

            adsContainer.contentAds = new ContentAd[ads.Length];

            ContentAd contentAd = new ContentAd();

            for (int i = 0; i < adsContainer.contentAds.Length; i++)
            {
                contentAd.jointId = ads[i].GetJointId();
                
                adsContainer.contentAds[i] = contentAd;
            }

            string json = JsonUtility.ToJson(adsContainer);
            string path = Application.streamingAssetsPath + PATH_ADS_CONTAINER;
            
            if (async)
            {
                File.WriteAllTextAsync(path, json);
            }
            else
            {
                File.WriteAllText(path, json);    
            }
        }

        public void SaveBiasesToJson(Bias[] biases, bool async)
        {
            BiasesContainer biasesContainer = new BiasesContainer();

            biasesContainer.contentBiases = new ContentBias[biases.Length];

            ContentBias contentBias;

            for (int i = 0; i < biasesContainer.contentBiases.Length; i++)
            {
                contentBias.jointId = biases[i].GetJointId();

                biasesContainer.contentBiases[i] = contentBias;
            }
            
            string json = JsonUtility.ToJson(biasesContainer);
            string path = Application.streamingAssetsPath + PATH_BIASES_CONTAINER;
            
            if (async)
            {
                File.WriteAllTextAsync(path, json);
            }
            else
            {
                File.WriteAllText(path, json);    
            }
        }

        public void SaveBribesToJson(Bribe[] bribes, bool async)
        {
            BribesContainer bribesContainer = new BribesContainer();

            bribesContainer.contentBribes = new ContentBribe[bribes.Length];

            ContentBribe contentBribe;

            for (int i = 0; i < bribesContainer.contentBribes.Length; i++)
            {
                contentBribe.jointId = bribes[i].GetJointId();
                contentBribe.totalMoney = bribes[i].GetTotalMoney();

                bribesContainer.contentBribes[i] = contentBribe;
            }
            
            string json = JsonUtility.ToJson(bribesContainer);
            string path = Application.streamingAssetsPath + PATH_BRIBES_CONTAINER;
            
            if (async)
            {
                File.WriteAllTextAsync(path, json);
            }
            else
            {
                File.WriteAllText(path, json);    
            }   
        }

        public void SaveLettersToJson(Letter[] letter, bool async)
        {
            LettersContainer lettersContainer = new LettersContainer();

            lettersContainer.contentLetters = new ContentLetter[letter.Length];

            ContentLetter contentLetter;

            for (int i = 0; i < lettersContainer.contentLetters.Length; i++)
            {
                contentLetter.jointId = letter[i].GetJointId();
                contentLetter.letterText = letter[i].GetLetterText();

                lettersContainer.contentLetters[i] = contentLetter;
            }
            
            string json;
            string path;

            json = JsonUtility.ToJson(lettersContainer);
            path = Application.streamingAssetsPath + PATH_LETTERS_CONTAINER;
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
            List<GameObject> envelopesContent = new List<GameObject>();
            
            GameObject[] ads = LoadAdsFromJson();
            GameObject[] biases = LoadBiasesFromJson();
            GameObject[] bribes = LoadBribesFromJson();
            GameObject[] letters = LoadLettersFromJson();
            
            LoadEnvelopesContentIntoList(envelopesContent, ads);
            LoadEnvelopesContentIntoList(envelopesContent, biases);
            LoadEnvelopesContentIntoList(envelopesContent, bribes);
            LoadEnvelopesContentIntoList(envelopesContent, letters);
            
            GameObject[] envelopes = LookForEnvelopes(envelopesContent.ToArray());
            
            SaveAdsToJsonByGivenEnvelopes(envelopes, async);
            SaveBiasesToJsonByGivenEnvelopes(envelopes, async);
            SaveBribesToJsonByGivenEnvelopes(envelopes, async);
            SaveLettersToJsonByGivenEnvelopes(envelopes, async);
            UnloadEnvelopesFromJson(async);
            
            return envelopes;
        }

        private GameObject[] LoadAdsFromJson()
        {
            if (!File.Exists(Application.streamingAssetsPath + PATH_ADS_CONTAINER))
            {
                return null;
            }
            
            string json = FileManager.LoadJsonFile(PATH_ADS_CONTAINER);

            if (json == "{}" || json == "{\r\n}")
            {
                return null;
            }

            AdsContainer adsContainer = JsonUtility.FromJson<AdsContainer>(json);

            GameObject[] ads = new GameObject[adsContainer.contentAds.Length];

            GameObject adGameObject;

            Ad adComponent;

            ContentAd contentAd;

            for (int i = 0; i < ads.Length; i++)
            {
                contentAd = adsContainer.contentAds[i];
                
                adGameObject = Instantiate(_envelopeContents[0]);
                adComponent = adGameObject.GetComponent<Ad>();
                adComponent.SetJointId(contentAd.jointId);
                adComponent.SetEnvelopeContentType(EnvelopeContentType.AD);

                ads[i] = adGameObject;
            }

            return ads;
        }

        private GameObject[] LoadBiasesFromJson()
        {
            if (!File.Exists(Application.streamingAssetsPath + PATH_BIASES_CONTAINER))
            {
                return null;
            }
            
            string json = FileManager.LoadJsonFile(PATH_BIASES_CONTAINER);

            if (json == "{}" || json == "{\r\n}")
            {
                return null;
            }

            BiasesContainer biasesContainer = JsonUtility.FromJson<BiasesContainer>(json);

            GameObject[] biases = new GameObject[biasesContainer.contentBiases.Length];

            GameObject biasGameObject;

            Bias biasComponent;

            ContentBias contentBias;

            for (int i = 0; i < biases.Length; i++)
            {
                contentBias = biasesContainer.contentBiases[i];
                
                biasGameObject = Instantiate(_envelopeContents[0]);
                biasComponent = biasGameObject.GetComponent<Bias>();
                biasComponent.SetJointId(contentBias.jointId);
                biasComponent.SetEnvelopeContentType(EnvelopeContentType.BIAS);

                biases[i] = biasGameObject;
            }

            return biases;
        }

        private GameObject[] LoadBribesFromJson()
        {
            if (!File.Exists(Application.streamingAssetsPath + PATH_BRIBES_CONTAINER))
            {
                return null;
            }
            
            string json = FileManager.LoadJsonFile(PATH_BRIBES_CONTAINER);

            if (json == "{}" || json == "{\r\n}")
            {
                return null;
            }

            BribesContainer bribesContainer = JsonUtility.FromJson<BribesContainer>(json);

            GameObject[] bribes = new GameObject[bribesContainer.contentBribes.Length];

            GameObject bribeGameObject;

            Bribe bribeComponent;

            ContentBribe contentBribe;

            for (int i = 0; i < bribes.Length; i++)
            {
                contentBribe = bribesContainer.contentBribes[i];
                
                bribeGameObject = Instantiate(_envelopeContents[0]);
                bribeComponent = bribeGameObject.GetComponent<Bribe>();
                bribeComponent.SetJointId(contentBribe.jointId);
                bribeComponent.SetTotalMoney(contentBribe.totalMoney);
                bribeComponent.SetEnvelopeContentType(EnvelopeContentType.BRIBE);

                bribes[i] = bribeGameObject;
            }

            return bribes;
        }

        private GameObject[] LoadLettersFromJson()
        {
            if (!File.Exists(Application.streamingAssetsPath + PATH_LETTERS_CONTAINER))
            {
                return null;
            }
            
            string json = FileManager.LoadJsonFile(PATH_LETTERS_CONTAINER);

            if (json == "{}" || json == "{\r\n}")
            {
                return null;
            }

            LettersContainer lettersContainer = JsonUtility.FromJson<LettersContainer>(json);

            GameObject[] letters = new GameObject[lettersContainer.contentLetters.Length];

            GameObject letterGameObject;

            Letter letterComponent;

            ContentLetter contentLetter;

            for (int i = 0; i < letters.Length; i++)
            {
                contentLetter = lettersContainer.contentLetters[i];
                
                letterGameObject = Instantiate(_envelopeContents[0]);
                letterComponent = letterGameObject.GetComponent<Letter>();
                letterComponent.SetJointId(contentLetter.jointId);
                letterComponent.SetLetterText(contentLetter.letterText);
                letterComponent.SetEnvelopeContentType(EnvelopeContentType.LETTER);

                letters[i] = letterGameObject;
            }

            return letters;
        }

        private GameObject[] LookForEnvelopes(GameObject[] envelopesContent)
        {
            if (!File.Exists(Application.streamingAssetsPath + PATH_ENVELOPES_CONTAINER))
            {
                return null;
            }
            
            string json = FileManager.LoadJsonFile(PATH_ENVELOPES_CONTAINER);

            if (json == "{}" || json == "{\r\n}")
            {
                Debug.Log(json);
                return null;
            }

            EnvelopesContainer envelopesContainer = JsonUtility.FromJson<EnvelopesContainer>(json);
            
            GameObject[] envelopes = new GameObject[envelopesContainer.contentEnvelopes.Length];

            Envelope envelopeComponent;

            EnvelopeContent envelopeContent;

            ContentEnvelope contentEnvelope;

            for (int i = 0; i < envelopesContainer.contentEnvelopes.Length; i++)
            {
                contentEnvelope = envelopesContainer.contentEnvelopes[i];
                
                envelopeContent = envelopesContent[i].GetComponent<EnvelopeContent>();
                if (envelopeContent.GetJointId() != contentEnvelope.jointId)
                {
                    continue;
                }

                GameObject envelopeGameObject = Instantiate(_envelope, _envelopesContainer);
                
                envelopesContent[i].SetActive(false);
                envelopesContent[i].transform.SetParent(envelopeGameObject.transform);

                envelopeComponent = envelopeGameObject.GetComponent<Envelope>();

                envelopeComponent.SetJointId(contentEnvelope.jointId);
                envelopeComponent.SetEnvelopeContentType(contentEnvelope.envelopeContentType);
                envelopeComponent.SetEnvelopeContent(envelopesContent[i]);
                
                envelopes[i] = envelopeGameObject;
            }

            return envelopes;
        }
        
        private void SaveAdsToJsonByGivenEnvelopes(GameObject[] envelopes, bool async)
        {
            string json = FileManager.LoadJsonFile(PATH_ADS_CONTAINER);
            
            AdsContainer adsContainer = JsonUtility.FromJson<AdsContainer>(json);

            ContentAd[] contentAdsArray = adsContainer.contentAds;

            if (contentAdsArray == null)
            {
                return;
            }

            List<ContentAd> contentAdsList = new List<ContentAd>();

            Envelope envelope;

            ContentAd contentAd;

            for (int i = 0; i < envelopes.Length; i++)
            {
                envelope = envelopes[i].GetComponent<Envelope>();
                
                for (int j = 0; j < contentAdsArray.Length; j++)
                {
                    contentAd = contentAdsArray[i];

                    if (envelope.GetJointId() != contentAd.jointId)
                    {
                        continue;
                    }
                    contentAdsList.Add(contentAd);
                    break;
                }
            }

            if (envelopes.LongLength == 0)
            {
                return;
            }
            
            SaveAdsToJson(contentAdsList.ToArray(), async);
        }
        
        private void SaveBiasesToJsonByGivenEnvelopes(GameObject[] envelopes, bool async)
        {
            string json = FileManager.LoadJsonFile(PATH_BIASES_CONTAINER);

            BiasesContainer biasesContainer = JsonUtility.FromJson<BiasesContainer>(json);

            ContentBias[] contentBiasesArray = biasesContainer.contentBiases;

            if (contentBiasesArray == null)
            {
                return;
            }

            List<ContentBias> contentBiasesList = new List<ContentBias>();

            Envelope envelope;

            ContentBias contentBias;

            for (int i = 0; i < envelopes.Length; i++)
            {
                envelope = envelopes[i].GetComponent<Envelope>();
                
                for (int j = 0; j < contentBiasesArray.Length; j++)
                {
                    contentBias = contentBiasesArray[i];

                    if (envelope.GetJointId() != contentBias.jointId)
                    {
                        continue;
                    }
                    contentBiasesList.Add(contentBias);
                    break;
                }
            }

            if (envelopes.LongLength == 0)
            {
                return;
            }
            
            SaveBiasesToJson(contentBiasesList.ToArray(), async);
        }
        
        private void SaveBribesToJsonByGivenEnvelopes(GameObject[] envelopes, bool async)
        {
            string json = FileManager.LoadJsonFile(PATH_BRIBES_CONTAINER);
            
            BribesContainer bribesContainer = JsonUtility.FromJson<BribesContainer>(json);

            ContentBribe[] contentBribesArray = bribesContainer.contentBribes;

            if (contentBribesArray == null)
            {
                return;
            }

            List<ContentBribe> contentBribesList = new List<ContentBribe>();

            Envelope envelope;

            ContentBribe contentBribe;

            for (int i = 0; i < envelopes.Length; i++)
            {
                envelope = envelopes[i].GetComponent<Envelope>();
                
                for (int j = 0; j < contentBribesArray.Length; j++)
                {
                    contentBribe = contentBribesArray[i];

                    if (envelope.GetJointId() != contentBribe.jointId)
                    {
                        continue;
                    }
                    contentBribesList.Add(contentBribe);
                    break;
                }
            }

            if (envelopes.LongLength == 0)
            {
                return;
            }

            SaveBribesToJson(contentBribesList.ToArray(), async);
        }
        
        private void SaveLettersToJsonByGivenEnvelopes(GameObject[] envelopes, bool async)
        {
            string json = FileManager.LoadJsonFile(PATH_LETTERS_CONTAINER);
            
            LettersContainer lettersContainer = JsonUtility.FromJson<LettersContainer>(json);

            ContentLetter[] contentLettersArray = lettersContainer.contentLetters;

            if (contentLettersArray == null)
            {
                return;
            }

            List<ContentLetter> contentLettersList = new List<ContentLetter>();

            Envelope envelope;

            ContentLetter contentLetter;

            for (int i = 0; i < envelopes.Length; i++)
            {
                envelope = envelopes[i].GetComponent<Envelope>();
                
                for (int j = 0; j < contentLettersArray.Length; j++)
                {
                    contentLetter = contentLettersArray[i];

                    if (envelope.GetJointId() != contentLetter.jointId)
                    {
                        continue;
                    }
                    contentLettersList.Add(contentLetter);
                    break;
                }
            }

            if (envelopes.LongLength == 0)
            {
                return;
            }
            
            SaveLettersToJson(contentLettersList.ToArray(), async);
        }

        private void UnloadEnvelopesFromJson(bool async)
        {
            EnvelopesContainer[] envelopesContainers = Array.Empty<EnvelopesContainer>();

            string json = JsonUtility.ToJson(envelopesContainers);
            string path = UnityEngine.Device.Application.streamingAssetsPath + PATH_ENVELOPES_CONTAINER;
            
            if (async)
            {
                File.WriteAllTextAsync(path, json);
                return;
            }
            
            File.WriteAllText(path, json);
        }

        public GameObject[] LoadEnvelopesContentFromJson(bool async = false)
        {
            List<GameObject> envelopesContent = new List<GameObject>();
            
            GameObject[] ads = LoadAdsFromJson();
            GameObject[] biases = LoadBiasesFromJson();
            GameObject[] bribes = LoadBribesFromJson();
            GameObject[] letters = LoadLettersFromJson();
            
            LoadEnvelopesContentIntoList(envelopesContent, ads);
            LoadEnvelopesContentIntoList(envelopesContent, biases);
            LoadEnvelopesContentIntoList(envelopesContent, bribes);
            LoadEnvelopesContentIntoList(envelopesContent, letters);

            if (envelopesContent.Count == 0)
            {
                return envelopesContent.ToArray();
            }

            ContentAd[] emptyArrayContentAds = Array.Empty<ContentAd>();
            ContentBias[] emptyArrayContentBiases = Array.Empty<ContentBias>();
            ContentBribe[] emptyArrayContentBribes = Array.Empty<ContentBribe>();
            ContentLetter[] emptyArrayContentLetters = Array.Empty<ContentLetter>();
            
            SaveAdsToJson(emptyArrayContentAds, async);
            SaveBiasesToJson(emptyArrayContentBiases, async);
            SaveBribesToJson(emptyArrayContentBribes, async);
            SaveLettersToJson(emptyArrayContentLetters, async);

            return envelopesContent.ToArray();
        }

        private void SaveAdsToJson(ContentAd[] contentAds, bool async)
        {
            string json = JsonUtility.ToJson(contentAds);
            string path = Application.streamingAssetsPath + PATH_ADS_CONTAINER;

            if (async)
            {
                File.WriteAllTextAsync(path, json);
                return;
            }
            
            File.WriteAllText(path, json);
        }

        private void SaveBiasesToJson(ContentBias[] contentBiases, bool async)
        {
            string json = JsonUtility.ToJson(contentBiases);
            string path = Application.streamingAssetsPath + PATH_BIASES_CONTAINER;

            if (async)
            {
                File.WriteAllTextAsync(path, json);
                return;
            }
            
            File.WriteAllText(path, json);
        }

        private void SaveBribesToJson(ContentBribe[] contentBribes, bool async)
        {
            string json = JsonUtility.ToJson(contentBribes);
            string path = Application.streamingAssetsPath + PATH_BRIBES_CONTAINER;

            if (async)
            {
                File.WriteAllTextAsync(path, json);
                return;
            }
            
            File.WriteAllText(path, json);
        }

        private void SaveLettersToJson(ContentLetter[] contentLetters, bool async)
        {
            string json = JsonUtility.ToJson(contentLetters);
            string path = Application.streamingAssetsPath + PATH_LETTERS_CONTAINER;

            if (async)
            {
                File.WriteAllTextAsync(path, json);
                return;
            }
            
            File.WriteAllText(path, json);
        }

        public GameObject[] CreateEnvelopesFromJson()
        {
            string json = FileManager.LoadJsonFile(PATH_ENVELOPES_CONTAINER);

            if (json == "{}")
            {
                return null;
            }
            
            Debug.Log(json);
            
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
                
                LoadEnvelopeContentIntoEnvelope(i, contentEnvelopes, envelopeGameObject, envelopeComponent);

                envelopes[i] = envelopeGameObject;
            }

            return envelopes;
        }

        private void LoadEnvelopeContentIntoEnvelope(int i, ContentEnvelope[] contentEnvelopes, GameObject envelopeGameObject, 
            Envelope envelopeComponent)
        {
            GameObject envelopeContent = null;
                
            switch (contentEnvelopes[i].envelopeContentType)
            {
                case EnvelopeContentType.AD:
                    envelopeContent = AddAdFromJsonByGivenJointId(contentEnvelopes[i].jointId, envelopeGameObject);
                    break;
                    
                case EnvelopeContentType.BIAS:
                    envelopeContent = AddBiasFromJsonGivenJointId(contentEnvelopes[i].jointId, envelopeGameObject);
                    break;
                    
                case EnvelopeContentType.BRIBE:
                    envelopeContent = AddBribeFromJsonGivenJointId(contentEnvelopes[i].jointId, envelopeGameObject);
                    break;
                    
                case EnvelopeContentType.LETTER:
                    envelopeContent = AddLetterFromJsonGivenJointId(contentEnvelopes[i].jointId, envelopeGameObject);
                    break;
            }
                
            envelopeComponent.SetEnvelopeContent(envelopeContent);
        }

        private GameObject AddAdFromJsonByGivenJointId(int jointId, GameObject envelopeGameObject)
        {
            string json = FileManager.LoadJsonFile(PATH_ADS_CONTAINER);
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
        
        private GameObject AddBiasFromJsonGivenJointId(int jointId, GameObject envelopeGameObject)
        {
            string json = FileManager.LoadJsonFile(PATH_BIASES_CONTAINER);
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
        
        private GameObject AddBribeFromJsonGivenJointId(int jointId, GameObject envelopeGameObject)
        {
            string json = FileManager.LoadJsonFile(PATH_BRIBES_CONTAINER);
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
        
        private GameObject AddLetterFromJsonGivenJointId(int jointId, GameObject envelopeGameObject)
        {
            string json = FileManager.LoadJsonFile(PATH_LETTERS_CONTAINER);
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
