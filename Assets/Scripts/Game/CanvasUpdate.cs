using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class CanvasUpdate : MonoBehaviour
{
    [SerializeField] public List<TextMeshProUGUI> allTmpros = new List<TextMeshProUGUI>();
    [SerializeField] public List<Image> allImages = new List<Image>();
    public string[] allTexts;
    public string[] allImageNames;

    private void Start()
    {
        allTexts = new string[allTmpros.Count];
        allImageNames = new string[allImages.Count];
    }

    public void GetTexts()
    {
        for (int i = 0; i < allTmpros.Count; i++)
        {
            allTexts[i] = allTmpros[i].text;
        }
    }

    public void UpdateTexts(string[] texts)
    {
        for(int i = 0; i < texts.Length; i++)
        {
            allTmpros[i].text = texts[i];
        }
    }

    [PunRPC]
    public void UpdateTextsUtil(string[] texts)
    {
        UpdateTexts(texts);
    }

    public void GetImages()
    {
        for (int i = 0; i < allImages.Count; i++)
        {
            allImageNames[i] = allImages[i].sprite.name;
        }
    }

    public void UpdateImages(string[] imageNames)
    {
        for (int i = 0; i < imageNames.Length; i++)
        {
            allImages[i].sprite = Resources.Load<Sprite>("Images/" + imageNames[i]);
        }
    }

    [PunRPC]
    public void UpdateImagesUtil(string[] imageNames)
    {
        UpdateImages(imageNames);
    }
}
