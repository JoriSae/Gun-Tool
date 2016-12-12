using UnityEngine;
using XInputDotNetPure;
using System.Collections;

public class TempController : MonoBehaviour
{
    #region Controller Variables
    private bool playerIndexSet = false;

    private PlayerIndex playerIndex;
    private GamePadState state;
    private GamePadState prevState;

    public GameObject gun;
    private Gun gunScript;

    public Vector2 vibrationIntensity;

    public float vibrationTime;
    private float previousState;
    private float lateReloadKey;
    private float timeSinceLastShot;
    #endregion

    // Use this for initialization
    void Start()
    {
        gunScript = gun.GetComponent<Gun>();
	}
	
	// Update is called once per frame
	void Update()
    {
        UpdateTimers();
        UpdateInput();
        CalculateFiringState();
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
        //Update timer
        timeSinceLastShot -= Time.deltaTime;
    }

    void CalculateFiringState()
    {
        //Check fire state and fire accordingly
        switch (gunScript.fireState)
        {
            case Gun.FireState.Automatic:
                if (Input.GetAxis("Fire") > 0)
                {
                    gunScript.AttemptFireProjectile();
                }
                break;
            case Gun.FireState.SemiAutomatic:
                if (previousState == 0 && Input.GetAxis("Fire") > 0)
                {
                    gunScript.AttemptFireProjectile();
                }
                break;
        }
    }

    void UpdateInput()
    {
        if (Input.GetAxis("Reload") > 0 && gunScript.ammo != gunScript.clipSize && lateReloadKey == 0 && gunScript.clipAmount > 0 && gunScript.canShoot)
        {
            gunScript.canShoot = false;
            gunScript.Invoke("Reload", gunScript.reloadTime);
        }
    }

    void LateUpdate()
    {
        //Check if button is still pressed
        previousState = Input.GetAxis("Fire");
        lateReloadKey = Input.GetAxis("Reload");
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
