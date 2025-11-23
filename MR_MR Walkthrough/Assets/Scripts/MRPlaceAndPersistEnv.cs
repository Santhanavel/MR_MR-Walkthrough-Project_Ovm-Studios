using Meta.XR.BuildingBlocks;
using System;
using System.Collections;
using System.Collections.Generic;
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
    
    public SpatialAnchorCoreBuildingBlock anchorCore;
    public SpatialAnchorLoaderBuildingBlock anchorLoader;
    public NotificationData notificationData;

    public const string EnvPosGuid = "EnvPosGuid";
    public const string EnvRotGuid = "EnvRotGuid";

    public bool isEnvLoaded = false;

    public ControllerButtonsMapper controllerButtonsMapper;
    private void Start()
    {
        anchorCore.OnAnchorsEraseAllCompleted.AddListener(ClearGUID);
        anchorCore.OnAnchorCreateCompleted.AddListener(FindGUIDs);
    }

    public void LoadEnv()
    {
        if (isEnvLoaded)
            return;

        LoadObject(anchorObj, EnvObj, EnvPosGuid);
        LoadObject(anchorObj, EnvObjPrefab, EnvRotGuid);
        StartCoroutine(Rotate());
        isEnvLoaded = true;
    }
   
    private void LoadObject(GameObject anchor ,GameObject GameObj , string anchorGuid)
    {
        string guidString = PlayerPrefs.GetString(anchorGuid);
        List<Guid> guidEnv = new List<Guid>();

        if (Guid.TryParse(guidString, out Guid guid))
        {
            guidEnv.Add(guid);
            Debug.Log("Converted to GUID: " + guid);
            anchorCore.LoadAndInstantiateAnchors(anchor, guidEnv);
            StartCoroutine(loadAncObj(guidEnv[0], GameObj));
            GUIDs.Add(guid);
        }
        else
        {
            Debug.LogWarning("Invalid GUID string!");
        }
    }

    IEnumerator loadAncObj(Guid guidEnv, GameObject obj)
    {
        yield return new WaitForSeconds(3f);
        GameObject an = FindAnchorByGuid(guidEnv);
        GameObject env = Instantiate(obj, an.transform.position, Quaternion.identity);
        Anchors.Add(an.GetComponent<OVRSpatialAnchor>());
        env.transform.parent = an.transform;
    }


    public GameObject FindAnchorByGuid(Guid uuid)
    {
       OVRSpatialAnchor[] anchors = FindObjectsOfType<OVRSpatialAnchor>();

        foreach (OVRSpatialAnchor a in anchors)
        {
            Guid id = a.Uuid;  // or AnchorId / Id depending on SDK version
            if (id == uuid)
                return a.gameObject;
        }

        return null;
    }

    public void FindGUIDs(OVRSpatialAnchor loadedAnchor , OperationResult result)
    {
        if (result == OperationResult.Success)
        {
            Guid uuid = loadedAnchor.Uuid;
            Anchors.Add(loadedAnchor);
            GUIDs.Add(uuid);
            notificationData.ShowPopUp(0);

        }
        else
        {
            notificationData.ShowPopUp(5);

        }

    }

    public void ClearGUID(OperationResult result)
    {
        if (result == OperationResult.Success)
        {
            GUIDs.Clear();
            Anchors.Clear();
            PlayerPrefs.DeleteKey(EnvPosGuid);
            PlayerPrefs.DeleteKey(EnvRotGuid);
            notificationData.ShowPopUp(1);
            isEnvLoaded = false;

        }
        else
        {
            notificationData.ShowPopUp(6);

        }

    }
    public void ClearGUID()
    {
        GUIDs.Clear();
        Anchors.Clear();
        PlayerPrefs.DeleteKey(EnvPosGuid);
        PlayerPrefs.DeleteKey(EnvRotGuid);
        notificationData.ShowPopUp(1);
        isEnvLoaded = false;
    }

    public void SaveGUIDs()
    {
        PlayerPrefs.SetString(EnvPosGuid, GUIDs[0].ToString());
        PlayerPrefs.SetString(EnvRotGuid, GUIDs[1].ToString());
        PlayerPrefs.Save();
        notificationData.ShowPopUp(2);

    }

    IEnumerator Rotate()
    {
        yield return new WaitForSeconds(6f);

        EnvAutoAlign alin = FindAnyObjectByType<EnvAutoAlign>();

        if (alin != null)
        alin.Rotate();
        if (Anchors.Count > 0)
        {
            notificationData.ShowPopUp(3);
            controllerButtonsMapper.enabled = false;

        }
        else
        {
            notificationData.ShowPopUp(4);
        }
    }

}