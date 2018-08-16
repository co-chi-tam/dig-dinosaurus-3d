using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CSelectLevelScene : MonoBehaviour {

	[Header("Level")]
	[SerializeField]	protected Transform m_LevelRoot;
	[SerializeField]	protected CUISelectLevelItem m_ItemPrefab;
	[SerializeField]	protected GridLayoutGroup m_GridLayout;
	[Header("Dinosaurs")]
	[SerializeField]	protected string m_DinosaurusFolder = "DinosaurusImages/";
	[SerializeField]	protected Sprite[] m_DinosaurusSprites;
	[Header("Scene")]
	[SerializeField]	protected CSwitchScene m_SwitchScene;

	protected virtual void Start() {
		this.LoadLevel();
	} 

	public virtual void LoadLevel() {
		this.m_DinosaurusSprites = Resources.LoadAll<Sprite>(this.m_DinosaurusFolder);
		for (int i = 0; i < this.m_DinosaurusSprites.Length; i++)
		{
			var sprite = this.m_DinosaurusSprites[i];
			var name = sprite.name;
			var cellItem = Instantiate(this.m_ItemPrefab);
			cellItem.transform.SetParent (this.m_LevelRoot);
			cellItem.transform.localPosition = Vector3.zero;
			cellItem.transform.localScale = Vector3.one;
			cellItem.gameObject.SetActive(true);
			cellItem.name = string.Format("Level {0}", name);
			cellItem.Setup (i, sprite, name, () => {
				this.SelectedLevel (cellItem.itemValue);
			});
		}
		this.m_ItemPrefab.gameObject.SetActive(false);
	}
	
	public virtual void SelectedLevel(int value) {
		CGrid3D.DINOSAURUS_INDEX = value;
		this.m_SwitchScene.LoadScene("Game3DScene");
	}

}
