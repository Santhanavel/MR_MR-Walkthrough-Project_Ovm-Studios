using Meta.XR.BuildingBlocks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using static OVRSpatialAnchor;

public class MRPlaceAndAnchor : MonoBehaviour
{
    public List<GameObject> mAnchorsComp = new List<GameObject>();

    [Header("Prefab To Spawn")]
    public GameObject spawnPrefab;

    [Header("Anchor Building Blocks")]
    public SpatialAnchorLoaderBuildingBlock anchorLoader;
    public SpatialAnchorCoreBuildingBlock anchorCore;

    [Header("Events")]
    public UnityEvent OnMultipleAnchorsFound;

    [Header("Debug")]
    public List<OVRSpatialAnchor> anchors = new List<OVRSpatialAnchor>();

    private bool placementEnabled = true;



    public void EnablePlaceingAnchor()
    {
        foreach (GameObject gameObject in mAnchorsComp)
        {
            gameObject.SetActive(true);
        }
        // Delete anchors from world + storage
        anchorCore.EraseAllAnchors();
        anchors.Clear();
    }

    public void DisablePlaceingAnchor()
    {
        foreach (GameObject gameObject in mAnchorsComp)
        {
            gameObject.SetActive(false);
        }
    }
    private void Awake()
    {
        // Bind loader event
        anchorCore.OnAnchorCreateCompleted.AddListener(OnAnchorCreateCompleted);
        anchorCore.OnAnchorsLoadCompleted.AddListener(OnSavedAnchorsLoaded);

    }
    public void LoadAnchorEnv()
    {
        // Load anchors from local storage
        anchorLoader.LoadAnchorsFromDefaultLocalStorage();

    }
    public void PlaceEnvironment()
    {
        if (anchors.Count > 1)
        {
            // MULTIPLE ANCHORS FOUND
            Debug.Log("Multiple anchors found → clearing all.");

            OnMultipleAnchorsFound?.Invoke();

            // Delete anchors from world + storage
            anchorCore.EraseAllAnchors();
            anchors.Clear();
            placementEnabled = true; // allow new placement
        }
        else if (anchors.Count == 1)
        {
            // ONLY ONE ANCHOR FOUND → Spawn object at anchor
            Debug.Log("One anchor found → spawning object.");

            Instantiate(spawnPrefab, anchors[0].transform.position, Quaternion.identity);
            placementEnabled = false; // lock placement
        }
    }

    public void OnSavedAnchorsLoaded(IReadOnlyList<OVRSpatialAnchor> loadedAnchors)
    {
        anchors.Clear();

        foreach (var anchor in loadedAnchors)
        {
            anchors.Add(anchor);
        }

        Debug.Log("Loaded Anchors Count = " + anchors.Count);

        if (anchors.Count > 1)
        {
            // MULTIPLE ANCHORS FOUND
            Debug.Log("Multiple anchors found → clearing all.");

            OnMultipleAnchorsFound?.Invoke();

            // Delete anchors from world + storage
            anchorCore.EraseAllAnchors();
            anchors.Clear();
            placementEnabled = true; // allow new placement
        }
        else if (anchors.Count == 1)
        {
            // ONLY ONE ANCHOR FOUND → Spawn object at anchor
            Debug.Log("One anchor found → spawning object.");

            Instantiate(spawnPrefab, anchors[0].transform.position, Quaternion.identity);
            placementEnabled = false; // lock placement
        }
    }
    public void OnAnchorCreateCompleted(OVRSpatialAnchor anchor, OperationResult result)
    {
        if (result != OperationResult.Success)
        {
            Debug.LogWarning("Anchor creation failed: " + result);
            return;
        }

        anchors.Add(anchor);
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid leaks
        if (anchorCore != null)
        {
            anchorCore.OnAnchorCreateCompleted.RemoveListener(OnAnchorCreateCompleted);
            anchorCore.OnAnchorsLoadCompleted.RemoveListener(OnSavedAnchorsLoaded);
        }
    }

}