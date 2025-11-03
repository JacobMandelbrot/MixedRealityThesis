using System;
using System.Collections.Generic;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class DemoGameManager : MonoBehaviour
{
    public static DemoGameManager Instance { get; private set; }
    
    private AudioSource audioSource;

    // [Header("Spawn Points")]
    // public Transform modelSpawnPoint;
    // public Transform lightSpawnPoint;

    [Header("Prefab Lists")]
    public GameObject[] modelPrefabs;
    public GameObject[] lightPrefabs;
    
    [Header("Dummy Spawning")]
    public GameObject dummyPrefab;

    // Track the instantiated objects in the scene
    private List<GameObject> spawnedModels = new List<GameObject>();
    private List<GameObject> spawnedLights = new List<GameObject>();
    
    // Current active indices
    private int currentModelIndex = 0; // Start on third element (index 2)
    private int currentLightIndex = 0;  // Start on third element (index 2)
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeObjects();
    }

    // Instantiate all models and lights, but only activate the third one
    private void InitializeObjects()
    {
        modelPrefabs[0].SetActive(true);
        lightPrefabs[0].SetActive(true);
        dummyPrefab.SetActive(false);
        // Spawn all models
        for (int i = 1; i < modelPrefabs.Length; i++)
        {
            modelPrefabs[i].SetActive(false);
            // if (modelPrefabs[i] != null && modelSpawnPoint != null)
            // {
            //     GameObject instance = Instantiate(modelPrefabs[i], 
            //         modelSpawnPoint.position, 
            //         modelSpawnPoint.rotation);
            //     
            //     instance.SetActive(i == currentModelIndex); // Only activate the third one
            //     spawnedModels.Add(instance);
            // }
        }

        // Spawn all lights
        for (int i = 1; i < lightPrefabs.Length; i++)
        {
            lightPrefabs[i].SetActive(false);
            // if (lightPrefabs[i] != null && lightSpawnPoint != null)
            // {
            //     GameObject instance = Instantiate(lightPrefabs[i], 
            //         lightSpawnPoint.position, 
            //         lightSpawnPoint.rotation);
            //     
            //     instance.SetActive(i == currentLightIndex); // Only activate the third one
            //     spawnedLights.Add(instance);
            // }
        }
    }

    public void PlaySound(AudioClip soundEffect)
    {
        if (soundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
    }

    public void ChangeModel()
    {
        dummyPrefab.SetActive(false);
        // Deactivate current model
        if (modelPrefabs.Length > 0 && currentModelIndex < modelPrefabs.Length)
        {
            modelPrefabs[currentModelIndex].SetActive(false);
        }

        // Move to next index (loop back to 0 if at the end)
        currentModelIndex = (currentModelIndex + 1) % modelPrefabs.Length;

        // Activate new model
        if (modelPrefabs.Length > 0 && currentModelIndex < modelPrefabs.Length)
        {
            modelPrefabs[currentModelIndex].SetActive(true);
        }
    }

    public void ChangeLight()
    {
        // Deactivate current light
        if (lightPrefabs.Length > 0 && currentLightIndex < lightPrefabs.Length)
        {
            lightPrefabs[currentLightIndex].SetActive(false);
        }

        // Move to next index (loop back to 0 if at the end)
        currentLightIndex = (currentLightIndex + 1) % lightPrefabs.Length;

        // Activate new light
        if (lightPrefabs.Length > 0 && currentLightIndex < lightPrefabs.Length)
        {
            lightPrefabs[currentLightIndex].SetActive(true);
        }
    }

    public void SpawnDummy()
    {
        if (dummyPrefab != null)
        {
            // Make current model inactive
            if (modelPrefabs.Length > 0 && currentModelIndex < modelPrefabs.Length)
            {
                modelPrefabs[currentModelIndex].SetActive(false);
            }

            // Toggle the dummy
            dummyPrefab.SetActive(!dummyPrefab.activeSelf);
        }
    }
}