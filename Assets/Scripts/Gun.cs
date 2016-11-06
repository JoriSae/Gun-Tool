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
            script.bulletSpread = EditorGUILayout.Vector3Field(gui_BulletSpread, script.bulletSpread);
            script.maxBulletSpread = EditorGUILayout.Vector3Field(gui_MaxBulletSpread, script.maxBulletSpread);
        }
        EditorGUILayout.EndToggleGroup();
        #endregion

        script.firingVariables = EditorGUILayout.BeginToggleGroup("Firing Variables:", script.firingVariables);
        if (script.firingVariables)
        {
            script.bulletSpread = EditorGUILayout.Vector3Field(gui_BulletSpread, script.bulletSpread);
            script.maxBulletSpread = EditorGUILayout.Vector3Field(gui_MaxBulletSpread, script.maxBulletSpread);
            script.minBulletSpread = EditorGUILayout.Vector3Field(gui_MinBulletSpread, script.minBulletSpread);
            script.recoil = EditorGUILayout.Vector3Field(gui_Recoil, script.recoil);
            script.recoilStablisation = EditorGUILayout.Vector3Field(gui_RecoilStablisation, script.recoilStablisation);
        }
        EditorGUILayout.EndToggleGroup();
    }
}

public class Gun : MonoBehaviour {

    #region Variables
    #region Controller Variables
    private bool playerIndexSet = false;
    private PlayerIndex playerIndex;
    private GamePadState state;
    private GamePadState prevState;
    private float timeSinceLastShot;
    [SerializeField] private float vibrationTime;
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
    public enum FireState { Automatic, SemiAutomatic, BurstShot };
    public FireState fireState;
    public float firingForce;
    public Vector3 firingAngle;
    public float shotsPerSecond;
    private float timeBeforeShot;
    public GameObject projectile;
    public Transform firePoint;
    private Rigidbody projectileRigidBody;
    #endregion

    #region Ammo Variables
    public bool ammoVariables;
    [SerializeField] private float ammo;
    [SerializeField] private float clipSize;
    [SerializeField] private float reloadTime;
    #endregion
    #endregion
	
	// Update is called once per frame
	void Update()
    {
        UpdateTimers();

        UpdateFiringAngle();

        CalculateFiringState();

        AdjustRecoil(recoilStablisation);

        UpdateVibration();
    }

    void UpdateVibration()
    {
        if (timeSinceLastShot <= 0)
        {
            GamePad.SetVibration(playerIndex, 0, 0);
        }
    }

    void UpdateTimers()
    {
        timeBeforeShot -= Time.deltaTime;
        timeSinceLastShot -= Time.deltaTime;
    }

    void UpdateFiringAngle()
    {
        firingAngle = transform.rotation.eulerAngles;

        firingAngle.x += bulletSpread.x * Random.Range(-1f, 2f);
        firingAngle.y += bulletSpread.y * Random.Range(-1f, 2f);
        firingAngle.z += bulletSpread.z * Random.Range(-1f, 2f);

        print(firingAngle);
    }

    void CalculateFiringState()
    {
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
        previousState = Input.GetAxis("Fire");
    }

    void AdjustRecoil(Vector3 recoilAdjustment)
    {
        bulletSpread.x = Mathf.Clamp(bulletSpread.x + recoilAdjustment.x, minBulletSpread.x, maxBulletSpread.x);
        bulletSpread.y = Mathf.Clamp(bulletSpread.y + recoilAdjustment.y, minBulletSpread.y, maxBulletSpread.y);
        bulletSpread.z = Mathf.Clamp(bulletSpread.z + recoilAdjustment.z, minBulletSpread.z, maxBulletSpread.z);
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

            timeSinceLastShot = vibrationTime;
            GamePad.SetVibration(playerIndex, 1, 1);

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


