using System;
using System.Linq;
using toio.MathUtils;

namespace TactileMap {
    // 地図上のある経路のナビゲーション
    public class MapNavigation
    {
        public int[] Route          { get; private set; }
        public bool  Reached        { get; private set; }

        public int Origin         { get => Route[0]; }
        public int Destination    { get => Route[Route.Length - 1]; }
        public int NextLandmarkId { get => Route[nextRouteIndex]; }
        public Landmark NextLandmark
        {
            get {
                int nextLandmarkId = this.NextLandmarkId;
                return Array.Find(map.Landmarks, landmark => landmark.Id == nextLandmarkId);
            }
        }

        private Map map;
        private int nextRouteIndex = 0;

        public MapNavigation(Map map, int[] route)
        {
            this.map = map;
            Route = route;
            Reached = false;
        }

        public void Next()
        {
            if (Route.Length == nextRouteIndex + 1)
                Reached = true;
            else
                nextRouteIndex++;
        }
    }
}
