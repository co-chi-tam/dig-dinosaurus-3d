using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CCell2D : MonoBehaviour {

	[SerializeField]	protected int m_X;
	public int x { 
		get { return this.m_X; }
		set { this.m_X = value; }
	}
	[SerializeField]	protected int m_Y;
	public int y { 
		get { return this.m_Y; }
		set { this.m_Y = value; }
	}
    [SerializeField]	protected int m_Value;
	public int value { 
		get { return this.m_Value; }
		set { this.m_Value = value; }
	}
	[SerializeField]	protected Text m_Text;
	[SerializeField]	protected Button m_Button;

	public virtual void SetupItem(int x, int y, int value, UnityAction callback) {
		this.m_X = x;
		this.m_Y = y;
        this.m_Value = value;
		this.m_Text.text = value == 0 ? "" : string.Format("{0}", value);
		if (callback != null) {
			this.m_Button.onClick.RemoveAllListeners();
			this.m_Button.onClick.AddListener(callback);
		}
	}

	public virtual void SetActive(bool value) {
		this.gameObject.SetActive (value);
	}

	// override object.Equals
	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}
		return base.Equals (obj);
	}
	
	// override object.GetHashCode
	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 23 + this.m_X.GetHashCode();
			hash = hash * 23 + this.m_Y.GetHashCode();
			hash = hash * 23 + this.m_Value.GetHashCode();
			return hash;
		}
		// return base.GetHashCode();
	}

}
