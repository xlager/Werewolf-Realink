using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

[CreateAssetMenu]

public class DataSO : ScriptableObject
{
    [HideInInspector]
    public List<PlayerRoles> playerRoles;
    public int numberOfPlayers;
    [HideInInspector]
    public int whoIsPlaying;
    [HideInInspector]
    public bool isSeerAlive;
    public string[] playerNames = { "Matheus",  "Roberto", "Jonas",  "Joana",
                                     "Cl�udio",  "Raimundo","Pedro",  "Bruno",
                                     "Ana",      "Vanessa", "J�ssica","Manuela",
                                     "Guilherme","Mariana", "Marta",  "Reb�ca"};

    public void ResetData()
    {
        numberOfPlayers = 6;
        isSeerAlive = true;
        if (playerRoles != null)
            playerRoles.Clear();
        else
            playerRoles = new List<PlayerRoles>();
    }

    public string TranslateRole(PlayerRoles role)
    {
        
        switch (role)
        {
            case PlayerRoles.Villager:
                return "Alde�o";
            case PlayerRoles.Werewolf:
                return "Lobisomem";
            case PlayerRoles.Seer:
                return "Vidente";
            default:
                return "Alde�o";
        }
    }
};
