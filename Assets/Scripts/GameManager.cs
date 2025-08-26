using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    [Header("References")]
    public LayoutHandler layoutManager;     // For grid arrangement
    public RectTransform cardParent;        // Container panel for cards
    public GameObject cardPrefab;           // Card prefab to spawn
    public TextMeshProUGUI scoreText;                  // UI text for score display

    [Header("Board Settings")]
    public int rows = 4;
    public int cols = 4;

    private List<int> cardValues = new List<int>();
    private List<CardUI> cardsOnBoard = new List<CardUI>();

    private CardUI firstCard = null;
    private CardUI secondCard = null;

    private int score = 0;

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        // Clear previous board if regenerating
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }
        cardsOnBoard.Clear();

        // Prepare IDs (pairs)
        cardValues.Clear();
        int totalCards = rows * cols;

        for (int i = 0; i < totalCards / 2; i++)
        {
            cardValues.Add(i);
            cardValues.Add(i);
        }

        // Shuffle
        cardValues = cardValues.OrderBy(x => Random.value).ToList();

        // Apply layout
        layoutManager.SetLayout(rows, cols);

        // Spawn cards
        // for (int i = 0; i < cardValues.Count; i++)
        // {
        //     GameObject cardObj = Instantiate(cardPrefab, cardParent);
        //     CardUI card = cardObj.GetComponent<CardUI>();
        //     card.Initialize(cardValues[i]);
        //     cardsOnBoard.Add(card);
        // }
        for (int i = 0; i < cardValues.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardParent);
            CardUI card = cardObj.GetComponent<CardUI>();

            // Assign shuffled ID
            card.Initialize(cardValues[i]);

            cardsOnBoard.Add(card);

            Debug.Log("Spawning card with ID: " + cardValues[i]); // Debug check
        }

        score = 0;
        UpdateScore();
        SaveGame();
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.score = score;
        data.rows = rows;
        data.cols = cols;
        data.cardIDs = new List<int>(cardValues);

        // Track matched cards (by index)
        data.matchedIndices = new List<int>();
        for (int i = 0; i < cardsOnBoard.Count; i++)
        {
            if (cardsOnBoard[i] != null && cardsOnBoard[i].IsMatched())
                data.matchedIndices.Add(i);
        }

        SaveSystem.SaveGame(data);
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.LoadGame();
        if (data == null)
        {
            GenerateBoard();
            return;
        }

        rows = data.rows;
        cols = data.cols;
        cardValues = new List<int>(data.cardIDs);

        // Clear old board
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }
        cardsOnBoard.Clear();

        // Layout
        layoutManager.SetLayout(rows, cols);

        // Spawn cards
        for (int i = 0; i < cardValues.Count; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardParent);
            CardUI card = cardObj.GetComponent<CardUI>();
            card.Initialize(cardValues[i]);

            // If it was matched before, reveal it immediately
            if (data.matchedIndices.Contains(i))
                card.SetMatched();

            cardsOnBoard.Add(card);
        }

        score = data.score;
        UpdateScore();

        Debug.Log("Game Loaded!");
    }



    public void OnCardFlipped(CardUI card)
    {
        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;

            // Now check match
            StartCoroutine(CheckMatch());
        }
    }


    private System.Collections.IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(0.5f); // small delay to see flip

        if (firstCard.cardID == secondCard.cardID)
        {
            // Match
            firstCard.SetMatched();
            secondCard.SetMatched();
            score += 10;
        }
        else
        {
            // Mismatch → flip back
            firstCard.FlipBack();
            secondCard.FlipBack();
            // score -= 2; // penalty (optional)
        }

        UpdateScore();

        firstCard = null;
        secondCard = null;

        if (cardsOnBoard.All(c => c.gameObject.activeSelf && c.enabled && c.cardID >= 0 && c.transform.localScale != Vector3.zero && !c.GetComponent<CardUI>().Equals(null) && c.GetComponent<CardUI>().enabled && c.GetComponent<CardUI>().gameObject.activeSelf && c.GetComponent<CardUI>().cardID >= 0 && c.GetComponent<CardUI>().front.activeSelf))
        {
            Debug.Log("🎉 Game Over! You Win!");
        }
    }


    private void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}