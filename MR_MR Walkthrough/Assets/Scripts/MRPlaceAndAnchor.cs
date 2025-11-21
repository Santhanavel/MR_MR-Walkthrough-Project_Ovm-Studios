using System;
using System.Collections.Generic;
using UnityEngine;

public class MRPlaceAndAnchor : MonoBehaviour
{
    public OVRSpatialAnchor Anchor;

    private void Awake()
    {
        if(Anchor == null)
        {
            Anchor = FindObjectOfType<OVRSpatialAnchor>();

            if(Anchor == null)
            {
                Anchor = new OVRSpatialAnchor();
            }
        }
    }
}
