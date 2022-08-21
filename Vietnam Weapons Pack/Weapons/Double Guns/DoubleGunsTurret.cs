using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using VTOLVR.Multiplayer;

public class DoubleGunsTurret : HPEquipGunTurret
{
    public Gun gun2;
    
    private HUDWeaponInfo hudWeaponInfo = null;

    private string[] weaponMode = new string[]
    {
        "M134",
        "XM129",
        "Combined"
    };

    private GunMode WeaponMode;

    public enum GunMode
    {
        M134,
        XM129,
        Combined
    }

    protected override void OnEquip()
    {
        base.OnEquip();

        hudWeaponInfo = VTOLAPI.GetPlayersVehicleGameObject().GetComponentInChildren<HUDWeaponInfo>();

        HPEquippable.EquipFunction equipFunction = new EquipFunction();
        HPEquippable.EquipFunction equipFunction2 = equipFunction;

        equipFunction2.optionEvent = (HPEquippable.EquipFunction.OptionEvent) Delegate.Combine(equipFunction2.optionEvent, new HPEquippable.EquipFunction.OptionEvent(ToggleWeaponSelect));
        equipFunction.optionName = "WEAPON MODE";
        equipFunction.optionReturnLabel = weaponMode[(int) WeaponMode];

        HPEquippable.EquipFunction equipFunction3 = new EquipFunction();
        HPEquippable.EquipFunction equipFunction4 = equipFunction3;

        equipFunction3.optionName = "GIMBAL";
        equipFunction4.optionEvent = (HPEquippable.EquipFunction.OptionEvent) Delegate.Combine(equipFunction2.optionEvent, new HPEquippable.EquipFunction.OptionEvent(this.TurretFunction));
        equipFunction3.optionReturnLabel = (this.turretLocked ? "LOCKED" : "SLAVED");

        this.equipFunctions = new HPEquippable.EquipFunction[]
        {
            equipFunction,
            equipFunction3
        };

        WeaponMode = GunMode.M134;

        MeshRenderer mat1 = this.GetComponent<MeshRenderer>();
        MeshRenderer mat2 = this.transform.Find("TurretPitch").GetComponent<MeshRenderer>();
        
        var Livery = this.GetComponentInParent<AircraftLiveryApplicator>().materials[0].GetTexture("_Livery");

        var perBiome = weaponManager.GetComponent<PerBiomeLivery>();
        MapGenBiome.Biomes currBiome = MapGenBiome.Biomes.Boreal;
        if (VTMapGenerator.fetch)
        {
            currBiome = VTMapGenerator.fetch.biome;
        }
        foreach (var biome in perBiome.liveries)
        {
            if (biome.biome == currBiome)
            {
                Livery = biome.livery;
                break;
            }
        }
        
        
        mat1.material.SetTexture("_DetailAlbedoMap", Livery);
        Debug.Log("Mat1 " + Livery.name);
        mat1.material.EnableKeyword("_DETAIL_MULX2");
        mat2.material.SetTexture("_DetailAlbedoMap", Livery);
        mat2.material.EnableKeyword("_DETAIL_MULX2");
    }


    public override void OnStartFire()
    {
        this.firing = true;

        switch (WeaponMode)
        {
            case GunMode.Combined:
                gun.SetFire(true);
                gun2.SetFire(true);
                break;
            case GunMode.M134:
                gun.SetFire(true);
                break;
            case GunMode.XM129:
                gun2.SetFire(true);
                break;
        }
    }

    public override void OnStopFire()
    {
        this.firing = false;
        this.gun.SetFire(false);
        this.gun2.SetFire(false);
    }

    public override int GetMaxCount()
    {
        switch (WeaponMode)
        {
            case GunMode.M134:
                return gun.maxAmmo;
            case GunMode.XM129:
                return gun2.maxAmmo;
            case GunMode.Combined:
                return gun.maxAmmo + gun2.maxAmmo;
        }

        return gun.maxAmmo;
    }

