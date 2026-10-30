using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class CollectibleManager : MonoBehaviour
{
    public int diamondCount = 0;
    public int targetDiamonds = 2;
    public int points = 0;
    public int targetPoints = 800;

    public int itemCount
    {
        get => diamondCount;
        set => diamondCount = value;
    }

    public int targetCount
    {
        get => targetDiamonds;
        set => targetDiamonds = value;
    }

    public TextMeshProUGUI countText; 
    public GameObject winPanel;       

    private bool gameWon = false;

    private void Start()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (countText != null)
        {
            countText.text = "Diamonds: " + diamondCount + " / " + targetDiamonds + "\nPoints: " + points + " / " + targetPoints;
        }

        if (diamondCount >= targetDiamonds && points >= targetPoints && !gameWon)
        {
            gameWon = true;
            winPanel.SetActive(true);
            Time.timeScale = 0f;      
        }

        if (gameWon && Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
