using System;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private AudioSource audioSource;

    public GameObject[] spawnPoints;

    [Serializable]
    public class Entry
    {
        public string key;
        public GameObject[] objects;
    }

    [SerializeField]
    private List<Entry> entries = new List<Entry>();
    
    public Dictionary<string, GameObject[]> objectDictionary = new Dictionary<string, GameObject[]>();

    // This now tracks the spawned instances in the scene
    private List<GameObject> currentGameObjects = new List<GameObject>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            BuildDictionary();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void BuildDictionary()
    {
        objectDictionary.Clear();
        foreach (Entry entry in entries)
        {
            if (!string.IsNullOrEmpty(entry.key) && entry.objects != null)
            {
                objectDictionary[entry.key] = entry.objects;
            }
        }
        Debug.Log($"Dictionary built with {objectDictionary.Count} keys");
    }
    
    public void PlaySound(AudioClip soundEffect)
    {
        if (soundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
    }

    public void ChangeMode(string newMode)
    {
        // Destroy all currently spawned instances
        foreach (GameObject currentObj in currentGameObjects)
        {
            Destroy(currentObj);
        }
        currentGameObjects.Clear(); // Clear the list
    
        // Get the prefabs for the new mode and spawn them
        GameObject[] prefabsToSpawn = objectDictionary[newMode];
        SpawnAtPoints(prefabsToSpawn);
    }

    // Spawn objects at corresponding spawn points
    public void SpawnAtPoints(GameObject[] objectsToSpawn)
    {
        int spawnCount = Mathf.Min(objectsToSpawn.Length, spawnPoints.Length);
        
        for (int i = 0; i < spawnCount; i++)
        {
            if (objectsToSpawn[i] != null && spawnPoints[i] != null)
            {
                GameObject instance = Instantiate(objectsToSpawn[i], 
                    spawnPoints[i].transform.position, 
                    spawnPoints[i].transform.rotation);
                
                currentGameObjects.Add(instance); // Add the spawned instance
            }
        }
    }
}