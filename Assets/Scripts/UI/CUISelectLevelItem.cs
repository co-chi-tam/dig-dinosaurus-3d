using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CUISelectLevelItem : MonoBehaviour {

	[SerializeField]	protected int m_ItemValue;
	public int itemValue { 
		get { return this.m_ItemValue; } 
		set { this.m_ItemValue = value; } 
	}
	[SerializeField]	protected Vector2 m_Size = new Vector2 (800f, 250f);
	[SerializeField]	protected Image m_AvatarImage;
	[SerializeField]	protected TextMeshProUGUI m_TextMesh;
	[SerializeField]	protected Button m_Button;
	[SerializeField]	protected RectTransform m_ItemContent;

	protected virtual void Awake() {
		
	}

	public virtual void Setup(int value, Sprite avatar, string name, UnityAction callback) {
		this.m_ItemValue = value;
		this.m_AvatarImage.sprite = avatar;
		this.m_TextMesh.text = name;
		var cellWidth = this.m_ItemContent.sizeDelta.x / 2f;
		var newRandomX = Random.Range(cellWidth, this.m_Size.x - cellWidth);
		var newPosition = new Vector2(newRandomX, this.m_ItemContent.position.y);
		this.m_ItemContent.anchoredPosition = newPosition;
		if (callback != null) {
			this.m_Button.onClick.RemoveAllListeners();
			this.m_Button.onClick.AddListener(callback);
		}
	}
	
}
