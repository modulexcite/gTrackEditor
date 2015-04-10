using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LayoutPath))]
public class LayoutPathEditor : Editor
{
	private LayoutPath layoutPath;
	private Transform handleTransform;
	private Quaternion handleRotation;
	private int selectedIndex = -1;
	private const float handleSize = 0.06f;
	private const float pickSize = 0.08f;

	private bool insertMerker = false;

	private void OnSceneGUI ()
	{
		layoutPath = target as LayoutPath;

		handleTransform = layoutPath.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

		for (int i = 0; i < layoutPath.GetPointsCount(); ++i)
		{
			Vector3 p0 = ShowPoint(i);

			if (i < layoutPath.GetPointsCount() -1)
			{
				Vector3 p1 = ShowPoint(i + 1);	
				Handles.color = Color.white;
				Handles.DrawLine(p0, p1);
			}
		}

		if (Event.current.type == EventType.MouseUp && insertMerker )
		{
			// Shoot a ray from the mouse position into the world
			Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			RaycastHit hitInfo;
			// Shoot this ray. check in a distance of 10000.
			if (Physics.Raycast(worldRay, out hitInfo, 10000))
			{
				/*
				// Load the current prefab
				string path = "Assets/Prefabs/" + __typeStrings[__currentType] + ".prefab";
				GameObject anchor_point = Resources.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
				// Instance this prefab
				GameObject prefab_instance = PrefabUtility.InstantiatePrefab(anchor_point) as GameObject;
				// Place the prefab at correct position (position of the hit).
				prefab_instance.transform.position = hitInfo.point;
				prefab_instance.transform.parent = __objectGroup.transform;
				*/
				// Mark the instance as dirty because we like dirty
				layoutPath.AddPacenote(hitInfo.point);
				//EditorUtility.SetDirty(prefab_instance);
			}
			insertMerker = false;
			/*
			Ray ray = Camera.current.ScreenPointToRay(Event.current.mousePosition);
			//Debug.Log(Event.current.mousePosition);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit, 1000.0f))
			{
				//Debug.Log(hit.);
				layoutPath.AddPacenote(hit.point);
				//Vector3 newTilePosition = hit.point;
				//Instantiate(newTile, newTilePosition, Quaternion.identity);
			}
			*/
		}
	}

	private Vector3 ShowPoint (int index)
	{
		Vector3 point = handleTransform.TransformPoint(layoutPath.GetPoint (index));
		float size = HandleUtility.GetHandleSize(point);
		if (index == 0)
		{
			size *= 2f;
		}
		Handles.color = Color.yellow;
		if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotCap))
		{
			selectedIndex = index;
			Repaint();
		}
		if (selectedIndex == index)
		{
			//Debug.Log("index pressed: " + index.ToString());
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, handleRotation);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(layoutPath, "Move Point");
				EditorUtility.SetDirty(layoutPath);
				layoutPath.UpdatePoint(index, handleTransform.InverseTransformPoint(point));
			}
		}
		return point;
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		layoutPath = target as LayoutPath;
		if(GUILayout.Button("Import XML"))
		{
			var path = EditorUtility.OpenFilePanel("Select the XML that contains the spline path points",
				"",
				"xml");
			if (path.Length != 0)
			{
				layoutPath.BuildObject(path);
			}
		}
		if (selectedIndex >= 0)
		{
			if (GUILayout.Button("Remove selected point"))
			{
				layoutPath.RemoveAt(selectedIndex);	
			}
		}
		if (GUILayout.Button("Clear"))
		{
			layoutPath.Clear();	
		}
		insertMerker = GUILayout.Toggle (insertMerker, "Add marker");
	}
}

