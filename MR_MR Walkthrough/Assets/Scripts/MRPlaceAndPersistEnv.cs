using Meta.XR.BuildingBlocks;
using System;
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


    public const string EnvPosGuid = "EnvPosGuid";
    public const string EnvRotGuid = "EnvRotGuid";

    public bool isEnvLoaded = false;
    private void Start()
    {
        anchorCore.OnAnchorsEraseAllCompleted.AddListener(ClearGUID);
        anchorCore.OnAnchorCreateCompleted.AddListener(FindGUIDs);
    }



    public void LoadEnv()
    {
        if (isEnvLoaded)
            return;


        LoadObject(anchorObj, EnvPosGuid);
        GameObject an = FindAnchorByGuid(GUIDs[0]);
        Instantiate(EnvObj, an.transform.position, Quaternion.identity);

        LoadObject(anchorObj, EnvRotGuid);
        GameObject a = FindAnchorByGuid(GUIDs[1]);
        Instantiate(EnvObjPrefab, a.transform.position, Quaternion.identity);
        isEnvLoaded = true;
    }
    public void LoadEnvRot()
    {
       
    }


    private void LoadObject(GameObject prefab , string anchorGuid)
    {
        string guidString = PlayerPrefs.GetString(anchorGuid);
        List<Guid> guidEnv = new List<Guid>();

        if (Guid.TryParse(guidString, out Guid guid))
        {
            guidEnv.Add(guid);
            Debug.Log("Converted to GUID: " + guid);
            anchorCore.LoadAndInstantiateAnchors(prefab, guidEnv);

        }
        else
        {
            Debug.LogWarning("Invalid GUID string!");
        }
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
            GUIDs.Add(uuid);
        }

    }

    public void ClearGUID(OperationResult result)
    {
        if (result == OperationResult.Success)
        {
            GUIDs.Clear();
            PlayerPrefs.DeleteKey(EnvPosGuid);
            PlayerPrefs.DeleteKey(EnvRotGuid);
        }
       
    }

    public void SaveGUIDs()
    {
        if (GUIDs.Count <=2)
        {
            PlayerPrefs.SetString(EnvPosGuid, GUIDs[0].ToString());
            PlayerPrefs.SetString(EnvRotGuid, GUIDs[1].ToString());
            PlayerPrefs.Save();
        }
    }
}