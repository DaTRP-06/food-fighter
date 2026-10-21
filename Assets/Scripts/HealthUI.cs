using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI Configuration")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartContainer;

    [Header("Blink Settings")]
    [SerializeField] private int blinkCount = 3;
    [SerializeField] private float blinkInterval = 0.08f;

    private List<Image> heartImages = new List<Image>();

    public void InitializeHearts(int maxHealth)
    {
        // Bersihkan ikon lama jika ada
        foreach (Transform child in heartContainer)
        {
            Destroy(child.gameObject);
        }
        heartImages.Clear();

        // Gandakan ikon hati sesuai jumlah maxHealth
        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            Image img = heart.GetComponent<Image>();
            if (img != null)
            {
                heartImages.Add(img);
            }
        }
    }

    public void UpdateHearts(int currentHealth)
    {
        // Indeks hati yang berkurang sesuai dengan sisa health
        int lostHeartIndex = currentHealth;

        if (lostHeartIndex >= 0 && lostHeartIndex < heartImages.Count)
        {
            StartCoroutine(BlinkAndRemoveHeart(heartImages[lostHeartIndex]));
        }
    }

    private IEnumerator BlinkAndRemoveHeart(Image heartImage)
    {
        for (int i = 0; i < blinkCount; i++)
        {
            heartImage.enabled = false;
            yield return new WaitForSeconds(blinkInterval);
            heartImage.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
        }

        heartImage.enabled = false;
    }
}