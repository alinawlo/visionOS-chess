using System.Collections;
using System.Collections.Generic;
using PolySpatial.Samples;
using UnityEngine;

[RequireComponent(typeof(StringReference))]
public class CustomSpatialUIDropdown : SpatialUIDropdown
{
    StringReference selection;

    void Awake()
    {
        selection = GetComponent<StringReference>();

        m_ContentButtons.Clear();
        foreach (Transform child in m_ExpandedContent.transform){
            m_ContentButtons.Add(child.gameObject.GetComponent<CustomSpatialUIButton>());
        }
    }

    void Update(){
        selection.value = m_CurrentSelectionText.text;
    }
}
