using Meta.XR.BuildingBlocks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static OVRSpatialAnchor;

public class MRPlaceAndPersistEnv : MonoBehaviour
{
    public GameObject anchorObj;
    public GameObject EnvObj;
    public GameObject EnvObjPrefab;
    public List<Guid> GUIDs = new List<Guid>();
    public List<OVRSpatialAnchor> Anchors = new List<OVRSpatialAnchor>();
    private Transform _anchorPrefabTransform;
    public OVRCameraRig _cameraRig;
    public SpatialAnchorSpawnerBuildingBlock AnchorSpawner;
    public SpatialAnchorCoreBuildingBlock anchorCore;
    public SpatialAnchorLoaderBuildingBlock anchorLoader;
    public NotificationData notificationData;

    public const string EnvPosGuid = "EnvPosGuid";
    public const string EnvRotGuid = "EnvRotGuid";

    public bool isEnvLoaded = false;

    public ControllerButtonsMapper controllerButtonsMapper;
    private void Start()
    {
        _anchorPrefabTransform = _cameraRig.rightControllerAnchor.transform.GetChild(0);
        _anchorPrefabTransform.localPosition = Vector3.zero;
        _anchorPrefabTransform.localRotation = Quaternion.identity;
      ///  anchorCore.OnAnchorsEraseAllCompleted.AddListener(ClearGUID);
        anchorCore.OnAnchorCreateCompleted.AddListener(FindGUIDs);
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            ClearGUID();
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            LoadEnv();
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            SaveGUIDs();
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            CreateAnchor();
        }


    }
    public void LoadEnv()
    {
        if (isEnvLoaded) return;

        LoadAndSpawn(EnvPosGuid, EnvObj);
        LoadAndSpawn(EnvRotGuid, EnvObjPrefab);

        StartCoroutine(Rotate());
        isEnvLoaded = true;
    }

    private void LoadAndSpawn(string guidKey, GameObject prefab)
    {
        if (!Guid.TryParse(PlayerPrefs.GetString(guidKey), out Guid guid))
        {
            Debug.LogWarning("Invalid GUID string for " + guidKey);
            return;
        }

        GUIDs.Add(guid);
        var guidList = new List<Guid> { guid };

        anchorCore.LoadAndInstantiateAnchors(anchorObj, guidList);
        StartCoroutine(SpawnAfterAnchorLoad(guid, prefab));
    }

    private IEnumerator SpawnAfterAnchorLoad(Guid guid, GameObject prefab)
    {
        OVRSpatialAnchor anchor = null;
        float t = 0;

        while (anchor == null && t < 10f)
        {
            anchor = FindAnchorByGuid(guid)?.GetComponent<OVRSpatialAnchor>();
            t += Time.deltaTime;
            yield return null;
        }

        if (anchor == null)
        {
            Debug.LogError("Anchor not found after timeout");
            yield break;
        }

        var obj = Instantiate(prefab, anchor.transform.position, Quaternion.identity);
        obj.transform.SetParent(anchor.transform);

        Anchors.Add(anchor);
    }

    public GameObject FindAnchorByGuid(Guid uuid) =>
      FindObjectsOfType<OVRSpatialAnchor>()
          .FirstOrDefault(a => a.Uuid == uuid)?.gameObject;

    #region Create Anchor
    public void FindGUIDs(OVRSpatialAnchor loadedAnchor , OperationResult result)
    {
        if (result == OperationResult.Success)
        {
            Guid uuid = loadedAnchor.Uuid;
            Anchors.Add(loadedAnchor);
            GUIDs.Add(uuid);
          //  notificationData.ShowPopUp(0);

        }
      
    }

    public void CreateAnchor()
    {
        if(Anchors.Count < 2)
        {
            AnchorSpawner.SpawnSpatialAnchor(_anchorPrefabTransform.position, _anchorPrefabTransform.rotation);
            notificationData.ShowPopUp(0);
        }
        else
        {
            Debug.LogError("2 anchor Created");
        }
    }

    #endregion

    public void ClearGUID(OperationResult result)
    {
        if (result == OperationResult.Success)
        {
            /* anchorCore.EraseAnchorByUuid(GUIDs[0]);
             anchorCore.EraseAnchorByUuid(GUIDs[1]);
             PlayerPrefs.DeleteKey(EnvPosGuid);
             PlayerPrefs.DeleteKey(EnvRotGuid);
             notificationData.ShowPopUp(2);
             GUIDs.Clear();
             Anchors.Clear();
             isEnvLoaded = false;*/

            notificationData.ShowPopUp(0);

        }

    }
    public void ClearGUID()
    {
/*        if (Anchors.Count > 0)
        {
            for (int i = 0; i < Anchors.Count; i++)
            {
                anchorCore.EraseAnchorByUuid(GUIDs[i]);
            }
            GUIDs.Clear();
            Anchors.Clear();
        }
*/
        PlayerPrefs.DeleteKey(EnvPosGuid);
        PlayerPrefs.DeleteKey(EnvRotGuid);
        notificationData.ShowPopUp(2);
        anchorCore.EraseAllAnchors();

        isEnvLoaded = false;
    }

    public void SaveGUIDs()
    {
        if (GUIDs.Count < 2)
        {
            Debug.LogWarning("Not enough GUIDs to save!");
            return;
        }

        PlayerPrefs.SetString(EnvPosGuid, GUIDs[0].ToString());
        PlayerPrefs.SetString(EnvRotGuid, GUIDs[1].ToString());
        PlayerPrefs.Save();

        notificationData.ShowPopUp(1);
    }
    IEnumerator Rotate()
    {
        float t = 0f;

        while (Anchors.Count < 2 && t < 10f)
        {
            t += Time.deltaTime;
            yield return null;
        }

        FindAnyObjectByType<EnvAutoAlign>()?.Rotate();

        if (Anchors.Count > 0)
        {
            notificationData.ShowPopUp(3);
            controllerButtonsMapper.enabled = false;
        }
    }

}