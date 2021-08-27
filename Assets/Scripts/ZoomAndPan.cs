using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using toio;
using toio.Navigation;
using toio.MathUtils;

public class ZoomAndPan : MonoBehaviour {
    CubeManager cm;

    [SerializeField] private Dropdown dropdown;
    public ConnectType connectType;
    public Navigator.Mode naviMode = Navigator.Mode.BOIDS;
    private Vector[,] positions = new Vector[,] {
        { new Vector(10, 10), new Vector(10, 500), new Vector(500, 10), new Vector(500, 500), new Vector(200, 250), new Vector(300, 250) },
        { new Vector(150, 250), new Vector(250, 150), new Vector(500, 500), new Vector(500, 10), new Vector(10, 500), new Vector(10, 10) },
    };
    private int nowMode = 0;

    async void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        cm = new CubeManager(connectType);
        await cm.MultiConnect(6);

        for (int i = 0; i < cm.navigators.Count; i++)
        {
            var navi = cm.navigators[i];

            navi.usePred = true;
            navi.mode = naviMode;
            navi.cube.doubleTapCallback.AddListener("Cube" + i, OnDoubleTap);
        }
    }

    void Update()
    {
        if (cm.synced)
        {
            for (int i = 0; i < cm.navigators.Count; i++)
            {
                var navi = cm.navigators[i];
                navi.Navi2Target(positions[nowMode, i], maxSpd: 60).Exec();
            }
        }
    }

    public void OnDoubleTap(Cube c)
    {
        if (c.isDoubleTap)
        {
            c.PlayPresetSound(0);
            nowMode = (nowMode + 1) % 2;
        }
        else
        {
            c.PlayPresetSound(1);
        }
    }
}
