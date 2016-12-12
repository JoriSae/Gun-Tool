using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Gun : MonoBehaviour
{
    #region Variables
    public UnityEvent fireEvent;

    #region Bullet Spread Variables
    public bool bulletSpreadVariables;

    //Variation of the angle at which the bullet would exit the gun (from 0)
    public Vector3 bulletSpread;
    //Minimum angle variation
    public Vector3 minBulletSpread;
    //Maximum angle variation
    public Vector3 maxBulletSpread;
    //Adjustment in bullet spread every shot
    public Vector3 recoil;
    //Amount the recoil will be stabilized over time
    public Vector3 recoilStablisation;
    #endregion

    #region Firing Variables
    public bool firingVariables;

    public enum FireState { Automatic, SemiAutomatic, BurstShot };
    public FireState fireState;

    public GameObject projectile;
    public Transform firePoint;
    private Rigidbody projectileRigidBody;
    private Vector3 firingAngle;

    private bool isReloading = false;

    public float firingForce;
    public float shotsPerSecond;
    private float timeBeforeShot;

    #endregion

    #region Ammo Variables
    public bool ammoVariables;

    public float ammo;
    public float clipSize;
    public float ammoPool;
    public float reloadTime;
    #endregion
    #endregion

    void Start()
    {
        if (fireEvent == null)
        {
            fireEvent = new UnityEvent();
        }
    }

	// Update is called once per frame
	void Update()
    {
        UpdateTimers();

        UpdateFiringAngle();

        AdjustRecoil(recoilStablisation);

        ammo = Mathf.Clamp(ammo, 0, clipSize);
    }

    void UpdateTimers()
    {
        //Update timer
        timeBeforeShot -= Time.deltaTime;
    }

    void UpdateFiringAngle()
    {
        //Get firing angle
        firingAngle = transform.rotation.eulerAngles;

        //Assign new random fire angle
        firingAngle.x += bulletSpread.x * Random.Range(-1f, 2f);
        firingAngle.y += bulletSpread.y * Random.Range(-1f, 2f);
        firingAngle.z += bulletSpread.z * Random.Range(-1f, 2f);
    }

    public IEnumerator AttemptReload()
    {
        //Reload ammo
        if (ammoPool > 0 && !isReloading && ammo != clipSize)
        {
            isReloading = true;

            yield return new WaitForSeconds(reloadTime);

            float requiredProjectiles = clipSize - ammo;
            ammo += requiredProjectiles;
            ammoPool -= requiredProjectiles;

            isReloading = false;
        }
    }

    void AdjustRecoil(Vector3 recoilAdjustment)
    {
        //Adjust recoil
        bulletSpread.x = Mathf.Clamp(bulletSpread.x + recoilAdjustment.x, minBulletSpread.x, maxBulletSpread.x);
        bulletSpread.y = Mathf.Clamp(bulletSpread.y + recoilAdjustment.y, minBulletSpread.y, maxBulletSpread.y);
        bulletSpread.z = Mathf.Clamp(bulletSpread.z + recoilAdjustment.z, minBulletSpread.z, maxBulletSpread.z);
    }

    public void AttemptFireProjectile()
    {
        //Check if can shoot
        if (timeBeforeShot < 0 && ammo > 0 && !isReloading)
        {
            --ammo;

            //Spawn projectile and set rotation
            GameObject newProjectile = Instantiate(projectile, firePoint.position, Quaternion.identity) as GameObject;
            newProjectile.transform.rotation = Quaternion.Euler(firingAngle);

            //Set projectile force
            projectileRigidBody = newProjectile.GetComponent<Rigidbody>();
            projectileRigidBody.AddForce(newProjectile.transform.forward * firingForce);

            //Update timer
            timeBeforeShot = shotsPerSecond;

            fireEvent.Invoke();

            //adjust recoil
            AdjustRecoil(recoil);
        }
    }
}


