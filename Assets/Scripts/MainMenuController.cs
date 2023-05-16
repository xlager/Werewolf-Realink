using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Enums;


public class MainMenuController : MonoBehaviour
{

    public TextMeshProUGUI numberOfPlayersText, sortingPlayersName, sortingPlayersRole;
    public Slider sliderOfPlayers;
    public Image playerImage;
    public List<Sprite> playerImages;
    public Button nextButton;
    [SerializeField]
    private DataSO dataSO;

    private int numberOfPlayers = 7;
    private string[] playerNames = { "Matheus",  "Roberto", "Jonas",  "Joana",
                                     "Cláudio",  "Raimundo","Pedro",  "Bruno",
                                     "Ana",      "Vanessa", "Jéssica","Manuela",
                                     "Guilherme","Mariana", "Marta",  "Rebéca"};
    private int numberOfWerewolves = 2;
    private int numberOfVillagers = 4;
    private int numberOfSpecial = 1;
    private List<PlayerRoles> playerRoles;
    private int playersSorted = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerRoles = new List<PlayerRoles>();
    }
    public void ChangeNumberOfPlayers()
    {
        numberOfPlayersText.text = "Jogadores selecionados: " + sliderOfPlayers.value.ToString();
    }

    public void SetPlayers()
    {
        numberOfPlayers = ((int)sliderOfPlayers.value);
        dataSO.numberOfPlayers = numberOfPlayers;
        
        if (numberOfPlayers == 16)
            numberOfWerewolves = 3;
        else
            numberOfWerewolves = 2;

        numberOfVillagers = numberOfPlayers - numberOfWerewolves - numberOfSpecial;
        
        SetRolesList(playerRoles, PlayerRoles.Werewolf, numberOfWerewolves);
        SetRolesList(playerRoles, PlayerRoles.Seer, numberOfSpecial);
        SetRolesList(playerRoles, PlayerRoles.Villager, numberOfVillagers);
        if(dataSO.playerRoles == null)
            dataSO.playerRoles = new List<PlayerRoles>();
        else
            dataSO.playerRoles.Clear();
        
        PrepareNextPlayer();
    }

    public void MakeSort()
    {

        int randomValue = Random.Range(0, playerRoles.Count);
        sortingPlayersName.text = "O nome do jogador " + (playersSorted + 1).ToString() + " é : " + playerNames[playersSorted];
        playerImage.sprite = playerImages[playersSorted];
        sortingPlayersRole.text = "Seu papel é de: " + playerRoles[randomValue].ToString();
        dataSO.playerRoles.Add(playerRoles[randomValue]);
        playersSorted++;
        playerRoles.RemoveAt(randomValue);

    }

    public void PrepareNextPlayer()
    {
        if (playersSorted < numberOfPlayers)
        {
            sortingPlayersName.text = "Assim que pressionar o botão \"Próximo\", o jogador " + (playersSorted + 1).ToString() + " será selecionado";
            playerImage.gameObject.SetActive(false);
            sortingPlayersRole.text = "";
        }
        else
        {
            PrepareStartGaming();
        }
    }

    private void SetRolesList(List<PlayerRoles> playerTypes, PlayerRoles role, int numberToAdd)
    {
        for (int i = 0; i < numberToAdd; i++)
        {
            playerTypes.Add(role);
        }
    }

    private void PrepareStartGaming()
    {
        sortingPlayersName.text = "Todos os jogadores selecionados, ao apertar no botão próximo, iniciará o jogo";
        playerImage.gameObject.SetActive(false);
        sortingPlayersRole.text = "";
        
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick = new Button.ButtonClickedEvent();
        nextButton.onClick.AddListener(() => StartGame());
        nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Próximo";
    }

    public void StartGame()
    {
            SceneManager.LoadScene(1);
    }
}
