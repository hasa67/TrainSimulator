using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Vehicle", menuName = "Vehicle")]
public class VehicleInfo : ScriptableObject {
	public bool isLoco;

	public float weight; // (tonnes)
	public int axleCount;

	public string notchPowerFileName; // (hp)

	[Range(0,1)]
	public float locoEfficiency; // (0..1)
}
