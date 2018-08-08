using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPoint3D {

	public CPoint3D point;
	public CCell3D cell;

	public CPoint3D(CPoint3D point, CCell3D cell)
	{
		this.point = point;
		this.cell = cell;
	}

	public CPoint3D(CCell3D cell)
	{
		this.point = null;
		this.cell = cell;
	}

	public CPoint3D()
	{
		this.point = null;
		this.cell = null;
	}

	public virtual int CountCorner() {
		var next = this;
		var corner = 0;
		while(next.point != null) {
			next.cell.gameObject.SetActive(true);
			next = next.point;
		}
		return corner;
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
			hash = hash * 23 + this.cell.x.GetHashCode();
			hash = hash * 23 + this.cell.y.GetHashCode();
			hash = hash * 23 + this.cell.z.GetHashCode();
			return hash;
		}
		// return base.GetHashCode();
	}
	
}
