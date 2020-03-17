using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BlackjackManager : MonoBehaviour {
    public float money = 100f, wager = 10.00f;

    int playerPoints, dealerPoints;

    public int[] cardValues, tempCardValues;

    public Sprite[] cards, tempCards;

    public bool[] ace, tempAce;

    public GameObject cardObject, dealerCardObject, cardPrefab;
    public int deckIndex, dealerHandIndex, playerHandIndex;

    public int[] dealerCardValues;
    public Sprite[] dealerCards;
    public bool[] dealerAce;

    public Sprite backOfCard;

    public Text playerPointsText, dealerPointsText, wagerText, moneyText, betText;

    public GameObject hitButton, standButton, dealerHand, betPanel, betInput, errorText;

    public bool gameStarted;

    public GameObject winPanel, losePanel, tiePanel, bustedPanel, gameOverPanel, blackjackPanel;

    // use this for initialization
    void Start()
    {
        Shuffle();
    }

    public void startGame() {
        if (wager >= 0f){

            if (wager > money) {
                wager = money;

            }

            betPanel.SetActive(false);
            errorText.SetActive(false);
            playerDrawCard();
            playerDrawCard();
            dealerDrawCard();
            dealerDrawCard();
            gameStarted = true;
            GetComponent<AudioSource>().Play();
        }
    }

    // Update is called once per frame
    void Update() {
        wagerText.text = "Wager: " + wager.ToString("F2") + "$";

        playerPointsText.text = "Player Points:" + playerPoints.ToString ();

        if (Input.GetKeyDown(KeyCode.H) == true && gameStarted) {
            playerDrawCard();
        }

        if (Input.GetKeyDown(KeyCode.S) && gameStarted) {
            standing();
        }

        if (!gameStarted){
            betText.text = "Money: " + money.ToString("F2") + "$";
            moneyText.text = "Money: " + money.ToString("F2") + "$";
        }else
            moneyText.text = "Money: " + money.ToString("F2") + "$";
     
        if (playerPoints == 21 && gameStarted) {
            blackjackPanel.SetActive(true);
            money += wager;
            moneyText.text = "Money: " + money.ToString("F2") + "$";
            gameStarted = false;
        } else if (playerPoints > 21 && gameStarted && money > wager){
            bustedPanel.SetActive(true);
            money -= wager;
            moneyText.text = "Money: " + money.ToString("F2") + "$";
            gameStarted = false;
        }
        else if (playerPoints > 21 && gameStarted && money <= wager){
            gameOverPanel.SetActive(true);
            money -= wager;
            moneyText.text = "Money: " + money.ToString("F2") + "$";
            gameStarted = false;
        }

        if(deckIndex == cards.Length){
            deckIndex = 0;
            Shuffle();
        }

    }


        // Used for shuffling our deck
        void Shuffle() {
        Random.seed = (int)System.DateTime.Now.Ticks;


            for (int i = 0; i < cards.Length; i++) {
                int randomIndex = Random.Range(0, 51);


                while(cards[i] != tempCards[randomIndex]) {

                    if(tempCards[randomIndex] == null) {
                        tempCards[randomIndex] = cards[i];
                        tempCardValues[randomIndex] = cardValues[i];
                        tempAce[randomIndex] = ace[i];
                    } else {

                    if (randomIndex < cards.Length - 1)
                        randomIndex++;
                    else if (randomIndex == cards.Length - 1)
                        randomIndex = 0;
                }


                }
            }

            for (int x  = 0; x < cards.Length; x ++)
        {
            cards[x] = tempCards[x];
            tempCards[x] = null;
            cardValues[x] = tempCardValues[x];
            tempCardValues[x] = 0;
            ace[x] = tempAce[x];
            tempAce[x] = false;

        }


        }

    public void playerDrawCard(){
        GameObject.Find ("Player Hand").GetComponent<Transform> ().GetChild (playerHandIndex).gameObject.SetActive (true);

        //cardObject = (GameObject)Instantiate (cardPrefab);
        //cardObject.transform.parent = GameObject.Find("PlayerCards").transform;  

        GameObject.Find ("Player Hand").GetComponent<Transform> ().GetChild (playerHandIndex).GetComponent<Image> ().sprite = cards [deckIndex];

        if (playerPoints < 11 && cardValues [deckIndex] == 11 || cardValues[deckIndex] != 11)
              playerPoints += cardValues[deckIndex];
        else if (playerPoints >= 11 && cardValues [deckIndex] == 11)
            playerPoints += 1;

            deckIndex++;


        GameObject.Find ("Player Hand").GetComponent<Transform>().GetChild (playerHandIndex).GetComponent<AudioSource> ().Play ();
        playerHandIndex++;
    }

    public void dealerDrawCard() {
        //cardObject = (GameObject)Instantiate(cardPrefab);
        //cardObject.transform.parent = GameObject.Find("DealerCards").transform;
        //cardObject.GetComponent<Image>().sprite = backOfCard;

        GameObject.Find("Dealer Hand").GetComponent<Transform>().GetChild(dealerHandIndex).gameObject.SetActive(true);
        dealerPoints += cardValues[deckIndex];
        dealerCards[dealerHandIndex] = cards[deckIndex];
        dealerHandIndex++;
        deckIndex++;
    }

    public void standing() {
        hitButton.SetActive(false);
        standButton.SetActive(false);
        dealerPointsText.text = "Dealer Points: " + dealerPoints.ToString();

        for (int i = 0; i < dealerHand.transform.childCount; i++)
        {
            dealerHand.transform.GetChild(i).GetComponent<Image>().sprite = dealerCards [i];
        }

        // if the player wins
        if (playerPoints > dealerPoints)
        {
            winPanel.SetActive(true);
            money += wager;
        }

        // if the player loses but still has more money
        if (playerPoints < dealerPoints && money > wager)
        {
            losePanel.SetActive(true);
            money -= wager;
        }

        // if the player ties with the dealer
        if (playerPoints == dealerPoints)
        {
            tiePanel.SetActive(true);
        }

        // if the player loses and has no more money
        if (playerPoints < dealerPoints && money <= wager)
        {
            gameOverPanel.SetActive(true);
            money -= wager;
        }

        gameStarted = false;

    }

    public float getNewBet()
    {
        float value = 0f;

        try
        {
            value = float.Parse(betInput.GetComponent<InputField> ().text);
            errorText.SetActive(false);
        }

        catch(System.FormatException e)
        {
            errorText.SetActive(true);
        }

        if(value > money)
        {
            value = money;
            betInput.GetComponent<InputField>().text = value.ToString("F2");
        }

        return value;
    }

    public void changeBet(){
        wager = getNewBet ();
    }

    //check here to see about keeping win money instead of restarting at $100
    public void restart(bool restartMoney) {

        for (int i = 0; i < GameObject.Find("Player Hand").transform.childCount; i++) {
            GameObject.Find("Player Hand").transform.GetChild(i).gameObject.SetActive(false);
        }

        GameObject.Find("DealerCards").transform.GetChild (0).gameObject.GetComponent<Image> ().sprite = backOfCard;
        GameObject.Find("DealerCards").transform.GetChild (1).gameObject.GetComponent<Image> ().sprite = backOfCard;
        GameObject.Find("Dealer Hand").transform.GetChild (0).gameObject.SetActive(false);
        GameObject.Find("Dealer Hand").transform.GetChild (1).gameObject.SetActive(false);



        dealerPointsText.text = "Dealer Points: ";

        winPanel.SetActive(false);
        losePanel.SetActive(false);
        tiePanel.SetActive(false);
        bustedPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        blackjackPanel.SetActive(false);

        playerPoints = 0;
        dealerPoints = 0;
        dealerHandIndex = 0;
        playerHandIndex = 0;

        if (restartMoney)
            money = 100f;

        betPanel.SetActive(true);
        hitButton.SetActive(true);
        standButton.SetActive(true);
    }
}