    public override void Update()
    {
        if (this.mpRemote)
        {
            return;
        }

        Gun aimGun = (WeaponMode == GunMode.XM129 ? gun2 : gun);
        this.UpdateState();
        if (this.turretState != HPEquipGunTurret.TurretStates.Disabled)
        {
            if (!this.muvs || this.muvs.IsLocalWeaponController())
            {
                bool flag = this.targetingPage && this.targetingPage.tgpMode == TargetingMFDPage.TGPModes.PIP;
                bool flag2 = this.targetingPage && this.targetingPage.isSOI;
                if (this.turretState == HPEquipGunTurret.TurretStates.Locked ||
                    this.turretState == HPEquipGunTurret.TurretStates.SpeedLimit || flag)
                {
                    this.turret.AimToTarget(this.lockedTarget.position, true, true, true);
                    return;
                }

                if (base.weaponManager.opticalTargeter.locked && (!this.allowHeadtrackNoTGP || flag2))
                {
                    this.turret.AimToTarget(
                        aimGun.GetCalculatedTargetPosition(base.weaponManager.opticalTargeter.lockTransform.position,
                            base.weaponManager.opticalTargeter.targetVelocity), true, true, true);
                    return;
                }

                if (this.allowRadarTrack && this.radarUI && this.radarUI.currentLockedActor && this.radarUI.isSOI)
                {
                    this.turret.AimToTarget(
                        aimGun.GetCalculatedTargetPosition(this.radarUI.currentLockedActor.position,
                            this.radarUI.currentLockedActor.velocity), true, true, true);
                    return;
                }

                if (this.allowHeadtrackNoTGP && !this.targetingPage.isSOI)
                {
                    this.turret.AimToTarget(
                        aimGun.GetCalculatedTargetPosition(
                            VRHead.instance.transform.position + VRHead.instance.transform.forward * 1000f,
                            Vector3.zero), true, true, true);
                    return;
                }

                Ray ray = new Ray(base.weaponManager.opticalTargeter.cameraTransform.position,
                    base.weaponManager.opticalTargeter.cameraTransform.forward);
                RaycastHit raycastHit;
                if (Physics.Raycast(ray, out raycastHit, this.maxHeadtrackDist, 1, QueryTriggerInteraction.Ignore))
                {
                    this.turret.AimToTarget(aimGun.GetCalculatedTargetPosition(raycastHit.point, Vector3.zero), true,
                        true, true);
                    return;
                }

                this.turret.AimToTarget(
                    aimGun.GetCalculatedTargetPosition(ray.GetPoint(this.maxHeadtrackDist), Vector3.zero), true, true,
                    true);
                return;
            }
        }
        else
        {
            this.turret.AimToTarget(this.stowedTarget.position, true, true, true);
        }
    }

    public override int GetCount()
    {
        switch (WeaponMode)
        {
            case GunMode.M134:
                return gun.currentAmmo;
            case GunMode.XM129:
                return gun2.currentAmmo;
            case GunMode.Combined:
                return gun.currentAmmo + gun2.currentAmmo;
        }

        return gun.currentAmmo;
    }


    public string ToggleWeaponSelect()
    {
        switch (this.WeaponMode)
        {
            case GunMode.M134:
                this.WeaponMode = GunMode.XM129;
                if (hudWeaponInfo)
                    hudWeaponInfo.subLabelText.text = "XM129";
                break;
            case GunMode.XM129:
                this.WeaponMode = GunMode.Combined;
                if (hudWeaponInfo)
                    hudWeaponInfo.subLabelText.text = "Combined";
                break;
            case GunMode.Combined:
                this.WeaponMode = GunMode.M134;
                if (hudWeaponInfo)
                    hudWeaponInfo.subLabelText.text = "M134";
                break;
        }
        return this.weaponMode[(int)WeaponMode];
    }
}