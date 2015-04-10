using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayoutPath : MonoBehaviour
{
	[SerializeField]
	private List<Vector3> points = new List<Vector3>();
	private List<float> segments = new List<float>();

	public GameObject merker;
	private GameObject pacenotes;

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	[ShowOnly, Tooltip("Length in meters")]
	public float Length;
	[ShowOnly, Tooltip("Track Percentage")]
	public float Perc;
	[ShowOnly, Tooltip("Position on Path")]
	public Vector3 PosOnPath;

	private Vector3 pointPrec;
	private Vector3 pointNext;

	public Vector3 GetPoint(int index)
	{
		if (index < points.Count)
		{
			return points [index];
		}
		else
		{
			return new Vector3();	
		}
	}

	public int GetPointsCount()
	{
		return points.Count;
	}

	public void Clear()
	{
		points.Clear();
		calcLength();
	}

	public void RemoveAt(int index)
	{
		points.RemoveAt (index);
		calcLength();
	}

	public void UpdatePoint (int index, Vector3 point)
	{
		points[index] = point;
		calcLength();
	}

	public void AddPacenote(Vector3 posHit)
	{
		GetPositionOnPath (posHit);

		string path = "Assets/gTrackEditor/Generic.FBX";
		GameObject anchor_point = Resources.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
		// Instance this prefab
		GameObject prefab_instance = Instantiate(anchor_point) as GameObject;
		// Place the prefab at correct position (position of the hit).
		prefab_instance.transform.position = PosOnPath;

		prefab_instance.transform.rotation = Quaternion.LookRotation(pointNext - pointPrec) * Quaternion.Euler(270, 0, 0);
		prefab_instance.transform.SetParent (pacenotes.transform);
		prefab_instance.name = "LEFT_6";

		//GameObject note = (GameObject)Instantiate(merker, PosOnPath, Quaternion.identity);
		//note.transform.SetParent (pacenotes.transform);
	}

	public void BuildObject(string pathXml)
	{
		gUtility.CXml xSpline = new gUtility.CXml (pathXml, false);

		pacenotes = new GameObject("LayoutPacenotes");
		pacenotes.transform.SetParent (transform);

		for (int i = 1; i <= xSpline.Settings.GetNamedChildrenCount("P"); ++i)
		{
			string POINT = string.Format("P#{0}", i);
			gUtility.Vector3 pos = xSpline.Settings[POINT].ReadVector3("pos", gUtility.Vector3.ZERO);
			Vector3 posU = new Vector3(-pos.x, pos.z, -pos.y);
			points.Add(posU);
			/*
			GameObject point = GameObject.CreatePrimitive(PrimitiveType.Capsule); //new GameObject(string.Format("point{0}", i - 1));
			point.name = string.Format("point{0}", i - 1);
			point.transform.localScale = new UnityEngine.Vector3(0.5f, 2.0f, 0.5f);
			point.transform.SetParent(pointsObj.transform);
			point.transform.position = posU;
			*/
		}
		calcLength();
	}

	private void calcLength()
	{
		Length = 0;
		segments.Clear ();
		for (int i = 0; i < points.Count - 1; i++)
		{
			float len = (points[i +1] - points[i]).magnitude;
			segments.Add(len);
			Length += len;
		}
	}

	int segMin = -1;
	Vector3 AP = Vector3.zero;
	Vector3 AB = Vector3.zero;
	float magnitudeAB = 0.0f;
	float ABAPproduct = 0.0f;
	float distRel = 0.0f;
	float dist = 0.0f;
	float distSegment = 0.0f;
	float fMin = 999999.0f;
	public void GetPositionOnPath(Vector3 posInWorld)
	{
		fMin = 999999.0f;
		for (int i = 0; i < points.Count - 1; i++)
		{
			if (getClosestPoint(posInWorld, points[i], points[i + 1]) == 0)
			{
				if (distSegment < fMin)
				{
					// questo è quello buono
					segMin = i;
					fMin = distSegment;
				}
			}
		}
		pointPrec = points [segMin];
		pointNext = points [segMin + 1];
		getClosestPoint (posInWorld, pointPrec, pointNext);
		//dist è la distanza dal punto, quindi devo calcolare le distanze precedenti

		// son dentro, calcolo la distanza totale e la percentuale giusta
		for (int i = 0; i < segMin; i++)
		{
			dist += segments[i];
		}
		Perc = 100.0f / Length * dist;
	}

	private int getClosestPoint(Vector3 P, Vector3 A, Vector3 B)
	{
		AP = P - A;
		AB = B - A;
		magnitudeAB = AB.sqrMagnitude;
		ABAPproduct = Vector3.Dot(AP, AB);
		distRel = ABAPproduct / magnitudeAB;
		
		if (distRel < 0.0f)
		{
			// segmento precedente	
			return -1;
		}
		else if (distRel > 1.0f)
		{
			// segmento successivo
			return 1;
		}
		else
		{
			// son dentro!
			PosOnPath = A + AB * distRel;
			dist = (PosOnPath - A).magnitude;
			distSegment = (P - PosOnPath).magnitude;
			return 0;
		}
		return 1;
	}
}
