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

    [SerializeField] private Dropdown dropdown;
    public ConnectType connectType;
    public Navigator.Mode naviMode = Navigator.Mode.BOIDS;

    private Map map;
    private MapNavigation navigation;

    async void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        map = Map.InitFromYaml();
        navigation = map.GetNavigation(1, 1);

        cm = new CubeManager(connectType);
        await cm.SingleConnect();

        foreach (var cubeNavi in cm.navigators)
            cubeNavi.usePred = true;
    }

    void Update()
    {
        if (!cm.synced)
            return;

        if (navigation.Reached) {
            updateNavigation();
            return;
        }

        foreach (var cubeNavi in cm.navigators)
        {
            var pos = navigation.NextLandmark.Position;
            var mv = cubeNavi.Navi2Target(pos).Exec();
            if (mv.reached)
                navigation.Next();
        }
    }

    private void updateNavigation()
    {
        var destination = dropdown.value + 1;
        if (navigation.Destination != destination)
            navigation = map.GetNavigation(navigation.Destination, destination);
    }
}
