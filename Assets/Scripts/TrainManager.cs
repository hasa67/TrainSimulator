using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class TrainManager : MonoBehaviour {

	public Track track;
	public GameObject locoModel;
	public GameObject wagonModel;
	public Text speedText; // (km/h)
	public Text kmText; // (km) + (m)

	private int locoCount = 1;
	private float locoLength = 20f; // (m)
	private int wagonCount = 12;
	private float wagonLength = 27.5f; // (m)

	private float accel = 100f; // (m/s^2)
	private float speed = 0f; // (m/s)
	private float km = 0f; // (m)
	private float newKm = 0f; // km of each vehivle
	private int trackDataCount = 0;

	private float[] notchPower;
	private int notch = 0;
	private int notchMax = 0;

	private float tractionEffort = 0f; // (kgf)
	private float totalResistForce = 0f; // (kgf)

	private List<GameObject> locoList = new List<GameObject>();
	private List<GameObject> wagonList = new List<GameObject>();
	private List<GameObject> trainList = new List<GameObject> ();
	private List<float> trainsKm = new List<float> (); // list of km of each vehicles in each frame


	void Start () {
		trackDataCount = track.GetTrackDataCount ();
		ClampKm ();

		for (int i = 0; i < locoCount; i++) {
			newKm = km - i * locoLength;
			GameObject newLoco = Instantiate (locoModel, this.transform.position, Quaternion.identity);
			newLoco.GetComponent<FollowTrack> ().Initialize (newKm, this.GetComponent<TrainManager> ());
			locoList.Add (newLoco);
			trainList.Add (newLoco);
			trainsKm.Add (newKm);

			if (i == 0) {
				Camera mainCam = Camera.main;
				mainCam.transform.SetParent (newLoco.transform);
				mainCam.transform.localPosition = new Vector3 (0.23f, 13.33f, -28.01f);
				mainCam.transform.localRotation = Quaternion.Euler (new Vector3 (10f, 0f, 0f));
			}
		}

		notchPower = ReadLocoNotchPower (locoList [0].GetComponent<FollowTrack> ().vehicleInfo.notchPowerFileName);
		notchMax = notchPower.Length - 1;

		for (int i = 0; i < wagonCount; i++) {
			newKm = km - (i + 1) * wagonLength - (locoCount - 1) * locoLength;
			GameObject newWagon = Instantiate (wagonModel, this.transform.position, Quaternion.identity);
			newWagon.GetComponent<FollowTrack> ().Initialize (newKm, this.GetComponent<TrainManager> ());
			wagonList.Add (newWagon);
			trainList.Add (newWagon);
			trainsKm.Add (newKm);
		}
	}
		

	void Update () {
		speedText.text = string.Concat ("Speed: ", Mathf.Floor (speed * 3.6f).ToString (), " KPH");
		kmText.text = string.Concat ("KM: ", Mathf.Floor (km / 1000).ToString (),
			"+", Mathf.Floor (km % 1000).ToString ());

		if (Input.GetKey ("w")) {
			speed += accel * Time.deltaTime;
		}
		if (Input.GetKey ("s")) {
			speed -= accel * Time.deltaTime;
		}

		trainsKm.Clear ();

		for (int i = 0; i < locoCount; i++) {
			GameObject loco = locoList [i];
			newKm = km - i * locoLength;
			loco.GetComponent<FollowTrack> ().SetPosition (newKm);
			trainsKm.Add (newKm);
		}

		for (int i = 0; i < wagonCount; i++) {
			GameObject wagon = wagonList [i];
			newKm = km - (i + 1) * wagonLength - (locoCount - 1) * locoLength;
			wagon.GetComponent<FollowTrack> ().SetPosition (newKm);
			trainsKm.Add (newKm);
		}

		km += speed * Time.deltaTime;
		ClampKm ();

		CalculateResistance ();
	}

	private void ClampKm(){
		km = Mathf.Clamp (km, track.GetTrackDataPoint (0, 0) + wagonCount * wagonLength + (locoCount - 1) * locoLength,
			track.GetTrackDataPoint (trackDataCount - 1, 0) - locoLength);
	}

	private void CalculateResistance(){
		totalResistForce = 0f;

		for (int i = 0; i < trainList.Count; i++) {
			VehicleInfo vi = trainList [i].GetComponent<FollowTrack> ().vehicleInfo;

			bool isLoco = vi.isLoco;
			float weight = vi.weight;
			int axleCount = vi.axleCount;

			float a;
			float b;
			if (isLoco) {
				a = 12.3f;
				b = 0.00453f;
			} else {
				a = 9f;
				b = 0.000372f;
			}

			float vehicleResist = 0.65f + 13.2f / (weight / axleCount) + 0.00931f * speed + a * b / weight * Mathf.Pow (speed, 2);
			totalResistForce += vehicleResist * weight;

			float curveResist = 0f;
			float curveRadius = track.GetTrackDataPointByKm (trainsKm [i], 6);
			if (curveRadius >= 500f) {
				curveResist = 650f / (curveRadius - 55f) * vi.weight;
			} else if (curveRadius > 50f) {
				curveResist = 500f / (curveRadius - 30f) * vi.weight;
			}
			totalResistForce += curveResist;

			float gradientResist = track.GetTrackDataPointByKm (trainsKm [i], 5) * vi.weight;
			totalResistForce += gradientResist;
		}
	}

	private void CalculateTractionEffort(){
		
	}

	private float[] ReadLocoNotchPower(string fileName){
		string filePath = Path.Combine (Application.streamingAssetsPath, string.Concat (fileName, ".csv"));
		string lineData = System.IO.File.ReadAllText (filePath);
		string[] notchData= lineData.Split(","[0]);

		float[] notchFloat = new float[notchData.Length];

		for (int i = 0; i < notchData.Length; i++) {
			notchFloat [i] = float.Parse (notchData [i]);
		}
			
		return notchFloat;
	}

	public int GetTrackDataCount(){
		return trackDataCount;
	}

	public float GetTrackDataPoint(int i, int j){
		return track.GetTrackDataPoint (i, j);
	}
}
