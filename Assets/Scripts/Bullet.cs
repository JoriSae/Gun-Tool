using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    #region Variables
    [SerializeField] private AnimationCurve damageOverTime;
    [SerializeField] private float bulletDamage;
    [SerializeField] private float bulletMass;
    [SerializeField] private float bulletFragmentation;
    [SerializeField] private float bulletCleanUpTime;
    [SerializeField] private Vector3 dragResistance;
    private Rigidbody rigidBody;
    #endregion

    // Use this for initialization
    void Start () {
        Destroy(gameObject, bulletCleanUpTime);

        rigidBody = GetComponent<Rigidbody>();
        rigidBody.mass = bulletMass;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
