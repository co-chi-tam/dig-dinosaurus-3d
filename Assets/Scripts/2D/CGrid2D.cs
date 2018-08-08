using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CGrid2D : MonoBehaviour {

	#region Fields

	[Header("Grid")]
	[SerializeField]	protected TextAsset m_GridText;
	[SerializeField]	protected int m_Row = 5;
	[SerializeField]	protected int m_Column = 5;
	[Header("Cell")]
	[SerializeField]	protected Transform m_GridRoot;
	[SerializeField]	protected GridLayoutGroup m_GridLayout;
	[SerializeField]	protected CCell2D m_CellPrefab;
	[Header("Line")]
	[SerializeField]	protected float m_LineTimer = 1f;
	protected float m_LineCounter = 0f;
	[SerializeField]	protected Transform m_LineRoot;
	[SerializeField]	protected LineRenderer m_LinePrefab;
	[Header("Events")]
	public UnityEvent OnStartGame;
	public UnityEvent OnEndGame;

	protected List<LineRenderer> m_LinePool = new List<LineRenderer>();
	protected int[,] m_Grid;
	protected CCell2D[,] m_GridCells;
	protected CCell2D m_Item1;
	protected CCell2D m_Item2;

	#endregion

	#region Monobehaviour Implementation

	protected virtual void Awake () {
		
	}

	protected virtual void Start() {
		this.GenerateFromText(this.m_GridText);
	}

	protected virtual void Update() {
		if (this.m_LineCounter > 0f) {
			this.m_LineCounter -= Time.deltaTime;
		} else {
			this.ResetLines();
		}
	}

	#endregion

	#region Main methods

	public virtual void OnCompleteGrid() {
		if (this.OnEndGame != null) {
			this.OnEndGame.Invoke();
		}
	}

	public virtual bool IsCheckCompleteGrid() {
		for (int y = 0; y < this.m_Grid.GetLength(1); y++)
		{
			for (int x = 0; x < this.m_Grid.GetLength(0); x++)
			{
				if (this.m_Grid[x, y] > 0) {
					return false;
				}
			}
		}
		return true;
	}

	public virtual void GenerateCells() {
		for (int y = 0; y < this.m_Grid.GetLength(1); y++)
		{
			for (int x = 0; x < this.m_Grid.GetLength(0); x++)
			{
				var cellItem = Instantiate(this.m_CellPrefab);
				cellItem.transform.SetParent (this.m_GridRoot);
				cellItem.transform.localPosition = Vector3.zero;
				cellItem.transform.localScale = Vector3.one;
				cellItem.gameObject.SetActive(true);
				cellItem.name = string.Format("Cell {0}-{1}", x, y);
				cellItem.SetupItem(x, y, this.m_Grid[x, y], () => {
					this.SubmitItem(cellItem);
				});
				this.m_GridCells[x, y] = cellItem;
			}
		}
		this.m_CellPrefab.gameObject.SetActive(false);
	}

	public virtual void GenerateFromText(TextAsset value) {
		if (value != null) {
			this.GenerateFromStr(value.text);
		} else {
			this.GenerateRandomGrid();
		}
		this.GenerateCells();
		if (this.OnStartGame != null) {
			this.OnStartGame.Invoke();
		}
	}

	public virtual void GenerateFromStr(string value) {
		var lines = value.Split('\n');
		var columns = lines[0].Split(' '); 
		this.m_Row = lines.Length;
		this.m_Column = columns.Length;
		this.m_Grid = new int[this.m_Row, this.m_Column];
		this.m_GridCells = new CCell2D[this.m_Grid.GetLength(0), this.m_Grid.GetLength(1)];
		this.m_GridLayout.constraintCount = this.m_Column;
		var strGrid = "";
		for (int y = 0; y < lines.Length; y++)
		{
			var lineRows = lines[y].Split(' ');
			for (int x = 0; x < lineRows.Length; x++)
			{
				var intValue = int.Parse(lineRows[x].ToString());
				this.m_Grid[x, y] = intValue;
				strGrid += string.Format("{0}{1}", intValue, x < lineRows.Length - 1 ? " " : string.Empty);
			}
			strGrid += "\n";
		}
		Debug.Log (strGrid);
	}

	public virtual void GenerateRandomGrid() {
		this.m_Grid = new int[,] {
			{ 0, 0, 0, 0, 0, 0, 0 },
			{ 0, 1, 1, 2, 2, 3, 0 },
			{ 0, 3, 4, 4, 5, 5, 0 },
			{ 0, 6, 6, 7, 7, 8, 0 },
			{ 0, 8, 9, 9, 1, 1, 0 },
			{ 0, 2, 2, 3, 3, 0, 0 },
			{ 0, 0, 0, 0, 0, 0, 0 }
		};
		this.m_GridCells = new CCell2D[this.m_Grid.GetLength(0), this.m_Grid.GetLength(1)];
		this.m_GridLayout.constraintCount = this.m_Grid.GetLength(1);
		var i = 0;
		var strGrid = "";
		for (int y = 0; y < this.m_Grid.GetLength(1); y++)
		{
			for (int x = 0; x < this.m_Grid.GetLength(0); x++)
			{
				if (x == 0 || x == this.m_Grid.GetLength(0) - 1 
					|| y == 0 || y == this.m_Grid.GetLength(1) - 1) {
					this.m_Grid[x, y] = 0;	
				} else {
					var rdRow = Random.Range(1, this.m_Row - 1);	
					var rdCol = Random.Range(1, this.m_Column - 1);	
					i = this.m_Grid[x, y];
					this.m_Grid[x, y] = this.m_Grid[rdRow, rdCol];
					this.m_Grid[rdRow, rdCol] = i;
				}
				strGrid += string.Format("{0}{1}", this.m_Grid[x, y], x < this.m_Grid.GetLength(0) - 1 ? " " : string.Empty);
			}
			strGrid += "\n";
		}
		Debug.Log (strGrid);
	}

	public void SubmitItem(CCell2D value) {
		this.ResetLines();
		if (this.m_Item1 == null) {
			this.m_Item1 = value;
		} else if (this.m_Item2 == null) {
			this.m_Item2 = this.m_Item1 != value ? value : null;
		}
		if (this.m_Item1 != null && this.m_Item2 != null) {
			if (this.CheckRect(
				this.m_Item1.x, this.m_Item1.y, 
				this.m_Item2.x, this.m_Item2.y
			)) {
				this.m_Grid[this.m_Item1.x, this.m_Item1.y] = 0; 
				this.m_Grid[this.m_Item2.x, this.m_Item2.y] = 0;
				this.m_GridCells[this.m_Item1.x, this.m_Item1.y].SetActive(false);
				this.m_GridCells[this.m_Item2.x, this.m_Item2.y].SetActive(false);
				this.m_LineCounter = this.m_LineTimer;
			}
			this.m_Item1 = null;
			this.m_Item2 = null;
			if (this.IsCheckCompleteGrid()) {
				this.OnCompleteGrid();
			}
		}
		this.m_GridLayout.enabled = false;
	}

	public bool CheckRect(int x1, int y1, int x2, int y2) {
		// 0. Available
		if (x1 == x2 && y1 == y2) // DUPLICATE
			return false;
		var value1 = this.m_Grid[x1, y1];
		var value2 = this.m_Grid[x2, y2];
		if (value1 != value2)		// NOT SAME VALUE
			return false;
		if (value1 == 0 && value2 == 0) // NOT AVAILABLE
			return false;
		// INIT
		var minX = Mathf.Min(x1, x2);
		var maxX = Mathf.Max(x1, x2);
		var minY = Mathf.Min(y1, y2);
		var maxY = Mathf.Max(y1, y2);
		// CASE 1
		if (this.Case1(x1, y1, x2, y2, minX, minY, maxX, maxY)) {
			return true;
		}
		// CASE 2
		if (this.Case2(x1, y1, x2, y2, minX, minY, maxX, maxY)) {
			return true;
		}
		// CASE 3
		if (this.Case3(x1, y1, x2, y2, minX, minY, maxX, maxY)) {
			return true;
		}
		// CASE 4
		if (this.Case4(x1, y1, x2, y2, minX, minY, maxX, maxY)) {
			return true;
		}
		// CASE 5
		if (this.Case5(x1, y1, x2, y2, minX, minY, maxX, maxY)) {
			return true;
		}
		// CASE 6
		if (this.Case6(x1, y1, x2, y2, minX, minY, maxX, maxY)) {
			return true;
		}
		// CASE 7
		if (this.Case7(x1, y1, x2, y2, minX, minY, maxX, maxY)) {
			return true;
		}
		// CASE 8
		if (this.Case8(x1, y1, x2, y2, minX, minY, maxX, maxY)) {
			return true;
		}
		// CASE 9
		if (this.Case9(x1, y1, x2, y2, minX, minY, maxX, maxY)) {
			return true;
		}
		// CASE 10
		if (this.Case10(x1, y1, x2, y2, minX, minY, maxX, maxY)) {
			return true;
		}
		// Debug.Log (string.Format("{0}:{1}={2}:{3}", minX, maxX, minY, maxY));
		return false;
	}

	#endregion

	#region Cases

	protected virtual bool Case1(int x1, int y1, int x2, int y2, int minX, int minY, int maxX, int maxY) {
		// 1. CHECK X
		// x....x
		if (y1 == y2) {
			if (this.CheckX(minX + 1, maxX - 1, y1)) {	
				// Debug.Log ("case 1.");
				this.DrawLineX(minX, maxX, y1);
				return true;
			}
		}
		return false;
	}

	protected virtual bool Case2(int x1, int y1, int x2, int y2, int minX, int minY, int maxX, int maxY) {
		// 2. CHECK Y
		// x
		// .
		// .
		// x
		if (x1 == x2) {
			if (this.CheckY(minY + 1, maxY - 1, x1)) {
				// Debug.Log ("case 2.");
				this.DrawLineY(minY, maxY, x1);
				return true;
			}
		}
		return false;
	}

	protected virtual bool Case3(int x1, int y1, int x2, int y2, int minX, int minY, int maxX, int maxY) {
		// 3. Check X, Y 
		// x....
		//     .
		//     .
		//     x
		if (this.CheckX(minX + 1, maxX - 1, minY)) {
			if (this.CheckY(minY, maxY - 1, maxX)) {
				this.DrawLineX(minX, maxX, minY);
				this.DrawLineY(minY, maxY, maxX);
				// Debug.Log ("case 3.");
				return true;
			}
		}
		return false;
	}

	protected virtual bool Case4(int x1, int y1, int x2, int y2, int minX, int minY, int maxX, int maxY) {
		// 4. Check X, Y 
		// ....x
		// .
		// .
		// x
		if (this.CheckX(minX, maxX - 1, minY)) {
			if (this.CheckY(minY, maxY - 1, minX)) {
				this.DrawLineX(minX, maxX, minY);
				this.DrawLineY(minY, maxY, minX);
				// Debug.Log ("case 4.");
				return true;
			}
		}
		return false;
	}

	protected virtual bool Case5(int x1, int y1, int x2, int y2, int minX, int minY, int maxX, int maxY) {
		// 5. Check X, Y 
		// x
		// .
		// .
		// ....x
		if (this.CheckX(minX, maxX - 1, maxY)) {
			if (this.CheckY(minY + 1, maxY, minX)) {
				this.DrawLineX(minX, maxX, maxY);
				this.DrawLineY(minY, maxY, minX);
				// Debug.Log ("case 5.");
				return true;
			}
		}
		return false;
	}

	protected virtual bool Case6(int x1, int y1, int x2, int y2, int minX, int minY, int maxX, int maxY) {
		// 6. Check X, Y 
		//     x
		//     .
		//     .
		// x....
		if (this.CheckX(minX + 1, maxX, maxY)) {
			if (this.CheckY(minY + 1, maxY, maxX)) {
				this.DrawLineX(minX, maxX, maxY);
				this.DrawLineY(minY, maxY, maxX);
				// Debug.Log ("case 6.");
				return true;
			}
		}
		return false;
	}

	protected virtual bool Case7(int x1, int y1, int x2, int y2, int minX, int minY, int maxX, int maxY) {
		// 7. Check X, Y
		// x...    
		//    .    
		//    .	   
		//    ...x 
		for (int i = minX + 1; i < maxX; i++)
		{
			if (this.CheckY(minY, maxY, i)) {
				if (this.CheckXWith(x1 + 1, i, y1) 
					&& this.CheckXWith(i, x2 - 1, y2)) {
					this.DrawLineY(minY, maxY, i);
					this.DrawLineX(x1, i, y1);
					this.DrawLineX(i, x2, y2);	
					// Debug.Log ("case 7A.");
					return true;
				}
				if (this.CheckXWith(x1 - 1, i, y1) 
					&& this.CheckXWith(i, x2 + 1, y2)) {
					this.DrawLineY(minY, maxY, i);
					this.DrawLineX(x1, i, y1);
					this.DrawLineX(i, x2, y2);	
					// Debug.Log ("case 7B.");
					return true;
				}
			}
		}
		return false;
	}

	protected virtual bool Case8(int x1, int y1, int x2, int y2, int minX, int minY, int maxX, int maxY) {
		// 8. Check X, Y
		// x   
		// .      
		// .....	   
		//     .
		//     x
		for (int i = minY; i < maxY; i++)
		{
			if (this.CheckX(minX, maxX, i)) {
				if (this.CheckYWith(y1 + 1, i, x1) 
					&& this.CheckYWith(i, y2 - 1, x2)) {
					this.DrawLineX(minX, maxX, i);
					this.DrawLineY(y1, i, x1);
					this.DrawLineY(i, y2, x2);	
					// Debug.Log ("case 8A.");
					return true;
				}
				if (this.CheckYWith(y1 - 1, i, x1) 
					&& this.CheckYWith(y2 + 1, i, x2)) {
					this.DrawLineX(minX, maxX, i);
					this.DrawLineY(y1, i, x1);
					this.DrawLineY(i, y2, x2);	
					// Debug.Log ("case 8A.");
					return true;
				}
			} 
		}
		return false;
	}

	protected virtual bool Case9(int x1, int y1, int x2, int y2, int minX, int minY, int maxX, int maxY) {
		// 9. Check X, Y
		// .....
		// .   .
		// x   .
		//     x
		for (int col = 0; col < this.m_Grid.GetLength(1); col++)
		{
			if (this.CheckX(minX, maxX, col)) {
				if (this.CheckYWith(col, y1 - 1, x1) 
					&& this.CheckYWith(col, y2 - 1, x2)) {
					this.DrawLineX(minX, maxX, col);
					this.DrawLineY(col, y1, x1);
					this.DrawLineY(col, y2, x2);	
					// Debug.Log ("case 9A.");
					return true;
				}
				if (this.CheckYWith(col, y1 + 1, x1) 
					&& this.CheckYWith(col, y2 + 1, x2)) {
					this.DrawLineX(minX, maxX, col);
					this.DrawLineY(col, y1, x1);
					this.DrawLineY(col, y2, x2);
					// Debug.Log ("case 9B.");
					return true;
				}
			}
		}
		return false;
	}

	protected virtual bool Case10(int x1, int y1, int x2, int y2, int minX, int minY, int maxX, int maxY) {
		// 10. Check X, Y
		// ...x
		// .   
		// .   
		// ....x
		for (int row = 0; row < this.m_Grid.GetLength(0); row++)
		{
			if (this.CheckY(minY, maxY, row)) {
				if (this.CheckXWith(row, x1 - 1, y1) 
					&& this.CheckXWith(row, x2 - 1, y2)) {
					this.DrawLineY(minY, maxY, row);
					this.DrawLineX(row, x1, y1);
					this.DrawLineX(row, x2, y2);
					// Debug.Log ("case 10A.");
					return true;
				}
				if (this.CheckXWith(row, x1 + 1, y1) 
					&& this.CheckXWith(row, x2 + 1, y2)) {
					this.DrawLineY(minY, maxY, row);
					this.DrawLineX(row, x1, y1);
					this.DrawLineX(row, x2, y2);
					// Debug.Log ("case 10B.");
					return true;
				}
			}
		}
		return false;
	}

	#endregion

	#region Ultilities

	public virtual void DrawLineX(int x1, int x2, int y) {
		var minX = Mathf.Min(x1, x2);
		var maxX = Mathf.Max(x1, x2);
		var line = this.CreateLine();
		var length = maxX - minX + 1;
		line.positionCount = length;
		var index = 0;
		for (int x = minX; x <= maxX; x++)
		{
			var cell = this.m_GridCells[x, y];
			var position = cell.transform.localPosition;
			position.z = -100f;
			line.SetPosition(index, position);
			index++;
		}
	}

	public virtual void DrawLineY(int y1, int y2, int x) {
		var minY = Mathf.Min(y1, y2);
		var maxY = Mathf.Max(y1, y2);
		var line = this.CreateLine();
		var length = maxY - minY + 1;
		line.positionCount = length;
		var index = 0;
		for (int y = minY; y <= maxY; y++)
		{
			var cell = this.m_GridCells[x, y];
			var position = cell.transform.localPosition;
			position.z = -100f;
			line.SetPosition(index, position);
			index++;
		}
	}

	protected LineRenderer CreateLine () {
		for (int i = 0; i < this.m_LinePool.Count; i++)
		{
			var linePool = this.m_LinePool[i];
			if (linePool.gameObject.activeInHierarchy == false) {
				linePool.gameObject.SetActive(true);
				return linePool;
			}
		}
		var line = Instantiate(this.m_LinePrefab);
		line.transform.SetParent(this.m_LineRoot);
		line.transform.localPosition = Vector3.zero;
		line.transform.localScale = Vector3.one;
		line.gameObject.SetActive(true);
		this.m_LinePool.Add(line);
		this.m_LinePrefab.gameObject.SetActive (false);
		return line;
	}

	protected void ResetLines() {
		for (int i = 0; i < this.m_LinePool.Count; i++)
		{
			var linePool = this.m_LinePool[i];
			linePool.gameObject.SetActive(false);
		}
	}

	public bool CheckXWith(int x1, int x2, int y) {
		var minX = Mathf.Min(x1, x2);
		var maxX = Mathf.Max(x1, x2);
		return this.CheckX(minX, maxX, y);
	}

	public bool CheckYWith(int y1, int y2, int x) {
		var minY = Mathf.Min(y1, y2);
		var maxY = Mathf.Max(y1, y2);
		return this.CheckY(minY, maxY, x);
	}
	
	public bool CheckX(int x1, int x2, int y) {
		if (x1 < 0 
			|| x1 >= this.m_Grid.GetLength(0)
			|| x2 < 0 
			|| x2 >= this.m_Grid.GetLength(0)
			|| y < 0
			|| y >= this.m_Grid.GetLength(1))
			return false;
		for (int x = x1; x <= x2; x++)
		{
			var value = this.m_Grid[x, y];
			if (value != 0)
				return false;
		}
		return true;
	}

	public bool CheckY(int y1, int y2, int x) {
		if (y1 < 0 
			|| y1 >= this.m_Grid.GetLength(1)
			|| y2 < 0 
			|| y2 >= this.m_Grid.GetLength(1)
			|| x < 0
			|| x >= this.m_Grid.GetLength(0))
			return false;
		for (int y = y1; y <= y2; y++)
		{
			var value = this.m_Grid[x, y];
			if (value != 0)
				return false;
		}
		return true;
	}

	#endregion

}
