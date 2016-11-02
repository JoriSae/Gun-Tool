using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    #region Variables
    #region Bullet Spread Variables
    [SerializeField] private Vector3 bulletSpread;
    [SerializeField] private Vector3 minBulletSpread;
    [SerializeField] private Vector3 maxBulletSpread;
    [SerializeField] private Vector3 recoil;
    [SerializeField] private float recoilStablisation;
    [SerializeField] private float timeUntilRecoilStabilisation;
    #endregion

    #region Firing Variables
    private enum FireState { Automatic, SemiAutomatic, BurstShot };
    [SerializeField] private FireState fireState;
    [SerializeField] private bool bulletDrop;
    [SerializeField] private float firingForce;
    [SerializeField] private Vector3 firingAngle;
    [SerializeField] private float shotsPerSecond;
    private float timeBeforeShot;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;
    private Rigidbody projectileRigidBody;
    #endregion

    #region Ammo Variables
    [SerializeField] private float ammo;
    [SerializeField] private float clipSize;
    #endregion
    #endregion

    // Use this for initialization
    void Start()
    {
    }
	
	// Update is called once per frame
	void Update()
    {
        UpdateInput();

        UpdateTimers();

        UpdateFiringAngle();

        CalculateFiringState();
    }

    void UpdateInput()
    {

    }

    void UpdateTimers()
    {
        timeBeforeShot -= Time.deltaTime;
    }

    void UpdateFiringAngle()
    {
        firingAngle = transform.rotation.eulerAngles;
    }

    void CalculateFiringState()
    {
        switch (fireState)
        {
            case FireState.Automatic:
                if (Input.GetMouseButton(0))
                {
                    AttemptFireProjectile();
                }
                break;
            case FireState.SemiAutomatic:
                if (Input.GetMouseButtonDown(0))
                {
                    AttemptFireProjectile();
                }
                break;
        }

    }

    void AttemptFireProjectile()
    {
        if (timeBeforeShot < 0)
        {
            GameObject newProjectile = Instantiate(projectile, firePoint.position, Quaternion.identity) as GameObject;
            newProjectile.transform.rotation = Quaternion.Euler(firingAngle);
            projectileRigidBody = newProjectile.GetComponent<Rigidbody>();
            projectileRigidBody.AddForce(newProjectile.transform.forward * firingForce);
            timeBeforeShot = shotsPerSecond;
        }

        
    }
}
