using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using toio;
using toio.Navigation;
using toio.MathUtils;

public class CirclingByGesture : MonoBehaviour {
    CubeManager cm;

    [SerializeField] private Dropdown dropdown;
    public ConnectType connectType;
    public Navigator.Mode naviMode = Navigator.Mode.BOIDS;
    private int circleCount = 1;
    private int[] nextPositions = new int[] { 0, 0, 0, 0 };

    async void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        cm = new CubeManager(connectType);
        await cm.MultiConnect(6);

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
                }[index % 2];
            case 4:
                return new Vector[] {
                    new Vector(150, 150),
                    new Vector(360, 150),
                    new Vector(150, 360),
                    new Vector(360, 360),
                }[index % 4];
            default:
                return new Vector(250, 250);
        }
    }

    public void onUpdateDropdown()
    {
        switch (dropdown.value)
        {
            case 0:
                this.circleCount = 1;
                break;
            case 1:
                this.circleCount = 2;
                break;
            case 2:
                this.circleCount = 4;
                break;
            default:
                this.circleCount = 1;
                break;
        }
    }
}
