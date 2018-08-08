using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CCameraRotation : MonoBehaviour {

	[Header("Config")]
	[SerializeField]	protected bool m_UseMouse = false;
	[SerializeField]	protected float m_HoldingTime = 0.5f;
	[SerializeField]	protected float m_RotationSpeed = 6f;
	[SerializeField]	protected float m_DeltaPosition = 0.5f;
	[SerializeField]	protected float m_DeltaRotation = 0.5f;
	[SerializeField]	protected Camera m_Camera;
	[SerializeField]	protected Transform m_Target;
	[Header("Events")]
	public UnityEvent OnStartRotation;
	public UnityEvent OnEndRotation;

	protected float m_HoldingCounter = 0f;
	protected Vector3 m_LastMousePosition;
	protected Vector3 m_RotationMouse;

	protected virtual void Start() {
		this.m_Camera.transform.LookAt(this.m_Target);
	}

	protected virtual void Update() {
		if (this.m_UseMouse) {
			// CAMERA
			if (Input.GetMouseButtonDown(0)) {
				this.m_LastMousePosition = Input.mousePosition;
			}
			if (Input.GetMouseButton(0)) {
				this.RotationObject();
				this.m_HoldingCounter += Time.deltaTime;
				if (this.m_HoldingCounter >= this.m_HoldingTime) {
					if (this.OnStartRotation != null) {
						this.OnStartRotation.Invoke();
					}
				}
			}
			if (Input.GetMouseButtonUp(0)) {
				if (this.OnEndRotation != null) {
					this.OnEndRotation.Invoke();
				}
				this.m_HoldingCounter = 0f;
			}
		}
	}

	public virtual void RotationObject() {
		var delta = (Input.mousePosition - this.m_LastMousePosition).normalized;
		this.RotationObjectWith (delta.x);
	}

	public virtual void RotationObjectWith(float value) {
		var newRotation = Quaternion.Euler(0f, value * this.m_RotationSpeed, 0f);
		var nexPosition = newRotation * this.m_Camera.transform.position;
		this.m_Camera.transform.position = nexPosition;
		this.m_Camera.transform.LookAt(this.m_Target);
		this.m_LastMousePosition = Input.mousePosition;
	}

	protected float m_LastDeltaValue = 0f;
	public virtual void RotationObjectDelta(float value) {
		var delta = value - this.m_LastDeltaValue;
		var newValue = delta >= 0 ? value : -value;
		this.RotationObjectWith (delta);
		this.m_LastDeltaValue = value;
	}

}
