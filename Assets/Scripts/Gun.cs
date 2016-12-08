using UnityEngine;
using XInputDotNetPure;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Gun))]
[CanEditMultipleObjects]
public class ScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Gun script = target as Gun;

        #region GUI Object Initialization
        //Allows you to give a variable a mouse over tooltip
        GUIContent gui_BulletSpread = new GUIContent("Bullet Spread", "Variation of the angle at which the bullet would exit the gun (from 0)");
        GUIContent gui_MaxBulletSpread = new GUIContent("Max Bullet Spread", "Maximum angle variation");
        GUIContent gui_MinBulletSpread = new GUIContent("Min Bullet Spread", "Minimum angle variation");
        GUIContent gui_Recoil = new GUIContent("Recoil", "Adjustment in bullet spread every shot");
        GUIContent gui_RecoilStablisation = new GUIContent("Recoil Stablisation", "Amount the recoil will be stabilized over time");
        #endregion

        #region Firing
        script.firingVariables = EditorGUILayout.BeginToggleGroup("Firing Variables:", script.firingVariables);
        if (script.firingVariables)
        {
            script.firingForce = EditorGUILayout.FloatField("Firing Force", script.firingForce);
            script.shotsPerSecond = EditorGUILayout.FloatField("Shots Per Second", script.shotsPerSecond);
            script.projectile = (GameObject)EditorGUILayout.ObjectField("Projectile", script.projectile, typeof(GameObject), true);
            script.firePoint = (Transform)EditorGUILayout.ObjectField("Firing Point", script.firePoint, typeof(Transform), true);
            script.fireState = (Gun.FireState)EditorGUILayout.EnumPopup("Fire State", script.fireState);
        }
        EditorGUILayout.EndToggleGroup();
        #endregion

        #region Bullet Spread
        script.bulletSpreadVariables = EditorGUILayout.BeginToggleGroup("Bullet Spread Variables:", script.bulletSpreadVariables);
        if (script.bulletSpreadVariables)
        {
            script.bulletSpread = EditorGUILayout.Vector3Field(gui_BulletSpread, script.bulletSpread);
            script.maxBulletSpread = EditorGUILayout.Vector3Field(gui_MaxBulletSpread, script.maxBulletSpread);
            script.minBulletSpread = EditorGUILayout.Vector3Field(gui_MinBulletSpread, script.minBulletSpread);
            script.recoil = EditorGUILayout.Vector3Field(gui_Recoil, script.recoil);
            script.recoilStablisation = EditorGUILayout.Vector3Field(gui_RecoilStablisation, script.recoilStablisation);
        }
        EditorGUILayout.EndToggleGroup();
        #endregion

        #region Ammo
        script.ammoVariables = EditorGUILayout.BeginToggleGroup("Ammo Variables:", script.ammoVariables);
        if (script.ammoVariables)
        {
            script.ammo = EditorGUILayout.FloatField("Ammo", script.ammo);
            script.clipAmount = EditorGUILayout.FloatField("Clip Amount", script.clipAmount);
            script.clipSize = EditorGUILayout.FloatField("Clip Size", script.clipSize);
            script.reloadTime = EditorGUILayout.FloatField("Reload Time", script.reloadTime);
        }
        EditorGUILayout.EndToggleGroup();
        #endregion

        #region Controller
        script.controllerVariables = EditorGUILayout.BeginToggleGroup("Controller Variables:", script.controllerVariables);
        if (script.controllerVariables)
        {
            script.vibrationTime = EditorGUILayout.FloatField("Vibration Time", script.vibrationTime);
            script.vibrationIntensity = EditorGUILayout.Vector2Field("Vibration Intensity", script.vibrationIntensity);
        }
        EditorGUILayout.EndToggleGroup();
        #endregion
    }
}

public class Gun : MonoBehaviour {

    #region Variables
    #region Controller Variables
    public bool controllerVariables;
    private bool playerIndexSet = false;
    private PlayerIndex playerIndex;
    private GamePadState state;
    private GamePadState prevState;
    private float timeSinceLastShot;
    public float vibrationTime;
    public Vector2 vibrationIntensity;
    #endregion

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
    private float previousState;
    private float lateReloadKey;
    public enum FireState { Automatic, SemiAutomatic, BurstShot };
    public FireState fireState;
    public float firingForce;
    private Vector3 firingAngle;
    public float shotsPerSecond;
    private bool canShoot = true;
    private float timeBeforeShot;
    public GameObject projectile;
    public Transform firePoint;
    private Rigidbody projectileRigidBody;
    #endregion

    #region Ammo Variables
    public bool ammoVariables;
    public float ammo;
    public float clipSize;
    public float clipAmount;
    public float reloadTime;
    #endregion
    #endregion
	
    void Start()
    {
        if (clipAmount > 0)
        {
            --clipAmount;
            ammo = clipSize;
        }
    }

	// Update is called once per frame
	void Update()
    {
        UpdateInput();

        UpdateTimers();

        UpdateFiringAngle();

        CalculateFiringState();

        AdjustRecoil(recoilStablisation);

        UpdateVibration();
    }

    void UpdateVibration()
    {
        //If timer equals 0, set vibration to 0
        if (timeSinceLastShot <= 0)
        {
            GamePad.SetVibration(playerIndex, 0, 0);
        }
    }

    void UpdateTimers()
    {
        //Update timers
        timeBeforeShot -= Time.deltaTime;
        timeSinceLastShot -= Time.deltaTime;
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

    void UpdateInput()
    {
        if(Input.GetAxis("Reload") > 0 && ammo != clipSize && lateReloadKey == 0 && clipAmount > 0 && canShoot)
        {
            canShoot = false;
            Invoke("Reload", reloadTime);
        }
    }

    void Reload()
    {
        --clipAmount;
        ammo = clipSize;
        canShoot = true;
    }

    void CalculateFiringState()
    {
        //Check fire state and fire accordingly
        switch (fireState)
        {
            case FireState.Automatic:
                if (Input.GetAxis("Fire") > 0)
                {
                    AttemptFireProjectile();
                }
                break;
            case FireState.SemiAutomatic:
                if (previousState == 0 && Input.GetAxis("Fire") > 0)
                {
                    AttemptFireProjectile();
                }
                break;
        }

    }

    void LateUpdate()
    {
        //Check if button is still pressed
        previousState = Input.GetAxis("Fire");
        lateReloadKey = Input.GetAxis("Reload");
    }

    void AdjustRecoil(Vector3 recoilAdjustment)
    {
        //Adjust recoil
        bulletSpread.x = Mathf.Clamp(bulletSpread.x + recoilAdjustment.x, minBulletSpread.x, maxBulletSpread.x);
        bulletSpread.y = Mathf.Clamp(bulletSpread.y + recoilAdjustment.y, minBulletSpread.y, maxBulletSpread.y);
        bulletSpread.z = Mathf.Clamp(bulletSpread.z + recoilAdjustment.z, minBulletSpread.z, maxBulletSpread.z);
    }

    void AttemptFireProjectile()
    {
        //Check if can shoot
        if (timeBeforeShot < 0 && ammo > 0 && canShoot)
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

            //Update vibration timer and set vibration
            timeSinceLastShot = vibrationTime;
            GamePad.SetVibration(playerIndex, vibrationIntensity.x, vibrationIntensity.y);

            //adjust recoil
            AdjustRecoil(recoil);
        }
    }

    void FindGamePad()
    {
        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    //Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }

        prevState = state;
        state = GamePad.GetState(playerIndex);
    }
}


