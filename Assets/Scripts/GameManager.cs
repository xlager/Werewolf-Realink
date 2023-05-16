using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public List<Character> Players;

    [SerializeField]
    private DataSO dataSO;
    [SerializeField]
    private Sprite deadImg;


    private TimeOfDay timeOfDay;
    public TextMeshProUGUI timeOfDayText;

    public TextMeshProUGUI mainText;
    public Button nextButton;
    public Button jumpVoteButton;
    public Button unPauseButton;
    public Button backToMenuButton;
    public Image lockScreen;
    public TextMeshProUGUI endGameText;
    public Animator animator;
    private PlayerRoles turnOf;
    private bool canClickPlayers = false;
    private Character selectedPlayerToBeKilled;
    private Character selectedBySeer;
    private Character selectedByVillager;
    private int numberOfAlivePlayers;
    private int numberOfAliveWerewolves;
    private int villagerTurnCounter;
    private List<Character> aliveVilagersList;
    public List<Sprite> turnIcons;
    public Image turnIcon;

    // Start is called before the first frame update
    void Start()
    {
       // SceneTest();
        SetStage();
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            endGameText.text = "PAUSE";
            lockScreen.gameObject.SetActive(true);
            unPauseButton.gameObject.SetActive(true);  
            backToMenuButton.gameObject.SetActive(true);  
        }
    }

    private void SetStage()
    {
        //Configure buttons
        backToMenuButton.onClick.AddListener(() => ReturnToMenuButton());
        jumpVoteButton.onClick = new Button.ButtonClickedEvent();
        jumpVoteButton.onClick.AddListener(() => Voted());
        jumpVoteButton.onClick.AddListener(() => StartVoting(true));

        timeOfDay = TimeOfDay.Night;
        timeOfDayText.text = "Anoitecer";
        mainText.text = "O jogo começou.\r\nA partir de agora, os lobisomens tomarão a primeira ação, escolher quem eles matarão.\r\nJogador Lobisomem, apertar no próximo.";
        ChangeTurns(PlayerRoles.Werewolf);
        
        int count = 0;
        numberOfAlivePlayers = dataSO.numberOfPlayers;
        if (dataSO.numberOfPlayers == 16)
            numberOfAliveWerewolves = 3;
        else
            numberOfAliveWerewolves = 2;

        aliveVilagersList = new List<Character>();
        dataSO.isSeerAlive = true;
        foreach (Character p in Players)
        {
            if(count < dataSO.numberOfPlayers)
            {
                p.gameObject.SetActive(true);
                p.characterName = dataSO.playerNames[count];
                p.characterNameText.text = p.characterName;
                p.dead = false;
                p.role = dataSO.playerRoles[count];
                p.button.onClick = new Button.ButtonClickedEvent();
                p.button.onClick.AddListener(() => CharacterSelected(p));
                aliveVilagersList.Add(p);
}
            else
            {
                p.gameObject.SetActive(false);
            }
            count++;
        };
        WerewolfTurn();
    }


    private void WerewolfTurn()
    {
        animator.SetBool("PlayDay", true);
        animator.SetBool("PlayNight", true);
        if (!CheckEndGame())
        {
            canClickPlayers = true;
            ClearPlayersUI();
            ChangeTimeOfDay(TimeOfDay.Night);
            nextButton.interactable = false;
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick = new Button.ButtonClickedEvent();
            if (dataSO.isSeerAlive)
                nextButton.onClick.AddListener(() => SeerTurn());
            else
                nextButton.onClick.AddListener(() => GettingIntoToDawn());
        }
    }       

    private void SeerTurn()
    {
        canClickPlayers = true;
        ClearPlayersUI();
        ChangeTurns(PlayerRoles.Seer);
        mainText.text = "Turno do Vidente.\r\nSelecione qual personagem você deseja descobrir a identidade.";
        nextButton.interactable = false;
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick = new Button.ButtonClickedEvent();
        nextButton.onClick.AddListener(() => ShowSeerChoice());
    }
    private void ShowSeerChoice()
    {
        ClearPlayersUI();
        if (selectedBySeer.role == PlayerRoles.Werewolf)
            mainText.text = $"Turno do Vidente.\r\nO jogador {selectedBySeer.characterName} é um lobisomem;";
        else
            mainText.text = $"Turno do Vidente.\r\nA identidade do jogador {selectedBySeer.characterName} não é lobisomem;";

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick = new Button.ButtonClickedEvent();
        nextButton.onClick.AddListener(() => GettingIntoToDawn());
        canClickPlayers = false;
    }
    
    private void GettingIntoToDawn()
    {
        ClearPlayersUI();
        mainText.text = "O turno dos lobisomens terminou.\r\nA manhã se aproxima, com isto o cheiro da morte também...";
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick = new Button.ButtonClickedEvent();
        nextButton.onClick.AddListener(() => DawnTurn());
    }

    private void DawnTurn()
    {
        animator.SetBool("PlayNight", false);
        animator.SetBool("PlayDay", true);
        ClearPlayersUI();
        ChangeTimeOfDay(TimeOfDay.Dawn);
        DoTheKill(selectedPlayerToBeKilled);
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick = new Button.ButtonClickedEvent();
        nextButton.onClick.AddListener(() => VillagersTurn());
    }

    private void VillagersTurn()
    {
        ClearPlayersUI();
        if (!CheckEndGame())
        {
            mainText.text = "Amanheçeu.\r\nApós o assassinato, os aldeões se reúnem para votar em quem expulsar da vila.\r\nPressione próximo para iniciar a votação.";
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick = new Button.ButtonClickedEvent();
            nextButton.onClick.AddListener(() => Voted());
            nextButton.onClick.AddListener(() => StartVoting());
            villagerTurnCounter = -1;
        }
    }

    private void StartVoting(bool jumped = false)
    {
        if (jumped && selectedByVillager != null)
        {
            selectedByVillager.RemoveVote();
        }
        ClearPlayersUI(false);
        jumpVoteButton.gameObject.SetActive(true);
        if (villagerTurnCounter < numberOfAlivePlayers)
        {
            canClickPlayers = true;
            ChangeTurns(PlayerRoles.Villager);
            selectedByVillager = null;
            mainText.text = $"Votação.\r\nO jogador {aliveVilagersList[villagerTurnCounter].characterName} está votando.";
            nextButton.interactable = false;
        }
        else
        {
            canClickPlayers = false;
            mainText.text = $"Votação.\r\nA votação teve um final, aperte no próximo para ver quem foi expulso da vila";
            nextButton.interactable = true;
            jumpVoteButton.gameObject.SetActive(false);
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick = new Button.ButtonClickedEvent();
            nextButton.onClick.AddListener(() => EndVotation());
        }
    }

    private void Voted()
    {
        villagerTurnCounter++;
    }

    private void EndVotation()
    {
        switch (GetTheMostVoted())
        {
            case VotationResult.Win:
                {
                    mainText.text = $"Fim da Votação.\r\nO jogador {selectedByVillager.characterName} foi expulso da vila";
                    DoTheKill(selectedByVillager);
                    mainText.text = mainText.text + ("\n\rPressione próximo para avançar para o turno dos lobisomens");
                }
                break;
            case VotationResult.Tie:
                mainText.text = $"Fim da Votação.\r\nHouve mais do que um jogador com a mesma quantidade de votos, logo, ninguém foi expulso da vila";
                break;
            case VotationResult.Zero:
                mainText.text = $"Fim da Votação.\r\nOs aldeões optaram por não expulsar ninguém";
                break;
        }
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick = new Button.ButtonClickedEvent();
        nextButton.onClick.AddListener(() => WerewolfTurn());

    }

    private VotationResult GetTheMostVoted()
    {
        VotationResult Result = VotationResult.Zero;
        selectedByVillager = Players[0];
        for (int i = 1; i < dataSO.numberOfPlayers; i++)
        {
            if ((selectedByVillager.votationValue < Players[i].votationValue) && !Players[i].dead)
            {
                selectedByVillager = Players[i];
                Result = VotationResult.Win;
            }
            else if(selectedByVillager.votationValue == Players[i].votationValue && !Players[i].dead)
            {
                Result = VotationResult.Tie;                
            }
            Players[i].UnSelect();
        }
        if (selectedByVillager.votationValue == 0)
            Result = VotationResult.Zero;
        return Result;
    }
    private void ClearPlayersUI(bool all = true)
    {
        if (all)
        {
            foreach (var player in Players)
            {
                player.ClearVoting();
                player.UnSelect();
            }
        }
        else
        {
            foreach (var player in Players)
            {
                player.UnSelect();
            }
        }
    }

    private void DoTheKill(Character toBeDeceased)
    {
        if (toBeDeceased.Killed(deadImg))
            numberOfAliveWerewolves--;
        numberOfAlivePlayers--;
        aliveVilagersList.Remove(toBeDeceased);
    }
    private bool CheckEndGame()
    {
        if ((numberOfAlivePlayers - numberOfAliveWerewolves) == numberOfAliveWerewolves)
        {
            lockScreen.gameObject.SetActive(true);
            endGameText.text = "FIM DE JOGO\r\nOs Lobisomens venceram!";
            unPauseButton.gameObject.SetActive(false);
            return true;
        }
        else if (numberOfAliveWerewolves == 0)
        {
            lockScreen.gameObject.SetActive(true);
            unPauseButton.gameObject.SetActive(false);
            endGameText.text = "FIM DE JOGO\r\nOs Aldeões venceram!";
            return true;
        }
        else
            return false;
    }

    public void CharacterSelected(Character player)
    {
        if (canClickPlayers)
        {
            switch (turnOf)
            {
                case PlayerRoles.Villager:
                    {
                        if (selectedByVillager != player)
                        {
                            if (selectedByVillager != null)
                            {
                                selectedByVillager.RemoveVote();
                                selectedByVillager.UnSelect();
                            }
                            nextButton.interactable = true;
                            selectedByVillager = player;
                            player.GetVoted();
                            player.Select();
                        }
                        else
                        {
                            selectedByVillager.RemoveVote();
                            selectedByVillager.UnSelect();
                            selectedByVillager = null;

                        }
                    }
                    break;
                case PlayerRoles.Werewolf:
                    {
                        if (selectedPlayerToBeKilled != null && selectedPlayerToBeKilled != player)
                        {
                            selectedPlayerToBeKilled.UnSelect();
                        }
                        if ((player.role != PlayerRoles.Werewolf) && (!player.dead))
                        {
                            nextButton.interactable = true;
                            selectedPlayerToBeKilled = player;
                            player.Select();
                        }
                    }
                    break;
                case PlayerRoles.Seer:
                    {
                        if (selectedBySeer != null && selectedBySeer != player)
                        {
                            selectedBySeer.UnSelect();
                        }
                        nextButton.interactable = true;
                        selectedBySeer = player;
                        player.Select();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    private void ChangeTimeOfDay(TimeOfDay timeDay)
    {
        if (timeDay == TimeOfDay.Night)
        {
            timeOfDay = timeDay;
            ChangeTurns(PlayerRoles.Werewolf);
            timeOfDayText.text = "Anoitecer";
            mainText.text = "Turno dos Lobisomens.\r\nSelecione qual personagem você(s) deseja assasinar nesta noite clicando sobre sua imagem.\r\nApós, clicar no botão próximo para avançar o turno";
        }
        else
        {
            timeOfDay = TimeOfDay.Dawn;
            timeOfDayText.text = "Amanheçer";
            mainText.text = $"Amanheçeu.\r\nO jogador {selectedPlayerToBeKilled.characterName} foi assassinado!\r\nClique em próximo para continuar para o turno de votação.";
        }
    }
    private void ChangeTurns(PlayerRoles playerTurn)
    {
        turnOf = playerTurn;
        turnIcon.sprite = turnIcons[(int)turnOf];
    }
    private void ReturnToMenuButton()
    {
        SceneManager.LoadScene(0);
    }
    private void SceneTest()
    {
        if (dataSO.playerRoles == null)
        {
            dataSO.playerRoles = new List<PlayerRoles>();
        }
        else
        {
            dataSO.playerRoles.Clear();
        }
        for (int i = 0; i < 7; i++)
        {
            if (i == 0 || i == 1)
            {
                dataSO.playerRoles.Add(PlayerRoles.Werewolf);
            }
            else
            {
                if (i == 2)
                {
                    dataSO.playerRoles.Add(PlayerRoles.Seer);
                }
                else
                {
                    dataSO.playerRoles.Add(PlayerRoles.Villager);
                }
            }
        }
        dataSO.numberOfPlayers = 7;
        numberOfAlivePlayers = dataSO.numberOfPlayers;
        numberOfAliveWerewolves = 2;
        dataSO.isSeerAlive = true;
    }
}
