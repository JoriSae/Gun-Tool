using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    #region Variables
    [SerializeField] private AnimationCurve damageOverTime;
    [SerializeField] private float bulletDamage;
    [SerializeField] private float bulletCleanUpTime;
    private Rigidbody rigidBody;
    #endregion

    // Use this for initialization
    void Start()
    {
        //Destroy Bullet
        Destroy(gameObject, bulletCleanUpTime);
	}
}
