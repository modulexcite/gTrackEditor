using UnityEngine;
using UnityEditor;
using System.IO;
using Ionic.Zip;

public class gRallyTools {
    [MenuItem("Assets/gRally Generate Track")]
    static void GenerateTrack()
	{
        // Bring up save panel
        string path = EditorUtility.SaveFilePanel("Save Track", "", "track", "unity3d");

        if (path.Length != 0)
		{
			FileInfo fileInfo= new FileInfo(path);
			string sTrackName = fileInfo.DirectoryName + "\\track.xml";
			//string sZipName = fileInfo.DirectoryName + "\\track.gZip";

			// Build the resource file from the active selection.
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies |
																					BuildAssetBundleOptions.CompleteAssets |
																					BuildAssetBundleOptions.UncompressedAssetBundle);

			// ora cerco se ho selezionato anche il file track.xml
			for (int i = 0; i < selection.Length; i++)
			{
				//string sName = selection[i].name;

				if(selection[i].GetType() == typeof(TextAsset))
				{
					TextAsset ta = (TextAsset)selection[i];
					if (ta.name == "track")
					{
						// salvo l'xml parallelo al file
						if(File.Exists(sTrackName))
						{
							File.Delete(sTrackName);
						}
						File.WriteAllText(sTrackName, ta.text);
					}
				}
				//string sType = selection[i].GetType().ToString();
				//Debug.Log(string.Format("{0} [{1}]", sName, sType));
			}
			/* per il momento mi crasha ingame
			try {
				ZipFile zipFile = new ZipFile();
				zipFile.UseUnicodeAsNecessary = true;
				zipFile.AddFile(path, "");
				if(File.Exists(sTrackName))
				{
					zipFile.AddFile(sTrackName, "");
				}
				zipFile.Save(sZipName);
				Debug.Log(sZipName + " created!");
			} catch (System.Exception ex) {
				Debug.Log(ex.ToString());
			}
			*/
			Selection.objects = selection;
        }

    }


	[MenuItem ("gRally/Create Prefab From Selected")]
	static void CreatePrefabMenu ()
	{
		GameObject go = Selection.activeGameObject;
		string name = go.name;

		Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + name + ".prefab");
		PrefabUtility.ReplacePrefab(go, prefab);
		AssetDatabase.Refresh();
	}

	/*
	[MenuItem("GameObject/gRally Create Road Asset")]
	static void CreateRoadAsset()
	{
		GameObject go = Selection.activeGameObject;
		string name = go.name;
		Mesh m = go.GetComponent<MeshFilter>().sharedMesh;
		string path = "Assets/" + name + ".asset";
		AssetDatabase.CreateAsset(m, path);
        AssetDatabase.SaveAssets();
	//	Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/_river_mesh.asset", typeof(Mesh));
		Debug.Log("Mesh asset saved: Assets/" + name + ".asset");
	}
	*/
	/// <summary>
	/// Validates the menu.
	/// The item will be disabled if no game object is selected.
	/// </summary>
	/// <returns>True if the menu item is valid.</returns>
	[MenuItem ("GameObject/gRally Create Prefab From Selected", true)]
	static bool ValidateCreatePrefabMenu ()
	{
		return Selection.activeGameObject != null;
	}


	/*
	[MenuItem("Assets/Build AssetBundle From Selection - Track dependencies")]
    static void ExportResource () {
        // Bring up save panel
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0) {
            // Build the resource file from the active selection.
            Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path,
                              BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets);
            Selection.objects = selection;
        }
    }
    [MenuItem("Assets/Build AssetBundle From Selection - No dependency tracking")]
    static void ExportResourceNoTrack () {
        // Bring up save panel
        string path = EditorUtility.SaveFilePanel ("Save Resource", "", "New Resource", "unity3d");
        if (path.Length != 0) {
            // Build the resource file from the active selection.
            BuildPipeline.BuildAssetBundle(Selection.activeObject, Selection.objects, path);
        }
    }
    */
}
