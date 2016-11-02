using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    #region Variables
    [SerializeField] private AnimationCurve damageOverTime;
    [SerializeField] private float bulletDamage;
    [SerializeField] private float bulletMass;
    [SerializeField] private float bulletFragmentation;
    [SerializeField] private Vector3 bulletForce;
    [SerializeField] private Vector3 dragResistance;
    #endregion

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
