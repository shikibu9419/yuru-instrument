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
    public Navigator.Mode naviMode = Navigator.Mode.AVOID;

    private Map map;
    private MapNavigation navigation;
    private Dictionary<string, Landmark> landmarkByCube = new Dictionary<string, Landmark>();
    private int N = 1;

    async void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        map = Map.InitFromYaml();
        navigation = map.GetNavigation(1, 1);

        cm = new CubeManager(connectType);
        await cm.MultiConnect(N);

        for (var i = 0; i < N; i++)
        {
            var cubeNavi = cm.navigators[i];
            cubeNavi.usePred = true;
            landmarkByCube[cubeNavi.cube.id] = map.Landmarks[i];

            cubeNavi.cube.doubleTapCallback.AddListener("EventScene", OnDoubleTap);
        }
    }

    void Update()
    {
        if (!cm.synced)
            return;

        if (navigation.Reached) {
            updateNavigation();
            return;
        }

        for (var i = 0; i < N; i++)
        {
            var cubeNavi = cm.navigators[i];
            // TODO: Refactor
            // navigation.GoToNextLandmark();
            if (i == 0)
            {
                var pos = navigation.NextLandmark.Position;
                var mv = cubeNavi.Navi2Target(pos).Exec();
                if (mv.reached)
                    navigation.Next();
            } else
            {
                var pos = landmarkByCube[cubeNavi.cube.id].Position;
                cubeNavi.Navi2Target(pos).Exec();
            }
            if (cubeNavi.cube.isDoubleTap) {
                Debug.Log("fugapiyo");
                OnDoubleTap(cubeNavi.cube);
            }
        }
    }

    private void updateNavigation()
    {
        var destination = dropdown.value + 1;
        if (navigation.Destination != destination)
            navigation = map.GetNavigation(navigation.Destination, destination);
    }

    public void OnDoubleTap(Cube cube)
    {
        Debug.Log("hogefuga");
        cube.PlayPresetSound(2);
    }
}
