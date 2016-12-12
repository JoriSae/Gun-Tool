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
    private float canFire;
    private float lateReloadKey;
    private float timeSinceLastShot;
    #endregion

    // Use this for initialization
    void Start()
    {
        gunScript = gun.GetComponent<Gun>();

        FindGamePad();

        gunScript.fireEvent.AddListener(Vibrate);
    }
	
	// Update is called once per frame
	void Update()
    {
        CalculateFiringState();
        UpdateTimers();
        UpdateInput();
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
        if (gunScript.fireState == Gun.FireState.Automatic)
        {
            canFire = 0;
        }
    }

    void UpdateInput()
    {
        //Check Input
        if (Input.GetAxis("Reload") > 0 && lateReloadKey == 0)
        {
            StartCoroutine(gunScript.AttemptReload());
        }

        if (canFire == 0 && Input.GetAxis("Fire") > 0)
        {
            gunScript.AttemptFireProjectile();
        }
    }

    void LateUpdate()
    {
        //Check if button is still pressed
        canFire = Input.GetAxis("Fire");
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

    public void Vibrate()
    {
        //Update vibration timer and set vibration
        timeSinceLastShot = vibrationTime;
        GamePad.SetVibration(playerIndex, vibrationIntensity.x, vibrationIntensity.y);
    }
}
