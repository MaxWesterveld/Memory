using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameStatus
{
    waiting_on_first_card,
    waiting_on_second_card,
    match_found,
    no_match_found
}

public class Game : MonoBehaviour
{
    [SerializeField] private int rows;
    [SerializeField] private int collumns;
    [SerializeField] private int totalPairs;
    [SerializeField] private string frontSidesFolder;
    [SerializeField] private string backSidesFolder;
    [SerializeField] Sprite[] frontSprite;
    [SerializeField] Sprite[] backSprite;
    [SerializeField] List<Sprite> selectedFrontSprite;
    [SerializeField] Sprite selectedBackSprite;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Stack<GameObject> stackOfCards;
    [SerializeField] GameObject[,] placedCards;
    [SerializeField] GameObject fieldAnchor;
    [SerializeField] float xOffset;
    [SerializeField] float yOffset;
    [SerializeField] GameObject[] selectedCards;
    [SerializeField] GameStatus status;
    [SerializeField] float timeoutTimer;
    [SerializeField] float timeoutTarget;

    private void Start()
    {
        MakeCards();
        DistributeCards();
        selectedCards = new GameObject[2];
        status = GameStatus.waiting_on_first_card;
    }

    private void MakeCards()
    {
        CalculateAmountOfPairs();
        LoadSprites();
        SelectFrontSprite();
        SelectBackSprite();
        ConstructCards();
    }

    private void DistributeCards()
    {
        placedCards = new GameObject[collumns, rows];
        ShuffleCards();
        PlaceCardsOnField();
    }

    private void CalculateAmountOfPairs()
    {
        if (rows * collumns % 2 == 0)
        {

        }
    }

    private void LoadSprites()
    {
        frontSprite = Resources.LoadAll<Sprite>(frontSidesFolder);
        backSprite = Resources.LoadAll<Sprite>(backSidesFolder);
    }

    private void SelectFrontSprite()
    {
        selectedFrontSprite = new List<Sprite>
        {

        };
        while (selectedFrontSprite.Count < totalPairs)
        {
            int l_random = Random.Range(0, frontSprite.Length);
            if (!selectedFrontSprite.Contains(frontSprite[l_random]))
            {
                selectedFrontSprite.Add(frontSprite[l_random]);
            }
        }
    }

    private void SelectBackSprite()
    {
        if (backSprite.Length > 0)
        {
            int l_random = Random.Range(0, backSprite.Length);
            selectedBackSprite = backSprite[l_random];
        }
        else
        {
            Debug.LogError("There is no backcard to select");
        }
    }

    public void ConstructCards()
    {
        stackOfCards = new Stack<GameObject>();

        GameObject l_parent = new GameObject();
        l_parent.name = "Cards";

        foreach (Sprite l_selectedFrontSprite in selectedFrontSprite)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject l_go = Instantiate<GameObject>(cardPrefab);
                Card l_cardScript = l_go.GetComponent<Card>();

                l_cardScript.SetBack(selectedBackSprite);
                l_cardScript.SetFront(l_selectedFrontSprite);

                l_go.name = l_selectedFrontSprite.name;
                l_go.transform.parent = l_parent.transform;

                stackOfCards.Push(l_go);
            }
        }
    }

    private void ShuffleCards()
    {
        while (stackOfCards.Count > 0)
        {
            int l_sideX = Random.Range(0, collumns);
            int l_sideY = Random.Range(0, rows);

            if (placedCards[l_sideX, l_sideY] == null)
            {
                print("card " + stackOfCards.Peek().name + " is placed on x: " + l_sideX + " y: " + l_sideY);
                placedCards[l_sideX, l_sideY] = stackOfCards.Pop();
            }
        }
    }

    private void PlaceCardsOnField()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < collumns; x++)
            {
                GameObject l_card = placedCards[x, y];

                Card l_cardScript = l_card.GetComponent<Card>();

                Vector2 l_cardSize = l_cardScript.GetBackSize();

                float l_xPos = fieldAnchor.transform.position.x + (x * (l_cardSize.x + xOffset));
                float l_yPos = fieldAnchor.transform.position.y + (y * (l_cardSize.y + yOffset));

                placedCards[x, y].transform.position = new Vector3(l_xPos, l_yPos, 0f);
            }
        }
    }

    public void SelectCard(GameObject l_card)
    {
        if (status == GameStatus.waiting_on_first_card)
        {
            selectedCards[0] = l_card;
            status = GameStatus.waiting_on_second_card;
        }
        else if (status == GameStatus.waiting_on_second_card)
        {
            selectedCards[1] = l_card;
            CheckForMatchingPair();
        }
    }

    private void CheckForMatchingPair()
    {
        timeoutTimer = 0f;
        if (selectedCards[0].name == selectedCards[1].name)
        {
            status = GameStatus.match_found;
            RotateBackOrRemovePair();
        }
        else
        {
            status = GameStatus.no_match_found;
            RotateBackOrRemovePair();
        }
    }

    private void RotateBackOrRemovePair()
    {
        timeoutTimer = timeoutTimer + 1;
        if (timeoutTimer >= timeoutTarget)
        {
            if (status == GameStatus.match_found)
            {
                selectedCards[0].SetActive(false);
                selectedCards[1].SetActive(false);
            }
            if (status == GameStatus.no_match_found)
            {
                selectedCards[0].GetComponent<Card>().TurnToBack();
                selectedCards[1].GetComponent<Card>().TurnToBack();
            }
            selectedCards[0] = null;
            selectedCards[1] = null;

            status = GameStatus.waiting_on_first_card;
        }

    }

    public bool AllowedToSelectCard(Card l_card)
    {
        if (selectedCards[0] == null)
        {
            return true;
        }
        if (selectedCards[1] == null)
        {
            if (selectedCards[0] != l_card.gameObject)
            {
                return true;
            }
        }
        return false;
    }
}