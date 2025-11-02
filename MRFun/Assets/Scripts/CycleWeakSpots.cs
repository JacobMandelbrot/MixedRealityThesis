using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CycleWeakSpots : MonoBehaviour
{
    [Header("Weak Spot Configuration")]
    [Tooltip("Array of all weak spot game objects")]
    public GameObject[] weakSpots;

    private List<GameObject> inactiveWeakSpots = new List<GameObject>();

    void Start()
    {
        // Deactivate all weak spots at the start
        foreach (GameObject weakSpot in weakSpots)
        {
            if (weakSpot != null)
            {
                weakSpot.SetActive(false);
                inactiveWeakSpots.Add(weakSpot);
            }
        }
        
        ActivateRandomWeakSpot();

        Debug.Log($"WeakSpotManager initialized with {weakSpots.Length} weak spots. All deactivated.");
    }

    /// <summary>
    /// Activates a random inactive weak spot from the array
    /// </summary>
    public void ActivateRandomWeakSpot()
    {
        // Update the list of inactive weak spots
        inactiveWeakSpots = weakSpots.Where(ws => ws != null && !ws.activeSelf).ToList();

        if (inactiveWeakSpots.Count == 0)
        {
            Debug.LogWarning("No inactive weak spots available to activate!");
            return;
        }

        // Select a random inactive weak spot
        int randomIndex = Random.Range(0, inactiveWeakSpots.Count);
        GameObject selectedWeakSpot = inactiveWeakSpots[randomIndex];

        // Activate it
        selectedWeakSpot.SetActive(true);
        Debug.Log($"Activated weak spot: {selectedWeakSpot.name}");
    }

    /// <summary>
    /// Gets the count of currently active weak spots
    /// </summary>
    public int GetActiveWeakSpotCount()
    {
        return weakSpots.Count(ws => ws != null && ws.activeSelf);
    }

    /// <summary>
    /// Gets the count of currently inactive weak spots
    /// </summary>
    public int GetInactiveWeakSpotCount()
    {
        return weakSpots.Count(ws => ws != null && !ws.activeSelf);
    }
}
