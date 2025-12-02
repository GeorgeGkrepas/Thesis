using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using Photon.Pun;

public class MatchingHandler : MonoBehaviour
{
    private XmlDocument matchingDataXml;
    public XmlNodeList matches;
    public List<XmlNode> usedMatches = new List<XmlNode>();

    [SerializeField] private List<TextMeshProUGUI> part1PapersText = new List<TextMeshProUGUI>(); //Pinned papers' text, aka part1 of puzzle
    [SerializeField] private List<Image> part1PapersImage = new List<Image>(); //Pinned papers' image, aka part1 of puzzle
    [SerializeField] private List<XRSocketInteractor> socketInteractors = new List<XRSocketInteractor>(); //Socket Interactors of pins
    [SerializeField] private List<GameObject> GrabbablePapers = new List<GameObject>(); //Grabbable papers, aka part2 of puzzle
    [SerializeField] private TextMeshProUGUI questionText;

    //Used to reset their positions
    private Vector3[] paperResetPositions = new Vector3[8]; //Array of paper position coordinates
    private Quaternion[] paperResetRotations = new Quaternion[8]; //Array of paper position rotations

    private void Awake()
    {
        TextAsset xmlTextAsset = Resources.Load<TextAsset>("XML/MatchingList");
        this.matchingDataXml = new XmlDocument();
        this.matchingDataXml.LoadXml(xmlTextAsset.text);

        this.matches = this.matchingDataXml.SelectNodes("/MatchingList/matches"); //List of all matching bundles in the XML file
        
        for(int i = 0; i < GrabbablePapers.Count; i++)
        {
            paperResetPositions[i] = GrabbablePapers[i].transform.position;
            paperResetRotations[i] = GrabbablePapers[i].transform.rotation;
        }
    }

    public int GetRandomMatchingIndex()
    {
        int index;

        do
        {
            index = Random.Range(0, matches.Count); //Get a random matching bundle from 'matches' list
        } while (usedMatches.Contains(matches[index]));

        usedMatches.Add(this.matches[index]); //Add selected matching bundle to 'usedMatches' list

        return index;
    }

    public void SetMatchingPairs(int index)
    {
        //Declarations
        List<int> usedIndexes = new List<int>();
        int randomIndex;

        Resources.UnloadUnusedAssets();

        //Set question text
        questionText.text = this.matches[index].SelectSingleNode("question").InnerText;
        
        //Set part1 on sockets (Text)
        if (this.matches[index].SelectSingleNode("isImage").InnerText == "no")
        {
            GetComponent<PhotonView>().RPC("SwapQuestionType", RpcTarget.All, true, false);

            for (int i = 0; i < (socketInteractors.Count)/2; i++)
            {
                do
                {
                    randomIndex = Random.Range(0, (socketInteractors.Count)/2);
                } while (usedIndexes.Contains(randomIndex));

                usedIndexes.Add(randomIndex);
                part1PapersText[randomIndex].text = this.matches[index].SelectSingleNode("match" + (i + 1).ToString() + "/part1").InnerText;

            }

            for (int i = (socketInteractors.Count)/2; i < socketInteractors.Count; i++)
            {
                do
                {
                    randomIndex = Random.Range((socketInteractors.Count)/2, socketInteractors.Count);
                } while (usedIndexes.Contains(randomIndex));

                usedIndexes.Add(randomIndex);
                part1PapersText[randomIndex].text = this.matches[index].SelectSingleNode("match" + (i - 3).ToString() + "/part1").InnerText;

            }
        }
        else if (this.matches[index].SelectSingleNode("isImage").InnerText == "yes") //Set part1 on sockets (Image)
        {

            GetComponent<PhotonView>().RPC("SwapQuestionType", RpcTarget.All, false, true);

            for (int i = 0; i < (socketInteractors.Count) / 2; i++)
            {
                do
                {
                    randomIndex = Random.Range(0, (socketInteractors.Count)/2);
                } while (usedIndexes.Contains(randomIndex));

                usedIndexes.Add(randomIndex);
                part1PapersImage[randomIndex].sprite = Resources.Load<Sprite>(this.matches[index].SelectSingleNode("match" + (i + 1).ToString() + "/part1").InnerText);

            }

            for (int i = (socketInteractors.Count) / 2; i < socketInteractors.Count; i++)
            {
                do
                {
                    randomIndex = Random.Range((socketInteractors.Count) / 2, socketInteractors.Count);
                } while (usedIndexes.Contains(randomIndex));

                usedIndexes.Add(randomIndex);
                part1PapersImage[randomIndex].sprite = null;
                Resources.UnloadUnusedAssets();
                part1PapersImage[randomIndex].sprite = Resources.Load<Sprite>(this.matches[index].SelectSingleNode("match" + (i - 3).ToString() + "/part1").InnerText);

            }
        }

            //Empty usedIndexed list
            usedIndexes.Clear();

        //Set part2 on Grabbable Papers
        for (int i = 0; i < (socketInteractors.Count)/2; i++)
        {
            do
            {
                randomIndex = Random.Range(0, (socketInteractors.Count)/2);
            } while (usedIndexes.Contains(randomIndex));

            usedIndexes.Add(randomIndex);
            GrabbablePapers[randomIndex].GetComponentInChildren<TextMeshProUGUI>().text = this.matches[index].SelectSingleNode("match" + (i + 1).ToString() + "/part2").InnerText;

        }

        for (int i = (socketInteractors.Count)/2; i < socketInteractors.Count; i++)
        {
            do
            {
                randomIndex = Random.Range((socketInteractors.Count)/2, socketInteractors.Count);
            } while (usedIndexes.Contains(randomIndex));

            usedIndexes.Add(randomIndex);
            GrabbablePapers[randomIndex].GetComponentInChildren<TextMeshProUGUI>().text = this.matches[index].SelectSingleNode("match" + (i - 3).ToString() + "/part2").InnerText;

        }     
    }

