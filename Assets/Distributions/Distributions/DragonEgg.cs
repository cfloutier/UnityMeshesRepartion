using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonEgg : Distribution
{
    public int NbPasCircle = 100;
    public float Width = 10;
    public AnimationCurve WidthCircles = new AnimationCurve();
    public AnimationCurve tiltCircles = new AnimationCurve();
    public AnimationCurve Normals = new AnimationCurve();

    public float stepCircleAngle = 0;
    public float Length = 10;
    public float tiltStep = 5;


    public Vector3 addedRotation = Vector3.zero;

    public float scaleItemRatio = 1;
    public float scaleItem = 1;

    /// <summary>
    /// ratio [0-1]
    /// </summary>
    /// <param name="ratio"></param>
    public void AddCircle(float ratio, float addedRot, Vector3 center, Quaternion tilt)
    {
        if (NbPasCircle <= 2)
            NbPasCircle = 2;
        float widthCircle = WidthCircles.Evaluate(ratio) * Width;
      //  float widthTangent = Mathf.Atan2(   WidthCircles.Evaluate(ratio) - WidthCircles.Evaluate(ratio+0.01f), 0.01f)*Mathf.Rad2Deg;

        float deltaAngle = 360f / NbPasCircle;
        for (int i = 0; i < NbPasCircle; i++)
        {
            float angleCircle = addedRot + deltaAngle * i;
            Quaternion rotationCircle = Quaternion.Euler(0, angleCircle, 0);
            Quaternion normal = Quaternion.Euler(Normals.Evaluate(ratio)*180, 0, 0) ;
            Vector3 pos = center + tilt * (rotationCircle * Vector3.forward * widthCircle) ;

            positions.Add(pos);

            rotations.Add( tilt* rotationCircle * normal * Quaternion.Euler(addedRotation));

            scales.Add(Vector3.one *(scaleItemRatio * widthCircle +  scaleItem));
        }
    }

    protected override void Compute()
    {
        if (Nb < 2)
            Nb = 2;

        InitLists();

        var tilt = 0f;
        var center = Vector3.zero;
        float addedRot = 0;
        for (float ratio = 0f; ratio < 1.0f; ratio += 1.0f / Nb)
        {
            var tiltIncrement = tiltStep * tiltCircles.Evaluate(ratio); 
           
            /*            var t = tiltCircles.Evaluate(ratio)*360;*/
            var tiltRotation = Quaternion.Euler(tilt , 0,0) ;
           
            var centerIncr = tiltRotation * Vector3.up * Length/Nb;
           
            AddCircle(ratio, addedRot, center, tiltRotation);
            addedRot += stepCircleAngle;
            tilt += tiltIncrement;
            center += centerIncr;
        }

        base.Compute();
    }


}
