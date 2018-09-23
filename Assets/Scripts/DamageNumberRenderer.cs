using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNumberRenderer : MonoBehaviour
{
	public RectTransform ImageParent { get { return m_ImageParent; } }
	public Camera Camera { get { return m_Camera; } }

	[SerializeField] RectTransform m_ImageParent = null;
	[SerializeField] Camera m_Camera = null;
}
