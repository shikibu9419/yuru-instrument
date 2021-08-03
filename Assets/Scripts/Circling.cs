using UnityEngine;
using toio;
using toio.Navigation;
using toio.MathUtils;
using System.Collections;
using System.Collections.Generic;

public class Circling : MonoBehaviour {
    CubeManager cm;

    public Navigator.Mode naviMode = Navigator.Mode.BOIDS;
    private Vector[] centers = new Vector[] {
        new Vector(150, 150),
        new Vector(360, 150),
        new Vector(150, 360),
        new Vector(360, 360),
    };
    private int[] nextPositions = new int[] { 0, 0, 0, 0 };

    async void Start() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        cm = new CubeManager(ConnectType.Real);
        await cm.MultiConnect(4);

        foreach (var navi in cm.navigators) {
            navi.usePred = true;
            navi.mode = naviMode;
        }
    }

    void Update() {
        if (cm.synced) {
            for (int i = 0; i < cm.navigators.Count; i++) {
                var navi = cm.navigators[i];
                var mv = navi.Navi2Target(Vector.fromRadMag(Time.time / 1, 70) + centers[i % 4], maxSpd: 60, tolerance: 50).Exec();
            }
        }
    }
}
