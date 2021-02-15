using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using GEAR.Localization.Text;
using UnityEngine;
using TMPro;

namespace QuestManager
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private string QuestsPath = "Quests";
        [SerializeField] private GameObject MainQuestBody;
        [SerializeField] private Vector3 MQBodyiniOffset = new Vector3(0, -5, 0);
        [SerializeField] private Vector3 MQBodyOffset = new Vector3(0, -2.85f, 0);
        [SerializeField] private GameObject SubQuestBody;
        [SerializeField] private Vector3 SQBodyOffset = new Vector3(0f, -2f, -0.1f);
        [SerializeField] private GameObject DataObjectRoot;
        [SerializeField] private GameObject Cover;

        private int _mainQuestIndex = 0;
        private readonly List<MainQuest> _mainQuests = new List<MainQuest>();

        public MainQuest[] MainQuests => _mainQuests.ToArray();

        private void Start()
        {
            _mainQuestIndex = 0;
            ReadQuestXml();
            ActivateNextMainQuest();
        }

        public void ReadQuestXml()
        {
            if (string.IsNullOrEmpty(QuestsPath)) return;

            _mainQuests.Clear();

            var text = Resources.Load<TextAsset>(QuestsPath);
            if (!text)
            {
                Debug.LogError($"QuestManager::ReadQuestXML: Unable to load quest file {QuestsPath}");
                return;
            }

            var doc = XDocument.Load(new MemoryStream(text.bytes));

            var mainQuestPosition = MQBodyiniOffset;
            foreach (var mainQuest in doc.Descendants("MainQuest"))
            {
                var subQuestOffsetCount = 0;

                var mainQuestObject = InstantiateQuestObject<MainQuest>(MainQuestBody, mainQuestPosition, DataObjectRoot.transform);
                mainQuestObject.name = mainQuest.Element("TranslationKey")?.Value ?? "";
                mainQuestObject.GetComponentInChildren<LocalizedTMP>().Key = mainQuestObject.name;
                mainQuestObject.GetComponentInChildren<TextMeshPro>().text = mainQuest.Element("DefaultText")?.Value ?? "";

                foreach (var subQuest in mainQuest.Elements().Where(element => element.Name == "SubQuest"))
                {
                    var subQuestPosition = subQuestOffsetCount * SQBodyOffset;
                    var subQuestObject = InstantiateQuestObject<SubQuest>(SubQuestBody, subQuestPosition, mainQuestObject.transform);

                    subQuestObject.name = subQuest.Element("TranslationKey")?.Value ?? "";
                    subQuestObject.GetComponentInChildren<LocalizedTMP>().Key = subQuestObject.name;
                    subQuestObject.GetComponentInChildren<TextMeshPro>().text = subQuest.Element("DefaultText")?.Value ?? "";

                    if (subQuest.Attribute("HasAdditionalInformation")?.Value == "True")
                        subQuestObject.HasAdditionalInformation = true;

                    var subQuestScript = subQuest.Element("Script")?.Value;
                    if (subQuestScript != null)
                    {
                        var script = Type.GetType(subQuestScript);
                        subQuestObject.gameObject.AddComponent(script);
                        subQuestObject.gameObject.AddComponent<AudioSource>();
                    }

                    mainQuestObject.AddSubQuest(subQuestObject);


                    subQuestOffsetCount++;
                }

                mainQuestPosition += MQBodyOffset + SQBodyOffset * subQuestOffsetCount;
                _mainQuests.Add(mainQuestObject);
            }
        }

        private static T InstantiateQuestObject<T>(GameObject questPrefab, Vector3 position, Transform parent = null)
        {
            var mainQuestObject = Instantiate(questPrefab, parent);
            mainQuestObject.transform.localScale = Vector3.one;
            mainQuestObject.transform.localPosition = position;
            return mainQuestObject.GetComponent<T>();
        }

        public void ActivateNextMainQuest()
        {
            if (_mainQuestIndex >= _mainQuests.Count)
                return;

            var activeMainQuest = _mainQuests[_mainQuestIndex++];

            activeMainQuest.IsActive = true;
            activeMainQuest.ActivateNextSubQuest();
            activeMainQuest.OnQuestFinished.AddListener(() =>
            {
                activeMainQuest.IsActive = false;
                ActivateNextMainQuest();
            });
        }



        private void CleanObjects()
        {
            foreach (var mainQuest in _mainQuests)
            {
                if (mainQuest)
                    DestroyImmediate(mainQuest.gameObject);
            }
            _mainQuests.Clear();

            if (Cover)
            {
                Cover.transform.localRotation = Quaternion.Euler(new Vector3(0f, -90f, 90f));
            }
        }
    }
}