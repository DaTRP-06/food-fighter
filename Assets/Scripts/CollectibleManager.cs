using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class CollectibleManager : MonoBehaviour
{
    public int itemCount = 0;
    public int targetCount = 5;

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
            countText.text = "Saved: " + itemCount + " / " + targetCount;
        }

        if (itemCount >= targetCount && !gameWon)
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