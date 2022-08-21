using UnityEngine;

public class MultiBarrelRotator : GunBarrelRotator
{
    public Transform rotationTransform2;
    
    private void Update()
    {
        rotationTransform2.rotation = rotationTransform.rotation;
    }
}