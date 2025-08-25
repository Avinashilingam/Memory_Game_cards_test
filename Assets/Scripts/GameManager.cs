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
        layoutManager.SpawnCards(totalCards);

        score = 0;
        UpdateScore();
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
            score -= 2; // penalty (optional)
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