    public bool CheckIfMatchesAreCorrect(int index,int a, int b)
    {
        int correctMatchesCounter = 0;
        int savedIndex = 0;

        if (this.matches[index].SelectSingleNode("isImage").InnerText == "no") //Check if puzzle is text
        {
            for (int i = a; i <= b; i++)
            {
                for (int j = 0; j < (this.matches[index].ChildNodes.Count) - 2; j++)
                {
                    if (this.matches[index].SelectSingleNode("match" + (j + 1).ToString() + "/part1").InnerText == part1PapersText[i].text)
                        savedIndex = j;
                }

                if (this.matches[index].SelectSingleNode("match" + (savedIndex + 1).ToString() + "/part2").InnerText == socketInteractors[i].GetOldestInteractableSelected().transform.GetComponentInChildren<TextMeshProUGUI>().text)
                    correctMatchesCounter++;

            }
        }
        else if (this.matches[index].SelectSingleNode("isImage").InnerText == "yes") //Check if puzzle is images
        {
            for (int i = a; i <= b; i++)
            {
                for (int j = 0; j < (this.matches[index].ChildNodes.Count) - 2; j++)
                {
                    if (this.matches[index].SelectSingleNode("match" + (j + 1).ToString() + "/part1").InnerText == "Images/" + part1PapersImage[i].sprite.name)
                        savedIndex = j;
                }

                if (this.matches[index].SelectSingleNode("match" + (savedIndex + 1).ToString() + "/part2").InnerText == socketInteractors[i].GetOldestInteractableSelected().transform.GetComponentInChildren<TextMeshProUGUI>().text)
                    correctMatchesCounter++;

            }
        }

        if (correctMatchesCounter == 4)
            return true;
        else
            return false;

    }

    public bool CheckIfSocketsAreFilled(int a, int b)
    {
        int filledSocketCounter = 0;

        for (int i = a; i <= b; i++)
        {
            if (socketInteractors[i].hasSelection)
            {
                filledSocketCounter++;
            }
        }

        if (filledSocketCounter == 4)
            return true;
        else
            return false;

    }

    [PunRPC]
    public void ResetPaperPositionsUtil(int a, int b)
    {

        StartCoroutine(ResetPaperPositionCoroutine(a,b));

    }

    public IEnumerator ResetPaperPositionCoroutine(int a, int b)
    {
        for (int i = a; i <= b; i++)
        {
            socketInteractors[i].enabled = false;
        }

        yield return null;

        for (int i = a; i <= b; i++)
        {
            GrabbablePapers[i].transform.rotation = paperResetRotations[i];
            GrabbablePapers[i].transform.position = paperResetPositions[i];
        }

        yield return null;

        for (int i = a; i <= b; i++)
        {
            socketInteractors[i].enabled = true;
        }
    }

    [PunRPC]
    void SwapQuestionType(bool textBool, bool imageBool)
    {
        for (int i = 0; i < socketInteractors.Count; i++)
        {
            part1PapersText[i].gameObject.SetActive(textBool);
            part1PapersImage[i].gameObject.SetActive(imageBool);
        }
    }

}
