using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using toio;
using toio.Navigation;
using toio.MathUtils;
using TactileMap;

public class Navigation : MonoBehaviour {
    CubeManager cm;

    public ConnectType connectType;
    public Navigator.Mode naviMode = Navigator.Mode.BOIDS;

    private Map map;
    private Route route;
    private int nextLandmark;

    async void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        map = Map.fromYaml();
        route = new Route(new int[] { 1, 2, 4 });
        nextLandmark = 1;

        cm = new CubeManager(connectType);
        await cm.MultiConnect(1);

        foreach (var navi in cm.navigators)
        {
            navi.usePred = true;
            navi.mode = naviMode;
        }
    }

    void Update()
    {
        if (!cm.synced)
            return;

        var navi = cm.navigators[0];
        var mv = navi.Navi2Target(map.getLandmark(route).GetPosition(), maxSpd: 30).Exec();
        if (mv.reached)
            route.Next();
    }
}
