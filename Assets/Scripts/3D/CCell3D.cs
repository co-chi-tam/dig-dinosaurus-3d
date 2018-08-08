using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CCell3D : MonoBehaviour {

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
    [SerializeField]	protected int m_Z;
	public int z { 
		get { return this.m_Z; }
		set { this.m_Z = value; }
	}
    [SerializeField]	protected int m_Value;
	public int value { 
		get { return this.m_Value; }
		set { this.m_Value = value; }
	}
    [SerializeField]    protected Collider m_Collider;
    public Collider objCollider { 
		get { return this.m_Collider; }
		set { this.m_Collider = value; }
	}
	[SerializeField]    protected MeshRenderer m_MeshRenderer;
    public MeshRenderer objMeshRenderer { 
		get { return this.m_MeshRenderer; }
		set { this.m_MeshRenderer = value; }
	}

	protected Material m_Material;

	protected virtual void Awake() {
		this.m_Material = new Material(this.m_MeshRenderer.material.shader);
		this.m_MeshRenderer.material = this.m_Material;
	}

    public virtual void SetupItem(int x, int y, int z, int value, Texture2D texture,UnityAction callback) {
		this.m_X = x;
		this.m_Y = y;
		this.m_Z = z;
        this.m_Value = value;
		this.m_MeshRenderer.material.mainTexture = texture;
		this.m_Collider.gameObject.SetActive(value != 0);
	}

	public virtual void SetActive(bool value) {
		this.gameObject.SetActive (value);
		this.m_Value = value ? this.m_Value : 0;
	}

	public virtual Vector3 GetPosition() {
		return this.gameObject.transform.localPosition;
	}

	public override string ToString() 
	{
		return string.Format("Cell {0}-{1}-{2}", this.m_X, this.m_Y, this.m_Z);
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
			hash = hash * 23 + this.m_Z.GetHashCode();
			hash = hash * 23 + this.m_Value.GetHashCode();
			return hash;
		}
		// return base.GetHashCode();
	}
	
}
