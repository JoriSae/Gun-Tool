using UnityEngine;
using UnityEditor;

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
            script.ammoPool = EditorGUILayout.FloatField("Ammo Pool", script.ammoPool);
            script.clipSize = EditorGUILayout.FloatField("Clip Size", script.clipSize);
            script.reloadTime = EditorGUILayout.FloatField("Reload Time", script.reloadTime);
        }
        EditorGUILayout.EndToggleGroup();
        #endregion
    }
}