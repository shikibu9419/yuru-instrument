using UnityEngine;
using toio;
using toio.Navigation;
using toio.MathUtils;
using System;
using System.Collections;
using System.Collections.Generic;

public class Circling : MonoBehaviour {
    CubeManager cm;

    public Navigator.Mode naviMode = Navigator.Mode.BOIDS;
    private int[] nextPositions = new int[] { 0, 0, 0, 0 };
    private int circleCount = 4;

    async void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        cm = new CubeManager(ConnectType.Real);
        await cm.MultiConnect(4);

        foreach (var navi in cm.navigators)
        {
            navi.usePred = true;
            navi.mode = naviMode;
        }
    }

    void Update()
    {
        if (cm.synced)
        {
            for (int i = 0; i < cm.navigators.Count; i++)
            {
                var navi = cm.navigators[i];
                var mv = navi.Navi2Target(Vector.fromRadMag(Time.time / 1, getRadius()) + getCenterByIndex(i), maxSpd: 60, tolerance: 50).Exec();
            }
        }
    }

    private int getRadius()
    {
        return circleCount > 1 ? 70 : 160;
    }

    private Vector getCenterByIndex(int index)
    {
        switch (circleCount)
        {
            case 1:
                return new Vector(250, 250);
            case 2:
                return new Vector[] {
                    new Vector(250, 150),
                    new Vector(250, 360),
                }[index];
            case 4:
                return new Vector[] {
                    new Vector(150, 150),
                    new Vector(360, 150),
                    new Vector(150, 360),
                    new Vector(360, 360),
                }[index];
            default:
                return new Vector(250, 250);
        }
    }
}
