using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyConfig", menuName="My Config")]
public class ConfigScriptableObject : ScriptableObject {

	public int gridNumberX = 5;
	public int gridNumberZ = 5;
	public float size = 0.03f;
	public float borderSize = 0.005f;
	public Material borderMaterial;
	public Material quadMaterial;
	public Material quadActiveMaterial;
	public Material quadOnMaterial;
	public Material quadInfoMaterial;
	public Material quadWarningMaterial;
	public Material quadErrorMaterial;
	public Material quadPathMaterial;
	public Material obstacleMaterial;
    public Material transparentMaterial;
	public Material quadStarMaterial;
	public Material quadCircleMaterial;
	public Material quadLetterAnimationMaterial;
    public GameObject quadPrefab;
    public GameObject borderPrefab;
    public GameObject quadLetterPrefab;
    public GameObject particleSystemShowRobyPrefab;
	public GameObject[] robotPrefabs;
	public GameObject robotWithHatPrefab;

	public GameConfig gameConfig;


}
