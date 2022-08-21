using UnityEngine;

public class HPEquipGunTurretGaming : HPEquipGunTurret
{
    protected override void OnEquip()
    {
        base.OnEquip();
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
}