using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Scr_Models;

public class scr_WeaponComtroller : MonoBehaviour
{
    private scr_characterControler characterControler;

    [Header("References")]
    public Animator weaponAnimator;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    [Header("Setting")]
    public WeaponSettingModel settings;

    bool isInitialised;

    Vector3 newWeaponRotation;
    Vector3 newWeaponRotationVelocity;

    Vector3 targetWeaponRotation;
    Vector3 targetWeaponRotationVelocity;

    Vector3 newWeaponMovementRotation;
    Vector3 newWeaponMovementRotationVelocity;

    Vector3 targetWeaponMovementRotation;
    Vector3 targetWeaponMovementRotationVelocity;

    private bool isGroundedTrigger;

    public float fallingDelay;

    [Header("Weapon Sways")]
    public Transform weaponSwayObject;
    public float swayAmountA = 1;
    public float swayAmountB = 2;
    public float swayScale = 600;
    public float swayLerpSpeed = 14;


    float swayTime;
    Vector3 swayPosition;

    [Header("Sights")]
    public Transform sightTarget;
    public float sightOffset;
    public float anmingInTime;
    private Vector3 weaponSwayPosition;
    private Vector3 weaponSwayPositionVelocity;
    [HideInInspector]
    public bool isAnimingIn;

    [Header("Shooting")]
    public float rateOffFire;
    private float currentFireRate;
    public List<WeaponFireType> allowedFireTypes;
    public WeaponFireType currentFireType;
    [HideInInspector]
    public bool isShooting;

    #region - Update / Start -

    private void Start()
    {
        newWeaponRotation = transform.localRotation.eulerAngles;
        currentFireType = allowedFireTypes.First();
    }

    

   

    private void Update()
    {
        if(!isInitialised)
        {
            return;
        }
        CalulateWeaponRotation();
        SetWeaponAnimations();
        CalulateWeaponSways();
        CalulateAinmingIn();
        CalculateShooting();

    }
    #endregion

    #region - Shooting -

    private void CalculateShooting()
    {
        if (isShooting)
        {
            Shoot();

            if(currentFireType == WeaponFireType.SemiAuto)
            {
                isShooting = false;
            }
        }
    }

    private void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawn);
    }

    #endregion

    #region - Initialise -

    public void Initialise(scr_characterControler CharacterControler)
    {
        characterControler = CharacterControler;
        isInitialised = true;
    }

    #endregion

    #region - AmingIn -

    private void CalulateAinmingIn()
    {
        var targetPosition = transform.position;

        if (isAnimingIn)
        {
            targetPosition = characterControler.camera.transform.position + (weaponSwayObject.transform.position - sightTarget.position) + (characterControler.camera.transform.forward * sightOffset);
        }

        weaponSwayPosition = weaponSwayObject.transform.position;
        weaponSwayPosition = Vector3.SmoothDamp(weaponSwayPosition, targetPosition, ref weaponSwayPositionVelocity, anmingInTime);
        weaponSwayObject.transform.position = weaponSwayPosition + swayPosition;

    }

    #endregion

    #region - Jumping -

    public void TriggerJump()
    {
        isGroundedTrigger = false;
        weaponAnimator.SetTrigger("Jump");
    }

    #endregion

    #region - Rotation -

    private void CalulateWeaponRotation()
    {

        targetWeaponRotation.y += (isAnimingIn ? settings.SwayAmount / 3 : settings.SwayAmount) * (settings.SwayXInverted ? -characterControler.input_View.x : characterControler.input_View.x) * Time.deltaTime;
        targetWeaponRotation.x += (isAnimingIn ? settings.SwayAmount / 3 : settings.SwayAmount) * (settings.SwayYInverted ? characterControler.input_View.y : -characterControler.input_View.y) * Time.deltaTime;

        targetWeaponRotation.x = Mathf.Clamp(targetWeaponRotation.x, -settings.SwayClampX, settings.SwayClampX);
        targetWeaponRotation.y = Mathf.Clamp(targetWeaponRotation.y, -settings.SwayClampX, settings.SwayClampY);
        targetWeaponRotation.z = isAnimingIn ? 0 : targetWeaponRotation.y;

        targetWeaponRotation = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, settings.SwayResetSmooting);
        newWeaponRotation = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, settings.SwaySmoothing);

        targetWeaponMovementRotation.z = (isAnimingIn ? settings.MovementSwayX / 3 : settings.MovementSwayY) * (settings.MovementSwayXInverted ? -characterControler.input_Movement.x : characterControler.input_Movement.x);
        targetWeaponMovementRotation.x = (isAnimingIn ? settings.MovementSwayY / 3 : settings.MovementSwayY) * (settings.MovementSwayYInverted ? -characterControler.input_Movement.y : characterControler.input_Movement.y);

        targetWeaponMovementRotation = Vector3.SmoothDamp(targetWeaponMovementRotation, Vector3.zero, ref targetWeaponMovementRotationVelocity, settings.MovementSwaySmoothing);
        newWeaponMovementRotation = Vector3.SmoothDamp(newWeaponMovementRotation, targetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, settings.MovementSwaySmoothing);

        transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);
    }

    #endregion

    #region - Animation -

    private void SetWeaponAnimations()
    {

        if (isGroundedTrigger)
        {
            fallingDelay = 0;
        }
        else
        {
            fallingDelay += Time.deltaTime;
        }

        if (characterControler.isGrounded && !isGroundedTrigger && fallingDelay > 0.1f)
        {
            Debug.Log("Trigger Land");
            weaponAnimator.SetTrigger("Land");
            isGroundedTrigger = true;
        }
        if (!characterControler.isGrounded && isGroundedTrigger)
        {
            Debug.Log("Trigger Falling");
            weaponAnimator.SetTrigger("Falling");
            isGroundedTrigger = false;
        }


        weaponAnimator.SetBool("IsSprinting", characterControler.isSprinting);
        weaponAnimator.SetFloat("WeaponAnimationSpeed", characterControler.weaponAnimationSpeed);
    }

    #endregion

    #region - Sway -

    private void CalulateWeaponSways()
    {
        var targetPosition = LissajousCurve(swayTime, swayAmountA, swayAmountB) / (isAnimingIn ? swayScale * 3 : swayScale);

        swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime * swayLerpSpeed);
        swayTime += Time.deltaTime;

        if (swayTime > 6.3f)
        {
            swayTime = 0;
        }
    }

    private Vector3 LissajousCurve(float Time, float A, float B)
    {
        return new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
    }

    #endregion

}
