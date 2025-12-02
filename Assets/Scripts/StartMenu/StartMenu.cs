using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{

    [SerializeField] private Button CreateRoomButton, JoinRoomButton, ExitButton, PreviousGameButton;
    [SerializeField] private List<Button> backButtonList = new List<Button>();
    [SerializeField] private GameObject MainMenu, CreateRoomMenu, JoinRoomMenu, PreviousGameMenu;

    [SerializeField] private TMP_InputField createInput;
    [SerializeField] private TMP_InputField joinInput;
    
    private void Start()
    {
        //Setting up button functions
        CreateRoomButton.onClick.AddListener(SwitchToCreateRoom);
        JoinRoomButton.onClick.AddListener(SwitchToJoinRoom);
        PreviousGameButton.onClick.AddListener(SwitchToPreviousGame);
        ExitButton.onClick.AddListener(Exit);
        
        foreach (Button BackButton in backButtonList)
        {
            BackButton.onClick.AddListener(BackToMainMenu);
        }

    }

    void SwitchToCreateRoom()
    {
        MainMenu.SetActive(false);
        CreateRoomMenu.SetActive(true);
    }

    void SwitchToJoinRoom()
    {
        MainMenu.SetActive(false);
        JoinRoomMenu.SetActive(true);
    }

    void SwitchToPreviousGame()
    {
        MainMenu.SetActive(false);
        PreviousGameMenu.SetActive(true);
    }

    void Exit()
    {
        Application.Quit();
    }

    void BackToMainMenu()
    {
        CreateRoomMenu.SetActive(false);
        JoinRoomMenu.SetActive(false);
        PreviousGameMenu.SetActive(false);
        MainMenu.SetActive(true);

        createInput.text = "";
        joinInput.text = "";

    }

}
