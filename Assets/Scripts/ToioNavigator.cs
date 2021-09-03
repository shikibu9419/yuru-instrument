using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using toio;
using toio.Navigation;
using toio.MathUtils;
using TactileMap;

public class ToioNavigator : MonoBehaviour {
    CubeManager cm;

    public ConnectType connectType;
    public Navigator.Mode naviMode = Navigator.Mode.BOIDS;

    private Map map;
    private MapNavigation navigation;

    async void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        map = Map.InitFromYaml();
        navigation = new MapNavigation(new int[] { 1, 2, 4 });

        cm = new CubeManager(connectType);
        await cm.SingleConnect();

        foreach (var navi in cm.navigators)
        {
            navi.usePred = true;
        }
    }

    void Update()
    {
        if (!cm.synced)
            return;

        foreach (var navi in cm.navigators)
        {
            var pos = map.GetLandmark(navigation).GetPosition();
            var mv = navi.Navi2Target(pos).Exec();
            if (mv.reached)
                navigation.Next();
        }
    }
}
