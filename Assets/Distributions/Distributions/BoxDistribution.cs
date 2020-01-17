using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple random distribution depending on a radius (distance)
/// </summary>
public class BoxDistribution : ExtendedDistribution
{
    public enum Mode
    {
        AutoGrid,
        CustomGrid,
        Random
    }

    public Vector3 BoxSize;

    bool showSizes() { return mode == Mode.CustomGrid;  }

    public Mode mode;

    [Visible("showSizes()")]
    public int SizeX = 10;
    [Visible("showSizes()")]
    public int SizeY = 10;
    [Visible("showSizes()")]
    public int SizeZ = 10;

    void buildGrid()
    {
        if (SizeX < 1) SizeX = 1;
        if (SizeY < 1) SizeY = 1;
        if (SizeZ < 1) SizeZ = 1;


        float deltaX = SizeX  > 1 ? BoxSize.x / (SizeX - 1) : 0;
        float posX = SizeX > 1 ? - BoxSize.x / 2 : 0;

        float deltaY = SizeY > 1 ? BoxSize.y / (SizeY - 1) : 0;
        float posY = SizeY > 1 ? -BoxSize.y / 2 : 0;

        float deltaZ = SizeZ > 1 ? BoxSize.z / (SizeZ - 1) : 0;
        float posZ = SizeZ > 1 ? -BoxSize.z / 2 : 0;

        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                for (int z = 0; z < SizeZ; z++)
                {
                    positions.Add(new Vector3(
                        posX + deltaX * x,
                        posY + deltaY * y,
                        posZ + deltaZ * z));
                }
            }
        }
    }


    protected override void Compute()
    {
        Random.InitState(RandomSeed + 500);
        InitLists();

        switch (mode)
        {
            case Mode.CustomGrid:
                {
                    Nb = SizeX * SizeY * SizeZ;
                    buildGrid();
                }
                break;

            case Mode.AutoGrid:
                {
                    int nbLines = (int)Mathf.Pow(Nb, 0.33333f);
                    SizeX = SizeY = SizeZ = nbLines;

                    buildGrid();


                }

                break;
            case Mode.Random:
                for (int i = 0; i < Nb; i++)
                {
                    positions.Add(new Vector3(
                         Random.Range(-BoxSize.x / 2, BoxSize.x / 2),
                         Random.Range(-BoxSize.y / 2, BoxSize.y / 2),
                         Random.Range(-BoxSize.z / 2, BoxSize.z / 2)));
                }
                break;
            default:
                break;
        }

        base.Compute();
    }
}
