using Meta.XR.BuildingBlocks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MRPlaceAndAnchor : MonoBehaviour
{
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

    private void Awake()
    {
        // Load anchors from local storage
        anchorLoader.LoadAnchorsFromDefaultLocalStorage();
        // Bind loader event
        anchorCore.OnAnchorsLoadCompleted.AddListener(OnSavedAnchorsLoaded);

    }

    private void OnSavedAnchorsLoaded(IReadOnlyList<OVRSpatialAnchor> loadedAnchors)
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

            Instantiate(spawnPrefab, anchors[0].transform.position,Quaternion.identity);
            placementEnabled = false; // lock placement
        }
    }
}