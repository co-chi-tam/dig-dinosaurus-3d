using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CGrid3D : MonoBehaviour {

	#region Fields

	[Header("Config")]
	[SerializeField]	protected bool m_ActiveGrid = true;
	[Header("Dinosaurus")]
	[SerializeField]	protected Transform m_DinosaurusRoot;
	[SerializeField]	protected string m_DinosaurusFolder = "Dinosaurus/";
	[SerializeField]	protected GameObject[] m_DinosaurusPrefabs;
	[Header("Grid")]
	[SerializeField]	protected Vector3 m_CellSize = new Vector3(1f, 1f, 1f);
	// [SerializeField]	protected TextAsset m_GridText;
	// [SerializeField]	protected int m_Row = 5;
	// [SerializeField]	protected int m_Column = 5;
	// [SerializeField]	protected int m_Deep = 5;
	[Header("Cell")]
	[SerializeField]	protected string m_CellFolder = "Rock/";
	[SerializeField]	protected Texture2D[] m_CellTextures;
	[SerializeField]	protected Transform m_GridRoot;
	[SerializeField]	protected LayerMask m_CellLayerMask;
	[SerializeField]	protected CCell3D m_CellPrefab;
	[Header("Line")]
	[SerializeField]	protected float m_LineTimer = 3f;
	protected float m_LineCounter = 0f;
	[SerializeField]	protected Transform m_LineRoot;
	[SerializeField]	protected LineRenderer m_LinePrefab;
	[Header("Events")]
	public UnityEvent OnStartGame;
	public UnityEvent OnEndGame;
	protected CCell3D m_Item1;
	protected CCell3D m_Item2;
	protected List<LineRenderer> m_LinePool = new List<LineRenderer>();
	protected int[,,] m_Grid;
	protected CCell3D[,,] m_GridCells;

	#endregion

	#region Monobehaviour Implementation

	protected virtual void Awake () {
		this.InitResources();
	}

	protected virtual void Start() {
		this.InitGame();
	}

	protected virtual void Update() {
		if (this.m_ActiveGrid) {
			// LINE
			if (this.m_LineCounter > 0f) {
				this.m_LineCounter -= Time.deltaTime;
			} else {
				this.ResetLines();
			}
			// COLLIDER DETECT
			if (Input.GetMouseButtonUp(0)) {
				this.DetectCell();
			}
		}
	}

	#endregion

	#region Generate Grid

	public virtual void InitResources() {
		this.m_DinosaurusPrefabs = Resources.LoadAll<GameObject>(this.m_DinosaurusFolder);
		this.m_CellTextures = Resources.LoadAll<Texture2D>(this.m_CellFolder);
	}

	public virtual void InitGame() {
		this.GenerateRandomGrid();
		this.GenerateCells();
		this.GenerateDinosaurus();
	}

	public virtual void GenerateRandomGrid() {	
		this.m_Grid = new int[,,] {
			{ {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0} },
			{ {0, 0, 0, 0, 0, 0, 0, 0}, {0, 1, 1, 1, 1, 1, 1, 0}, {0, 1, 1, 1, 1, 1, 1, 0}, {0, 2, 2, 2, 2, 2, 2, 0}, {0, 2, 2, 2, 2, 2, 2, 0}, {0, 3, 3, 3, 3, 3, 3, 0}, {0, 9, 9, 9, 9, 9, 9, 0}, {0, 0, 0, 0, 0, 0, 0, 0} },
			{ {0, 0, 0, 0, 0, 0, 0, 0}, {0, 3, 3, 3, 3, 3, 3, 0}, {0, 4, 4, 4, 4, 4, 4, 0}, {0, 4, 4, 4, 4, 4, 4, 0}, {0, 5, 5, 5, 5, 5, 5, 0}, {0, 5, 5, 5, 5, 5, 5, 0}, {0, 9, 9, 9, 9, 9, 9, 0}, {0, 0, 0, 0, 0, 0, 0, 0} },
			{ {0, 0, 0, 0, 0, 0, 0, 0}, {0, 6, 6, 6, 6, 6, 6, 0}, {0, 6, 6, 6, 6, 6, 6, 0}, {0, 7, 7, 7, 7, 7, 7, 0}, {0, 7, 7, 7, 7, 7, 7, 0}, {0, 8, 8, 8, 8, 8, 8, 0}, {0, 8, 8, 8, 8, 8, 8, 0}, {0, 0, 0, 0, 0, 0, 0, 0} },
			{ {0, 0, 0, 0, 0, 0, 0, 0}, {0, 1, 1, 1, 1, 1, 1, 0}, {0, 2, 2, 2, 2, 2, 2, 0}, {0, 3, 3, 3, 3, 3, 3, 0}, {0, 4, 4, 4, 4, 4, 4, 0}, {0, 5, 5, 5, 5, 5, 5, 0}, {0, 6, 6, 6, 6, 6, 6, 0}, {0, 0, 0, 0, 0, 0, 0, 0} },
			{ {0, 0, 0, 0, 0, 0, 0, 0}, {0, 7, 7, 7, 7, 7, 7, 0}, {0, 8, 8, 8, 8, 8, 8, 0}, {0, 9, 9, 9, 9, 9, 9, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0} },
			{ {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0} },
			{ {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0}, {0, 0, 0, 0, 0, 0, 0, 0} }
		};
		// this.m_Grid = new int[this.m_Row, this.m_Column, this.m_Deep];
		this.m_GridCells = new CCell3D[this.m_Grid.GetLength(0), this.m_Grid.GetLength(1), this.m_Grid.GetLength(2)];
		var i = 0;
		for (int z = 1; z < this.m_Grid.GetLength(2) - 1; z++) 
		{
			for (int y = 1; y < this.m_Grid.GetLength(1) - 1; y++)
			{
				for (int x = 1; x < this.m_Grid.GetLength(0) - 1; x++)
				{
					var rdRow = UnityEngine.Random.Range(1, this.m_Grid.GetLength(0) - 1);	
					var rdCol = UnityEngine.Random.Range(1, this.m_Grid.GetLength(1) - 1);	
					var rdDep = UnityEngine.Random.Range(1, this.m_Grid.GetLength(2) - 1);	
					i = this.m_Grid[x, y, z];
					this.m_Grid[x, y, z] = this.m_Grid[rdRow, rdCol, rdDep];
					this.m_Grid[rdRow, rdCol, rdDep] = i;
				}
			}
		}
		this.m_GridRoot.localScale = this.m_CellSize;
	}

	public virtual void GenerateCells() {
		var centerX = (this.m_Grid.GetLength(0) - this.m_CellSize.x) / 2f;
		var centerY = (this.m_Grid.GetLength(1) - this.m_CellSize.y) / 2f;
		var centerZ = (this.m_Grid.GetLength(2) - this.m_CellSize.z) / 2f;
		for (int z = 0; z < this.m_Grid.GetLength(2); z++) 
		{
			for (int y = 0; y < this.m_Grid.GetLength(1); y++)
			{
				for (int x = 0; x < this.m_Grid.GetLength(0); x++)
				{
					var cellItem = Instantiate(this.m_CellPrefab);
					var cellValue = this.m_Grid[x, y, z];
					cellItem.transform.SetParent (this.m_GridRoot);
					cellItem.transform.localPosition = new Vector3(x - centerX, y - centerY, z - centerZ);
					cellItem.transform.localScale = Vector3.one;
					cellItem.gameObject.SetActive(true);
					cellItem.name = string.Format("Cell {0}-{1}-{2}", x, y, z);
					cellItem.SetupItem(x, y, z, 
						cellValue, 
						this.m_CellTextures[cellValue], () => {
						
					});
					this.m_GridCells[x, y, z] = cellItem;
				}
			}
		}
		this.m_CellPrefab.gameObject.SetActive(false);
	}

	public virtual void GenerateDinosaurus() {
		var randomIndex = UnityEngine.Random.Range(0, this.m_DinosaurusPrefabs.Length);
		var dinosaurus = Instantiate(this.m_DinosaurusPrefabs[randomIndex]);
		dinosaurus.transform.SetParent(this.m_DinosaurusRoot);
		dinosaurus.transform.localPosition = Vector3.zero;
	}

	#endregion

	#region Main methods

	public virtual void OnCompleteGrid() {
		if (this.OnEndGame != null) {
			this.OnEndGame.Invoke();
		}
	}

	public virtual bool IsCheckCompleteGrid() {
		for (int z = 0; z < this.m_Grid.GetLength(2); z++)
		{
			for (int y = 0; y < this.m_Grid.GetLength(1); y++)
			{
				for (int x = 0; x < this.m_Grid.GetLength(0); x++)
				{
					if (this.m_Grid[x, y, z] > 0) {
						return false;
					}
				}
			}
		}
		return true;
	}

	protected virtual void DetectCell() {
		var mousePos = Input.mousePosition;
		var ray = Camera.main.ScreenPointToRay(mousePos);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 1000f, this.m_CellLayerMask)) {
			var cell = hitInfo.collider.GetComponent<CCell3D>();
			if (cell != null) {
				this.SubmitCell(cell);
			}
		}
	}

	public void SubmitCell(CCell3D value) {
		this.ResetLines();
		if (this.m_Item1 == null) {
			this.m_Item1 = value;
		} else if (this.m_Item2 == null) {
			if (this.m_Item1 != value)
			{
				this.m_Item2 = value;
			}
			else 
			{
				this.m_Item1 = null;
				this.m_Item2 = null;
			}
		}
		if (this.m_Item1 != null && this.m_Item2 != null) {
			if (this.CheckRect(
				this.m_Item1.x, this.m_Item1.y, this.m_Item1.z,
				this.m_Item2.x, this.m_Item2.y, this.m_Item2.z
			)) {
				this.m_Grid[this.m_Item1.x, this.m_Item1.y, this.m_Item1.z] = 0; 
				this.m_Grid[this.m_Item2.x, this.m_Item2.y, this.m_Item2.z] = 0;
				this.m_GridCells[this.m_Item1.x, this.m_Item1.y, this.m_Item1.z].SetActive(false);
				this.m_GridCells[this.m_Item2.x, this.m_Item2.y, this.m_Item2.z].SetActive(false);
				this.m_LineCounter = this.m_LineTimer;
			}
			this.m_Item1 = null;
			this.m_Item2 = null;
			if (this.IsCheckCompleteGrid()) {
				this.OnCompleteGrid();
			}
		}
	}

	public bool CheckXWith(int x1, int x2, int y, int z) {
		var minX = Mathf.Min(x1, x2);
		var maxX = Mathf.Max(x1, x2);
		return this.CheckX(minX, maxX, y, z);
	}

	public bool CheckYWith(int y1, int y2, int x, int z) {
		var minY = Mathf.Min(y1, y2);
		var maxY = Mathf.Max(y1, y2);
		return this.CheckY(minY, maxY, x, z);
	}

	public bool CheckZWith(int z1, int z2, int x, int y) {
		var minZ = Mathf.Min(z1, z2);
		var maxZ = Mathf.Max(z1, z2);
		return this.CheckY(minZ, maxZ, x, y);
	}
	
	public bool CheckX(int x1, int x2, int y, int z) {
		if (x1 < 0 
			|| x1 >= this.m_Grid.GetLength(0)
			|| x2 < 0 
			|| x2 >= this.m_Grid.GetLength(0)
			|| y < 0
			|| y >= this.m_Grid.GetLength(1)
			|| z < 0
			|| z >= this.m_Grid.GetLength(2))
			return false;
		for (int x = x1; x <= x2; x++)
		{
			var value = this.m_Grid[x, y, z];
			if (value != 0)
				return false;
		}
		return true;
	}

	public bool CheckY(int y1, int y2, int x, int z) {
		if (y1 < 0 
			|| y1 >= this.m_Grid.GetLength(1)
			|| y2 < 0 
			|| y2 >= this.m_Grid.GetLength(1)
			|| x < 0
			|| x >= this.m_Grid.GetLength(0)
			|| z < 0
			|| z >= this.m_Grid.GetLength(2))
			return false;
		for (int y = y1; y <= y2; y++)
		{
			var value = this.m_Grid[x, y, z];
			if (value != 0)
				return false;
		}
		return true;
	}

	public bool CheckZ(int z1, int z2, int x, int y) {
		if (z1 < 0 
			|| z1 >= this.m_Grid.GetLength(2)
			|| z2 < 0 
			|| z2 >= this.m_Grid.GetLength(2)
			|| x < 0
			|| x >= this.m_Grid.GetLength(0)
			|| y < 0
			|| y >= this.m_Grid.GetLength(1))
			return false;
		for (int z = z1; z <= z2; z++)
		{
			var value = this.m_Grid[x, y, z];
			if (value != 0)
				return false;
		}
		return true;
	}

	public bool CheckCell(CCell3D cell1, CCell3D cell2, bool isCheckRoot = false) {
		if (cell1.y == cell2.y && cell1.z == cell2.z) {
			return this.CheckXWith(cell1.x + 1, cell2.x - 1, cell1.y, cell1.z);
		}
		if (cell1.x == cell2.x && cell1.z == cell2.z) {
			return this.CheckYWith(cell1.y + 1, cell2.y - 1, cell1.x, cell1.z);
		}
		if (cell1.x == cell2.x && cell1.y == cell2.y) {
			return this.CheckZWith(cell1.z + 1, cell2.z - 1, cell1.x, cell1.y);
		}
		return true;
	}

	public bool CheckPhysicCell(CCell3D cell1, CCell3D cell2, bool isCheckRoot = false) {
		var origin = cell1.transform.position;
		var direction = cell2.transform.position - origin;
		RaycastHit hitInfo;
		var result = Physics.Raycast(
			origin, direction, 
			out hitInfo, 
			direction.magnitude,
			this.m_CellLayerMask
		);
		if (isCheckRoot) {
			var cellCollider = hitInfo.collider.GetComponent<CCell3D>();
			return result && cellCollider.name == cell2.name;
		} else {
			return !result;
		}	
	}

	#endregion

	#region Cases

	public bool CheckRect(int x1, int y1, int z1, int x2, int y2, int z2) {
		// 0. Available
		if (x1 == x2 && y1 == y2 && z1 == z2) // DUPLICATE
			return false;
		var value1 = this.m_Grid[x1, y1, z1];
		var value2 = this.m_Grid[x2, y2, z2];
		if (value1 != value2)		// NOT SAME VALUE
			return false;
		if (value1 == 0 && value2 == 0) // NOT AVAILABLE
			return false;
		// INIT
		var minX = Mathf.Min(x1, x2);
		var maxX = Mathf.Max(x1, x2);
		var minY = Mathf.Min(y1, y2);
		var maxY = Mathf.Max(y1, y2);
		var minZ = Mathf.Min(z1, z2);
		var maxZ = Mathf.Max(z1, z2);
		// CASE 1
		if (this.Case1( x1, y1, z1, x2, y2, z2,
						minX, minY, minZ, maxX, maxY, maxZ)) {
			return true;
		}
		// CASE 2
		if (this.Case2( x1, y1, z1, x2, y2, z2,
						minX, minY, minZ, maxX, maxY, maxZ)) {
			return true;
		}
		// CASE 3
		if (this.Case3( x1, y1, z1, x2, y2, z2,
						minX, minY, minZ, maxX, maxY, maxZ)) {
			return true;
		}
		// CASE 4
		if (this.Case4( x1, y1, z1, x2, y2, z2,
						minX, minY, minZ, maxX, maxY, maxZ)) {
			return true;
		}
		// CASE 5
		if (this.Case5( x1, y1, z1, x2, y2, z2,
						minX, minY, minZ, maxX, maxY, maxZ)) {
			return true;
		}
		return false;
	}

	protected virtual bool Case1(int x1, int y1, int z1, int x2, int y2, int z2,
								 int minX, int minY, int minZ, int maxX, int maxY, int maxZ) {
		// 1. CHECK X
		// x....x
		if (y1 == y2 && z1 == z2) {
			if (this.CheckX(minX + 1, maxX - 1, y1, z1)) {	
				// Debug.Log ("case 1.");
				this.DrawLineX(minX, maxX, y1, z1);
				return true;
			}
		}
		return false;
	}

	protected virtual bool Case2(int x1, int y1, int z1, int x2, int y2, int z2,
								 int minX, int minY, int minZ, int maxX, int maxY, int maxZ) {
		// 2. CHECK Y
		// x
		// .
		// .
		// x
		if (x1 == x2 && z1 == z2) {
			if (this.CheckY(minY + 1, maxY - 1, x1, z1)) {
				// Debug.Log ("case 2.");
				this.DrawLineY(minY, maxY, x1, z1);
				return true;
			}
		}
		return false;
	}

	protected virtual bool Case3(int x1, int y1, int z1, int x2, int y2, int z2,
								 int minX, int minY, int minZ, int maxX, int maxY, int maxZ) {
		// 3. CHECK Z
		// x
		//   .
		//    .
		//     x
		if (x1 == x2 && y1 == y2) {
			if (this.CheckZ(minZ + 1, maxZ - 1, x1, y1)) {
				// Debug.Log ("case 3.");
				this.DrawLineZ(minZ, maxZ, x1, y1);
				return true;
			}
		}
		return false;
	}

	protected virtual bool Case4(int x1, int y1, int z1, int x2, int y2, int z2,
								 int minX, int minY, int minZ, int maxX, int maxY, int maxZ) {
		// 4. CHECK MIN X, Y, Z TO MAX X, Y, Z
		// x
		// .
		// .
		// ......x (or) ....
		var cell1 = this.m_GridCells[x1, y1, z1];
		var cell2 = this.m_GridCells[x2, y2, z2];
		var lineCell1 = this.GetLineNeighbours(cell1);
		var lineCell2 = this.GetLineNeighbours(cell2);
		for (int i = 0; i < lineCell1.Length; i++)
		{
			for (int j = 0; j < lineCell2.Length; j++)
			{	
				// IS CROSSED
				if (   lineCell1[i].x == lineCell2[j].x
					&& lineCell1[i].y == lineCell2[j].y
					&& lineCell1[i].z == lineCell2[j].z) {
					// Debug.Log ("case 4A.");
					// IS CORRECT CELL
					if (this.CheckPhysicCell(lineCell1[i], cell1, true)
						&& this.CheckPhysicCell(lineCell1[i], cell2, true)) 
					{
						// Debug.Log ("case 4B. " + lineCell1[i].name);
						this.DrawLineCell(lineCell1[i], cell1);
						this.DrawLineCell(lineCell1[i], cell2);
						return true;
					} 
				}
			}
		}
		return false;
	}

	protected virtual bool Case5(int x1, int y1, int z1, int x2, int y2, int z2,
								 int minX, int minY, int minZ, int maxX, int maxY, int maxZ) {
		// 5. CHECK MIN X, Y, Z TO MAX X, Y, Z
		// x
		// .
		// .....
		//     .
	 	//     x (or) ....
		var cell1 = this.m_GridCells[x1, y1, z1];
		var cell2 = this.m_GridCells[x2, y2, z2];
		var lineCell1 = this.GetLineNeighbours(cell1);
		var lineCell2 = this.GetLineNeighbours(cell2);
		for (int i = 0; i < lineCell1.Length; i++)
		{
			var lineCell3 = this.GetLineNeighbours(lineCell1[i]);
			for (int k = 0; k < lineCell3.Length; k++)
			{
				for (int j = 0; j < lineCell2.Length; j++)
				{	
					// IS CROSSED
					if (   lineCell3[k].x == lineCell2[j].x
						&& lineCell3[k].y == lineCell2[j].y
						&& lineCell3[k].z == lineCell2[j].z) {
						// Debug.Log (string.Format("5A. 1. {0} - {1} = {2}\n2. {3} - {4} = {5}\n3. {6} - {7} = {8}", 
						// 		lineCell1[i].name, cell1.name, this.CheckPhysicCell(lineCell1[i], cell1, true), 
						// 		lineCell3[k].name, cell2.name, this.CheckPhysicCell(lineCell3[k], cell2, true), 
						// 		lineCell1[i].name, lineCell3[k].name, this.CheckPhysicCell(lineCell1[i], lineCell3[k])));
						// IS CORRECT CELL
						if (this.CheckPhysicCell(lineCell1[i], cell1, true)
							&& this.CheckPhysicCell(lineCell3[k], cell2, true)
							&& this.CheckPhysicCell(lineCell1[i], lineCell3[k])) 
						{
							// Debug.Log ("case 5B.");
							this.DrawLineCell(lineCell1[i], cell1);
							this.DrawLineCell(lineCell3[k], cell2);
							this.DrawLineCell(lineCell1[i], lineCell3[k]);
							return true;
						} 
					}
				}
			}
		}
		return false;
	}

	#endregion

	#region Line

	protected void ResetLines() {
		for (int i = 0; i < this.m_LinePool.Count; i++)
		{
			var linePool = this.m_LinePool[i];
			linePool.gameObject.SetActive(false);
		}
	}

	protected LineRenderer CreateLine () {
		for (int i = 0; i < this.m_LinePool.Count; i++)
		{
			var linePool = this.m_LinePool[i];
			if (linePool.gameObject.activeInHierarchy == false) {
				linePool.gameObject.SetActive(true);
				linePool.transform.localRotation = Quaternion.identity;
				return linePool;
			}
		}
		var line = Instantiate(this.m_LinePrefab);
		line.transform.SetParent(this.m_LineRoot);
		line.transform.localPosition = Vector3.zero;
		line.transform.localScale = Vector3.one;
		line.transform.localRotation = Quaternion.identity;
		line.gameObject.SetActive(true);
		this.m_LinePool.Add(line);
		this.m_LinePrefab.gameObject.SetActive (false);
		return line;
	}

	public virtual void DrawLineX(int x1, int x2, int y, int z) {
		var minX = Mathf.Min(x1, x2);
		var maxX = Mathf.Max(x1, x2);
		var line = this.CreateLine();
		var length = maxX - minX + 1;
		line.positionCount = length;
		var index = 0;
		for (int x = minX; x <= maxX; x++)
		{
			var cell = this.m_GridCells[x, y, z];
			var position = cell.transform.localPosition;
			line.SetPosition(index, position);
			index++;
		}
	}

	public virtual void DrawLineY(int y1, int y2, int x, int z) {
		var minY = Mathf.Min(y1, y2);
		var maxY = Mathf.Max(y1, y2);
		var line = this.CreateLine();
		var length = maxY - minY + 1;
		line.positionCount = length;
		var index = 0;
		for (int y = minY; y <= maxY; y++)
		{
			var cell = this.m_GridCells[x, y, z];
			var position = cell.transform.localPosition;
			line.SetPosition(index, position);
			index++;
		}
	}

	public virtual void DrawLineZ(int z1, int z2, int x, int y) {
		var minZ = Mathf.Min(z1, z2);
		var maxZ = Mathf.Max(z1, z2);
		var line = this.CreateLine();
		var length = maxZ - minZ + 1;
		line.positionCount = length;
		var index = 0;
		for (int z = minZ; z <= maxZ; z++)
		{
			var cell = this.m_GridCells[x, y, z];
			var position = cell.transform.localPosition;
			line.SetPosition(index, position);
			index++;
		}
	}

	public virtual void DrawLineCell(CCell3D cell1, CCell3D cell2) {
		var line = this.CreateLine();
		line.positionCount = 2;
		line.SetPosition(0, cell1.GetPosition());
		line.SetPosition(1, cell2.GetPosition());
	}

	#endregion

	#region Utilities

	public virtual CCell3D[] GetLineNeighbours(CCell3D cell) {
		var results = new List<CCell3D>();
		for (int x = 0; x < this.m_GridCells.GetLength(0); x++)
		{
			if (this.m_Grid[x, cell.y, cell.z] <= 0) 
				results.Add(this.m_GridCells[x, cell.y, cell.z]);
		}
		for (int y = 0; y < this.m_GridCells.GetLength(1); y++)
		{
			if (this.m_Grid[cell.x, y, cell.z] <= 0) 
				results.Add(this.m_GridCells[cell.x, y, cell.z]);
		}
		for (int z = 0; z < this.m_GridCells.GetLength(2); z++)
		{
			if (this.m_Grid[cell.x, cell.y, z] <= 0) 
				results.Add(this.m_GridCells[cell.x, cell.y, z]);
		}
		return results.ToArray();
	}

	public virtual int GetLineNeighboursNonAlloc(CCell3D cell, ref CCell3D[] results) {
		var x = 0;
		var y = 0;
		var z = 0;
		for (int i = 0; i < results.Length; i++)
		{
			results[i] = null;
		}
		for (x = 0; x < this.m_GridCells.GetLength(0); x++)
		{
			if (this.m_Grid[x, cell.y, cell.z] <= 0) 
				results[x] = this.m_GridCells[x, cell.y, cell.z];
		}
		for (y = 0; y < this.m_GridCells.GetLength(1); y++)
		{
			if (this.m_Grid[x, cell.y, cell.z] <= 0) 
				results[x + y] = this.m_GridCells[x, cell.y, cell.z];
		}
		for (z = 0; z < this.m_GridCells.GetLength(2); z++)
		{
			if (this.m_Grid[x, cell.y, cell.z] <= 0)
				results[x + y + z] = this.m_GridCells[x, cell.y, cell.z];
		}
		var resultCount = 0;
		for (int i = 0; i < results.Length; i++)
		{
			for (int j = i + 1; j < results.Length; j++)
			{
				var right = results[j];
				if (results[i] == null && right != null) {
					results[i] = right;
					results[j] = null;
					break;
				} 
			}
			resultCount = results[i] != null ? resultCount + 1 : resultCount;
		}
		return resultCount;
	}

	public virtual CCell3D[] GetNeighbours(CCell3D cell) {
		return this.GetNeighbours(cell.x, cell.y, cell.z);
	}

	public virtual int GetNeighboursNonAlloc(CCell3D cell, ref CCell3D[] results) {
		return this.GetNeighbours(cell.x, cell.y, cell.z, ref results);
	}

	public virtual CCell3D[] GetNeighbours(int x, int y, int z) {
		var results = new List<CCell3D>();
		// Right
		if (x + 1 < this.m_GridCells.GetLength(0)) {
			results.Add (this.m_GridCells[x + 1, y, z]);
		} 
		// Left
		if (x - 1 >= 0) {
			results.Add (this.m_GridCells[x - 1, y, z]);
		} 
		// Top
		if (y + 1 < this.m_GridCells.GetLength(1)) {
			results.Add (this.m_GridCells[x, y + 1, z]);
		} 
		// Down
		if (y - 1 >= 0) {
			results.Add (this.m_GridCells[x, y - 1, z]);
		} 
		// Front
		if (z - 1 >= 0) {
			results.Add (this.m_GridCells[x, y, z - 1]);
		}
		// Back
		if (z + 1 < this.m_GridCells.GetLength(2)) {
			results.Add (this.m_GridCells[x, y, z + 1]);
		} 
		return results.ToArray();
	}

	public virtual int GetNeighbours(int x, int y, int z, ref CCell3D[] results) {
		// Right
		if (x + 1 < this.m_GridCells.GetLength(0)) {
			results[0] = this.GetGridCell(x + 1, y, z);
		} 
		// Left
		if (x - 1 >= 0) {
			results[1] = this.GetGridCell(x - 1, y, z);
		} 
		// Top
		if (y + 1 < this.m_GridCells.GetLength(1)) {
			results[2] = this.GetGridCell(x, y + 1, z);
		} 
		// Down
		if (y - 1 >= 0) {
			results[3] = this.GetGridCell(x, y - 1, z);
		} 
		// Front
		if (z - 1 >= 0) {
			results[4] = this.GetGridCell(x, y, z - 1);
		}
		// Back
		if (z + 1 < this.m_GridCells.GetLength(2)) {
			results[5] = this.GetGridCell(x, y, z + 1);
		} 
		// Sort
		var resultCount = 0;
		for (int i = 0; i < results.Length; i++)
		{
			for (int j = i + 1; j < results.Length; j++)
			{
				var right = results[j];
				if (results[i] == null && right != null) {
					results[i] = right;
					results[j] = null;
					break;
				} 
			}
			resultCount = results[i] != null ? resultCount + 1 : resultCount;
		}
		return resultCount;
	}

	public virtual void SetActiveGrid(bool value) {
		this.m_ActiveGrid = value;
		if (value == false) {
			this.m_Item1 = null;
			this.m_Item2 = null;
		}
	}

	public virtual int GetGrid(int x, int y, int z) {
		if (   x < 0 
			|| x >= this.m_Grid.GetLength(0)
			|| y < 0
			|| y >= this.m_Grid.GetLength(1)
			|| z < 0
			|| z >= this.m_Grid.GetLength(2))
			return 0;
		return this.m_Grid[x, y, z];
	}

	public virtual CCell3D GetGridCell(int x, int y, int z) {
		if (   x < 0 
			|| x >= this.m_Grid.GetLength(0)
			|| y < 0
			|| y >= this.m_Grid.GetLength(1)
			|| z < 0
			|| z >= this.m_Grid.GetLength(2))
			return null;
		return this.m_GridCells[x, y, z];
	}

	#endregion
	
}
