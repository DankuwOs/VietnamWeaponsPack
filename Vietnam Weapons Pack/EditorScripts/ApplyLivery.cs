using System;
using UnityEngine;

public class ApplyLivery : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            MeshRenderer mat1 = this.GetComponent<MeshRenderer>();
            MeshRenderer mat2 = this.transform.Find("TurretPitch").GetComponent<MeshRenderer>();

            var liveryTexture = this.GetComponentInParent<WeaponManager>().liverySample.material.GetTexture("_Livery");
        


            Debug.Log("Mat1");
            mat1.material.EnableKeyword("_DETAIL_MULX2");
            mat1.material.SetTexture("_DetailAlbedoMap", liveryTexture);
            Debug.Log("Canary_DoubleGuns: Is this the right livery? " + liveryTexture.name + " | Is enabled? " + mat1.material.IsKeywordEnabled("_DETAIL_MULX2"));
            Debug.Log("Mat2");
            mat2.material.EnableKeyword("_DETAIL_MULX2");
            mat2.material.SetTexture("_DetailAlbedoMap", liveryTexture);
        }
    }
}