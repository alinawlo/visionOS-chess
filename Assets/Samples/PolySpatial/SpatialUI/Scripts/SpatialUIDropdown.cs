using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PolySpatial.Samples
{
    public class SpatialUIDropdown : SpatialUI
    {
        [SerializeField]
        protected GameObject m_ExpandedContent;

        [SerializeField]
        protected TMP_Text m_CurrentSelectionText;

        [SerializeField]
        protected List<SpatialUIButton> m_ContentButtons;


        bool m_ShowingExpandedContent;

        void OnEnable()
        {
            foreach (var button in m_ContentButtons)
            {
                button.WasPressed += WasPressed;
            }
        }

        void OnDisable()
        {
            foreach (var button in m_ContentButtons)
            {
                button.WasPressed -= WasPressed;
            }
        }

        void WasPressed(string text, MeshRenderer meshRenderer)
        {
            m_CurrentSelectionText.text = text;
            m_ShowingExpandedContent = false;
            meshRenderer.material.color = SelectedColor;
            m_ExpandedContent.SetActive(false);
        }

        public override void Press(Vector3 position)
        {
            base.Press(position);
            m_ShowingExpandedContent = !m_ShowingExpandedContent;

            m_ExpandedContent.SetActive(m_ShowingExpandedContent);

            foreach (var button in m_ContentButtons)
            {
                if (m_CurrentSelectionText.text != button.ButtonText)
                {
                    button.MeshRenderer.material.color = UnselectedColor;
                }
            }
        }
    }
}
