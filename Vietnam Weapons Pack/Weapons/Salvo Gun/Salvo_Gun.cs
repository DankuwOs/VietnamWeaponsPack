using System.Collections.Generic;
using UnityEngine;

public class SalvoGun : HPEquipGun
{
    private List<SalvoGun> salvoGuns = new List<SalvoGun>();

    public bool isVintage;

    public override void OnStartFire()
    {
        this.firing = true;
        FireSalvo();
    }
    
    public override void OnStopFire()
    {
        this.firing = false;
        foreach (var salvoGun in salvoGuns)
        {
            salvoGun.gun.SetFire(false);
        }
        base.OnStopFire();
    }

    public void FireSalvo()
    {
        this.salvoGuns.Clear();
        for (int i = 0; i < base.weaponManager.equipCount; i++)
        {
            HPEquippable equip = base.weaponManager.GetEquip(i);
            if (equip && equip is SalvoGun && equip.GetCount() > 0 && equip.shortName == this.shortName)
            {
                this.salvoGuns.Add((SalvoGun)equip);
            }
        }
        this.salvoGuns.Sort(delegate(SalvoGun a, SalvoGun b)
        {
            float num2 = Vector3.Dot(a.transform.position - base.weaponManager.transform.position, base.weaponManager.transform.right);
            float value = Vector3.Dot(b.transform.position - base.weaponManager.transform.position, base.weaponManager.transform.right);
            return num2.CompareTo(value);
        });
        
        for (int j = 0; j < this.salvoGuns.Count; j++)
        {
            if (!this.firing)
                return;
            this.salvoGuns[j].gun.SetFire(true);
        }
        
    }
    
    
}