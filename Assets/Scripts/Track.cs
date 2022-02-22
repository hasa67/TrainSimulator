using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//[ExecuteInEditMode]
public class Track : MonoBehaviour {
	public GameObject arrow;
	public float startAngle = 30f;

//	public bool buildTrack = false;
//	public bool destroyTrack = false;

	private List<Point> pointsList = new List<Point> ();
	private List<float[]> pointsOnTrack = new List<float[]> (); // km, x, y, z, alpha(right/left), betha(up/down), radius
	private float straighDelta = 100f;
	private float curveDelta = 20f;
	private float delta;
	private float km = 0f;
	private float trackLength = 0f;

	void Awake(){
		ReadTrackData ();
		BuildTrack ();
	}

	void Update(){
//		if (buildTrack) {
//			BuildTrack ();
//			buildTrack = false;
//		}
//
//		if (destroyTrack) {
//			DestroyTrack ();
//			destroyTrack = false;
//		}
	}

	private void ReadTrackData(){
		string filePath = Path.Combine (Application.streamingAssetsPath, "TrackData.csv");
		string fileData = System.IO.File.ReadAllText (filePath);
		string[] lines = fileData.Split("\n"[0]);

		for (int i = 1; i < lines.Length; i++) {
			string[] lineData = (lines[i].Trim()).Split(","[0]);
			Point point = new Point ();
			point.length = int.Parse (lineData [0]);
			point.inclination = int.Parse (lineData [1]);
			point.radius = int.Parse (lineData [2]);
			if (int.Parse (lineData [3]) == 0) {
				point.isRight = false;
			} else {
				point.isRight = true;
			}

			pointsList.Add (point);
		}
	}

	private void BuildTrack(){
		float x1 = 0f; float y1 = 0f; float z1 = 0f;
		float x2 = 0f; float y2 = 0f; float z2 = 0f;
		float alpha1 = 0f; float alpha2 = 0f;

		alpha1 = startAngle * Mathf.PI / 180f;

		foreach (var item in pointsList) {
			trackLength += item.length;
		}

		pointsOnTrack.Add (new float[]{ 0f, 0f, 0f, 0f, alpha1, pointsList [0].inclination / 1000, 0f });

		for (int i = 0; i < pointsList.Count; i++) {
			if (pointsList [i].radius == 0) {
				delta = straighDelta;
			} else {
				delta = curveDelta;
			}

			Point pointA = pointsList [i];
			float kmB = km + pointA.length;

			km += delta;

			while (km <= kmB) {
				if (pointA.radius == 0) {
					x1 = pointsOnTrack [pointsOnTrack.Count - 1] [1];
					z1 = pointsOnTrack [pointsOnTrack.Count - 1] [3];
					alpha1 = pointsOnTrack [pointsOnTrack.Count - 1] [4];

					x2 = x1 + delta * Mathf.Cos (alpha1);
					z2 = z1 + delta * Mathf.Sin (alpha1);
					alpha2 = alpha1;
				} else {
					x1 = pointsOnTrack [pointsOnTrack.Count - 1] [1];
					z1 = pointsOnTrack [pointsOnTrack.Count - 1] [3];
					alpha1 = pointsOnTrack [pointsOnTrack.Count - 1] [4];

					x2 = x1 + delta * Mathf.Cos (alpha1);
					z2 = z1 + delta * Mathf.Sin (alpha1);
					if (pointA.isRight) {
						alpha2 = alpha1 - (delta / pointA.radius);
					} else {
						alpha2 = alpha1 + (delta / pointA.radius);
					}
				}

				y1 = pointsOnTrack [pointsOnTrack.Count - 1] [2];
				y2 = y1 + delta * pointA.inclination / 1000;

				pointsOnTrack.Add (new float[]{ km, x2, y2, z2, alpha2, pointA.inclination / 1000, pointA.radius });

				km += delta;
			}

			km -= delta;
		}

		foreach (var item in pointsOnTrack) {
			Vector3 arrowPosition = new Vector3(item[1], item[2], item[3]);
			GameObject newArrow = Instantiate (arrow, arrowPosition, Quaternion.identity);
			newArrow.transform.Rotate (new Vector3 (90f, 0f, 0f));
			newArrow.transform.Rotate (new Vector3 (0f, 0f, item [4] * 180f / Mathf.PI - 90f));
			newArrow.transform.Rotate (new Vector3 (-item [5] * 180f / Mathf.PI, 0f, 0f));
			newArrow.transform.parent = transform;
		}
	}

	public float GetTrackDataPoint(int i, int j){
		return pointsOnTrack [i] [j];
	}

	public float GetTrackDataPointByKm(float newKm, int j){
		int pointIndex = 0;

		for (int i = 0; i < pointsOnTrack.Count; i++) {
			if (pointsOnTrack [i] [0] > newKm) {
				pointIndex = i;
				break;
			}
		}

		return pointsOnTrack [pointIndex] [j];
	}

	public int GetTrackDataCount(){
		return pointsOnTrack.Count;
	}

//	private void DestroyTrack(){
//		List<GameObject> objects = new List<GameObject> ();
//		if (transform.childCount > 0) {
//			for (int i = 0; i < transform.childCount; i++) {
//				objects.Add (transform.GetChild (i).gameObject);
//			}
//			foreach (var item in objects) {
//				DestroyImmediate (item);
//			}
//		}
//	}

}
