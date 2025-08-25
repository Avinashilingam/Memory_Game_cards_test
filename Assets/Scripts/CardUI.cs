using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CardUI : MonoBehaviour
{
    [Header("Card Visuals")]
    public GameObject front;   // The front panel (with text or image)
    public GameObject back;    // The back panel (default visible)
    public TextMeshProUGUI frontText;     // The text that holds the card ID

    [Header("Card Data")]
    public int cardID;         // The value used for matching
    private bool isFlipped = false;
    private bool isMatched = false;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        ResetCard();
    }

    public void Initialize(int id)
    {
        cardID = id;
        frontText.text = id.ToString();
        ResetCard();
    }

    public void OnCardClicked()
    {
        if (isMatched || isFlipped) return;

        Flip();
        gameManager.OnCardFlipped(this);
    }

    public void Flip()
    {
        isFlipped = true;

        // Animate flip with DOTween
        transform.DORotate(new Vector3(0, 90, 0), 0.25f).OnComplete(() =>
        {
            back.SetActive(false);
            front.SetActive(true);
            transform.DORotate(new Vector3(0, 0, 0), 0.25f);
        });
    }

    public void FlipBack()
    {
        isFlipped = false;

        // Animate back flip
        transform.DORotate(new Vector3(0, 90, 0), 0.25f).OnComplete(() =>
        {
            back.SetActive(true);
            front.SetActive(false);
            transform.DORotate(new Vector3(0, 0, 0), 0.25f);
        });
    }

    public void SetMatched()
    {
        isMatched = true;

        // Optional: add a little "matched" bounce
        transform.DOScale(1.2f, 0.2f).OnComplete(() =>
        {
            transform.DOScale(1f, 0.2f);
        });
    }

    private void ResetCard()
    {
        isFlipped = false;
        isMatched = false;
        back.SetActive(true);
        front.SetActive(false);
        transform.rotation = Quaternion.identity;
    }
}