using Meta.XR.BuildingBlocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static OVRSpatialAnchor;

public class MRPlaceAndPersistEnv : MonoBehaviour
{
    [Header("Prefabs / References")]
    public GameObject anchorObj;
    public GameObject EnvObj;
    public GameObject EnvObjPrefab;

    [Header("References / Scripts")]

    public OVRCameraRig _cameraRig;
    public SpatialAnchorSpawnerBuildingBlock AnchorSpawner;
    public SpatialAnchorCoreBuildingBlock anchorCore;

    public NotificationData notificationData;
    public ControllerButtonsMapper controllerButtonsMapper;

    [Header("Runtime Data")]
    private Transform _anchorPrefabTransform;
    public List<Guid> GUIDs = new();
    public List<OVRSpatialAnchor> Anchors = new();

    private bool isEnvLoaded = false;
    private bool isSaved = false;
    private bool isInPlacement = false;

    public const string EnvPosGuid = "EnvPosGuid";
    public const string EnvRotGuid = "EnvRotGuid";

    private void Start()
    {
        _anchorPrefabTransform = _cameraRig.rightControllerAnchor.transform.GetChild(0);
        _anchorPrefabTransform.localPosition = Vector3.zero;
        _anchorPrefabTransform.localRotation = Quaternion.identity;

        anchorCore.OnAnchorCreateCompleted.AddListener(OnAnchorCreated);

        isSaved = PlayerPrefs.HasKey(EnvPosGuid) && PlayerPrefs.HasKey(EnvRotGuid);
    }
    // ----------------------------------------------------------
    // 1️⃣ CREATE ANCHORS
    // ----------------------------------------------------------
    #region CREATE ANCHORS

    public void CreateAnchor()
    {
        if (isSaved)
        {
            notificationData.Show(NotificationType.Error, "Anchors already saved!");
            return;
        }

        if (Anchors.Count >= 2)
        {
            notificationData.Show(NotificationType.Error, "Already created 2 anchors.");
            return;
        }

        isInPlacement = true;

        AnchorSpawner.SpawnSpatialAnchor(
            _anchorPrefabTransform.position,
            _anchorPrefabTransform.rotation
        );

        notificationData.Show(NotificationType.CreateAnchor);
    }

    private void OnAnchorCreated(OVRSpatialAnchor anchor, OperationResult result)
    {
        if (result != OperationResult.Success) return;

        Anchors.Add(anchor);
        GUIDs.Add(anchor.Uuid);

        if (Anchors.Count == 2)
            notificationData.Show(NotificationType.CreateAnchor, "Two anchors placed. You can now save.");
    }
    #endregion

    // ----------------------------------------------------------
    // 2️⃣ SAVE
    // ----------------------------------------------------------
    #region SAVE
    public void SaveGUIDs()
    {
        if (isSaved)
        {
            notificationData.Show(NotificationType.Error, "Already saved!");
            return;
        }

        if (Anchors.Count < 2)
        {
            notificationData.Show(NotificationType.Error, "Need exactly 2 anchors to save.");
            return;
        }

        PlayerPrefs.SetString(EnvPosGuid, GUIDs[0].ToString());
        PlayerPrefs.SetString(EnvRotGuid, GUIDs[1].ToString());
        PlayerPrefs.Save();

        isSaved = true;
        isInPlacement = false;

        notificationData.Show(NotificationType.SaveSuccess);
    }
    #endregion

    // ----------------------------------------------------------
    // 3️⃣ LOAD
    // ----------------------------------------------------------
    #region LOAD

    public void LoadEnv()
    {
        if (!isSaved)
        {
            notificationData.Show(NotificationType.Error, "No saved anchors. Place & save first.");
            return;
        }

        if (isEnvLoaded)
        {
            notificationData.Show(NotificationType.Error, "Environment already loaded.");
            return;
        }

        LoadAndSpawn(EnvPosGuid, EnvObj);
        LoadAndSpawn(EnvRotGuid, EnvObjPrefab);

        StartCoroutine(AutoRotate());
        isEnvLoaded = true;
    }

    private void LoadAndSpawn(string key, GameObject prefab)
    {
        if (!Guid.TryParse(PlayerPrefs.GetString(key), out Guid guid))
        {
            notificationData.Show(NotificationType.Error, "Saved anchor ID invalid!");
            return;
        }

        anchorCore.LoadAndInstantiateAnchors(anchorObj, new List<Guid> { guid });
        StartCoroutine(SpawnWhenReady(guid, prefab));
    }

    private IEnumerator SpawnWhenReady(Guid guid, GameObject prefab)
    {
        OVRSpatialAnchor anchor = null;
        float t = 0;

        while (anchor == null && t < 10)
        {
            anchor = FindAnchor(guid);
            t += Time.deltaTime;
            yield return null;
        }

        if (anchor == null)
        {
            notificationData.Show(NotificationType.Error, "Anchor load failed.");
            yield break;
        }
        GUIDs.Add(guid);
        Anchors.Add(anchor);

        var envObj = Instantiate(prefab, anchor.transform.position, Quaternion.identity);
        envObj.transform.SetParent(anchor.transform);
    }

    public OVRSpatialAnchor FindAnchor(Guid guid) =>
        FindObjectsOfType<OVRSpatialAnchor>().FirstOrDefault(a => a.Uuid == guid);
    #endregion

    // ----------------------------------------------------------
    // 4️⃣ AUTO ROTATE / ALIGN
    // ----------------------------------------------------------
    #region ALIGN
    private IEnumerator AutoRotate()
    {
        EnvAutoAlign alin = null;

        // Wait until EnvAutoAlign exists in scene
        while (alin == null)
        {
            alin = FindAnyObjectByType<EnvAutoAlign>();
            yield return null;
        }

        alin.Rotate();
        notificationData.Show(NotificationType.LoadSuccess);
       // controllerButtonsMapper.enabled = false;
    }
    #endregion
    // ----------------------------------------------------------
    // 5️⃣ DELETE
    // ----------------------------------------------------------
    #region DELETE
    public void ClearAll()
    {
        if (!isSaved && Anchors.Count == 0)
        {
            notificationData.Show(NotificationType.Error, "Nothing to delete.");
            return;
        }

        PlayerPrefs.DeleteKey(EnvPosGuid);
        PlayerPrefs.DeleteKey(EnvRotGuid);
        PlayerPrefs.Save();

        isSaved = false;
        isEnvLoaded = false;
        isInPlacement = false;

        GUIDs.Clear();

        anchorCore.EraseAllAnchors();

        foreach (var anchor in Anchors)
            if (anchor != null)
                Destroy(anchor.gameObject);

        Anchors.Clear();

        notificationData.Show(NotificationType.ClearSuccess);
    }
    #endregion
}
