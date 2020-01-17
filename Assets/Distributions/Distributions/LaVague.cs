using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaVague : Distribution
{
    public int NbPasCircle = 100;
    public float Width = 10;
    public AnimationCurve WidthCircles;
    public AnimationCurve tiltCircles;
    public float Length = 10;
    public float tiltStep = 5;

    /// <summary>
    /// ratio [0-1]
    /// </summary>
    /// <param name="ratio"></param>
    public void AddCircle(float ratio, Vector3 center, Quaternion tilt)
    {
        if (NbPasCircle <= 2)
            NbPasCircle = 2;
        float widthCircle = WidthCircles.Evaluate(ratio) * Width;
        
        float deltaAngle = 360f / NbPasCircle;
        for (int i = 0; i < NbPasCircle; i++)
        {
            float angle = deltaAngle * i;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 pos = center + tilt * (rotation * Vector3.forward * widthCircle) ;

            positions.Add(pos);
            rotations.Add(tilt*rotation);
        }
    }


    protected override void Compute()
    {
        if (Nb < 10)
            Nb = 10;

        InitLists();
        var tilt = 0f;
        var center = Vector3.zero;
        for (float ratio = 0f; ratio < 1.0f; ratio += 1.0f / Nb)
        {
            var tiltIncrement = tiltStep * tiltCircles.Evaluate(ratio); 
            tilt += tiltIncrement;
            /*            var t = tiltCircles.Evaluate(ratio)*360;*/
            var tiltRotation = Quaternion.Euler(tilt , 0,0) ;
           
            var centerIncr = tiltRotation * Vector3.up * Length/Nb;
            center += centerIncr;
            AddCircle(ratio, center, tiltRotation);
        }



        base.Compute();
    }


}
