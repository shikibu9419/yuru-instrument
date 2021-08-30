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

    public LocalUDPReceive udpReceive;
    public ConnectType connectType;
    public Navigator.Mode naviMode = Navigator.Mode.AVOID;

    private Vector[,] positions = new Vector[,] {
        { new Vector(200, 200), new Vector(200, 300), new Vector(300, 300), new Vector(300, 200) },
        { new Vector(200, 300), new Vector(300, 300), new Vector(300, 200), new Vector(200, 200) },
        { new Vector(300, 300), new Vector(300, 200), new Vector(200, 200), new Vector(200, 300) },
        { new Vector(300, 200), new Vector(200, 200), new Vector(200, 300), new Vector(300, 300) },
    };
    private Vector[] dPositions = new Vector[] {
        new Vector(-1, -1),
        new Vector(-1, 1),
        new Vector(1, 1),
        new Vector(1, -1),
    };
    private int nowMode = 0;

    async void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        cm = new CubeManager(connectType);
        await cm.MultiConnect(4);

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
            float scale = udpReceive.FormationScale;

            for (int i = 0; i < cm.navigators.Count; i++)
            {
                Vector dPos = dPositions[i] * scale * 500.0f;
                var navi = cm.navigators[i];
                navi.Navi2Target(positions[nowMode, i] + dPos, maxSpd: 115).Exec();
            }
        }
    }

    public void OnDoubleTap(Cube c)
    {
        if (c.isDoubleTap)
        {
            c.PlayPresetSound(0);
            nowMode = (nowMode + 1) % 4;
        }
        else
        {
            c.PlayPresetSound(1);
        }
    }
}
