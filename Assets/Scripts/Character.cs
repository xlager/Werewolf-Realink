using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//

public class Character : MonoBehaviour
{
    public string characterName { get;  set; }
    public Image avatar;
    public Image selectedImage;
    public TextMeshProUGUI characterNameText;
    public Button button;
    private bool myTurn { get; set;}
    public  bool dead { get; set;}
    public Image votationTextBackground;
    public TextMeshProUGUI votationText;
    public int votationValue { get; private set; } = 0;
    [HideInInspector]
    public Enums.PlayerRoles role;
    [SerializeField]
    private DataSO dataSO;

    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GetVoted()
    {   
        votationTextBackground.gameObject.SetActive(true);
        votationValue++;
        votationText.text = votationValue.ToString();
        if (votationValue <= 0)
        {
            votationTextBackground.gameObject.SetActive(false);
        }
    }
    public void RemoveVote()
    {
        votationValue--;
        if (votationValue <= 0)
        {
            votationTextBackground.gameObject.SetActive(false);
        }
        votationText.text = votationValue.ToString();
    }

    public bool Killed(Sprite deathImage)
    {
        avatar.sprite = deathImage;
        button.interactable = false;
        dead = true;
        if (role == Enums.PlayerRoles.Seer)
            dataSO.isSeerAlive = false;
        if (role == Enums.PlayerRoles.Werewolf)
            return true;
        else
            return false;
    }

    public void ClearVoting()
    {
        votationValue = 0;
        votationTextBackground.gameObject.SetActive(false);
    }

    public void Select()
    {
        selectedImage.gameObject.SetActive(true);
    }
    public void UnSelect()
    {
        selectedImage.gameObject.SetActive(false);
    }
}
