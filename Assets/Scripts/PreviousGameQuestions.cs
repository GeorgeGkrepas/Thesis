using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PreviousGameQuestions : MonoBehaviour
{
    TextAsset xmlTextAsset;
    private XmlDocument previousQuestionData;
    public XmlNodeList previousQuestions;

    private MatchingHandler matchingHandler;

    public GameObject gridUnit;
    public GameObject grid;

    private List<XmlNode> questionsToBeAdded = new List<XmlNode>();
    private List<string> answeredCorrectlyList = new List<string>();

    private void Start()
    {
        matchingHandler = GetComponent<MatchingHandler>();

        if(SceneManager.GetActiveScene().name == "StartingMenu")
        {
            LoadData();
            EmptyList();
            LoadFromXML();
        }
    }

    [PunRPC]
    public void AddToList(int questionIndex, string answeredCorrectly)
    {
        questionsToBeAdded.Add(matchingHandler.matches[questionIndex]);
        answeredCorrectlyList.Add(answeredCorrectly);
    }

    public void EmptyFile()
    {
        for(int i=0; i<19; i++)
        {
            previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/question").InnerText = "";
            previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/isImage").InnerText = "";
            previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/answeredCorrectly").InnerText = "";

            for(int j=1; j<=4; j++)
            {
                previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/match" + j.ToString() + "/part1").InnerText = "";
                previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/match" + j.ToString() + "/part2").InnerText = "";
            }
        }
 
        previousQuestionData.Save(Application.dataPath + "/Resources/XML/PreviousGameQuestionsList.xml");
    }

    public void LoadData()
    {
        xmlTextAsset = Resources.Load<TextAsset>("XML/PreviousGameQuestionsList");
        previousQuestionData = new XmlDocument();
        previousQuestionData.LoadXml(xmlTextAsset.text);

        if (SceneManager.GetActiveScene().name == "MultiplayerGame")
        {
            matchingHandler = GameObject.Find("GameManager").GetComponent<MatchingHandler>();
        }
    }

    public void SaveToXML()
    {
        for (int i = 0; i < questionsToBeAdded.Count; i++)
        {
            previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/question").InnerText = questionsToBeAdded[i].SelectSingleNode("question").InnerText;
            previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/isImage").InnerText = questionsToBeAdded[i].SelectSingleNode("isImage").InnerText;
            previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/answeredCorrectly").InnerText = answeredCorrectlyList[i];

            for (int j = 1; j <= 4; j++)
            {
                previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/match" + j.ToString() + "/part1").InnerText = questionsToBeAdded[i].SelectSingleNode("match" + j.ToString() + "/part1").InnerText;
                previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/match" + j.ToString() + "/part2").InnerText = questionsToBeAdded[i].SelectSingleNode("match" + j.ToString() + "/part2").InnerText;
            }
        }

        previousQuestionData.Save(Application.dataPath + "/Resources/XML/PreviousGameQuestionsList.xml");
    }

    public void LoadFromXML()
    {
        for(int i=0; i<19; i++)
        {
            if(previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/question").InnerText != "")
            {
                GameObject _gridUnit = GameObject.Instantiate(gridUnit);
                _gridUnit.transform.SetParent(grid.transform, false);

                _gridUnit.transform.Find("QuestionText").GetComponent<TextMeshProUGUI>().text = previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/question").InnerText;
                if (previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/isImage").InnerText == "no")
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        _gridUnit.transform.Find("Part1_" + j.ToString() + "image").gameObject.SetActive(false);
                        _gridUnit.transform.Find("Part1_" + j.ToString() + "text").GetComponent<TextMeshProUGUI>().text = previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/match" + j.ToString() + "/part1").InnerText;
                        _gridUnit.transform.Find("Part2_" + j.ToString()).GetComponent<TextMeshProUGUI>().text = previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/match" + j.ToString() + "/part2").InnerText;
                    }
                }
                else if (previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/isImage").InnerText == "yes")
                {
                    for (int j = 1; j <= 4; j++)
                    {
                        _gridUnit.transform.Find("Part1_" + j.ToString() + "text").gameObject.SetActive(false);
                        _gridUnit.transform.Find("Part1_" + j.ToString() + "image").GetComponent<Image>().sprite = Resources.Load<Sprite>(previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/match" + j.ToString() + "/part1").InnerText);
                        _gridUnit.transform.Find("Part2_" + j.ToString()).GetComponent<TextMeshProUGUI>().text = previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/match" + j.ToString() + "/part2").InnerText;
                    }
                }

                if (previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/answeredCorrectly").InnerText == "yes")
                {
                    _gridUnit.transform.Find("Panel").GetComponent<Image>().color = Color.green;
                }
                else if (previousQuestionData.SelectSingleNode("//*[@id='" + (i + 1).ToString() + "']/answeredCorrectly").InnerText == "no")
                {
                    _gridUnit.transform.Find("Panel").GetComponent<Image>().color = Color.red;
                }
            }  
        }
    }

    public void EmptyList()
    {
        for(int i=0; i < grid.transform.childCount;)
        {
            Destroy(grid.transform.GetChild(0));
        }
    }

}
