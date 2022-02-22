using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrack : MonoBehaviour {

	public VehicleInfo vehicleInfo;

	private int pointNumber = 1;
	private float km = 0f;
	private int trackDataCount = 0;

	private Vector3 vehiclePosition;
	private Vector3 vehicleRotation;
	private TrainManager tm;


	public void Initialize(float newKm, TrainManager trainManager){
		tm = trainManager;

		km = newKm;
		SetPosition (km);

		trackDataCount = tm.GetTrackDataCount ();
	}

	public void SetPosition(float newKm){
		km = newKm;

		for (int i = 0; i < trackDataCount; i++) {
			if (tm.GetTrackDataPoint (i, 0) > km) {
				pointNumber = i;
				break;
			}
		}

		Vector3 pointA = new Vector3 (tm.GetTrackDataPoint (pointNumber - 1, 1),
			                 tm.GetTrackDataPoint (pointNumber - 1, 2),
			                 tm.GetTrackDataPoint (pointNumber - 1, 3));
		Vector3 pointB = new Vector3 (tm.GetTrackDataPoint (pointNumber, 1),
			                 tm.GetTrackDataPoint (pointNumber, 2),
			                 tm.GetTrackDataPoint (pointNumber, 3));

		float yRotateA = 90f - tm.GetTrackDataPoint(pointNumber - 1, 4) * 180f / Mathf.PI;
		float yRotateB = 90f - tm.GetTrackDataPoint (pointNumber, 4) * 180f / Mathf.PI;

		float xRotateA = -tm.GetTrackDataPoint (pointNumber - 1, 5) * 180f / Mathf.PI;
		float xRotateB = -tm.GetTrackDataPoint (pointNumber, 5) * 180f / Mathf.PI;

		float kmA = tm.GetTrackDataPoint (pointNumber - 1, 0);
		float kmB = tm.GetTrackDataPoint (pointNumber, 0);

		float t = (km - kmA) / (kmB - kmA);
		gameObject.transform.position = Vector3.Lerp (pointA, pointB, t);
		gameObject.transform.localEulerAngles = new Vector3 (Mathf.Lerp (xRotateA, xRotateB, t),
			Mathf.Lerp (yRotateA, yRotateB, t), 0f);
	}
}
