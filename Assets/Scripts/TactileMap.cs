using System;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using toio.MathUtils;

namespace TactileMap
{
    // 地図における各地点 (建物, 曲がり角etc.)
    public struct Landmark
    {
        public int Id { get; set; }
        public int X  { get; set; }
        public int Y  { get; set; }
        public string Name { get; set; }

        public Vector Position { get => new Vector(X, Y); }

        public Landmark(int id, int x, int y, string name)
        {
            Id = id;
            Name = name;
            X = x;
            Y = y;
        }
    }

    // 地点同士を結ぶ道
    public struct LandmarkPath
    {
        public int From { get; set; }
        public int To   { get; set; }

        public LandmarkPath(int from, int to)
        {
            From = from;
            To = to;
        }
    }

    // 地図 = 全地点とそれらを結ぶ道の集合
    public class Map
    {
        public Landmark[] Landmarks { get; set; }
        public LandmarkPath[] Paths { get; set; }

        private static string YAML_PATH = "Assets/Data/";
        public static Map InitFromYaml() {
            StreamReader reader = new StreamReader(YAML_PATH + "map.yaml", System.Text.Encoding.UTF8);
            string text = reader.ReadToEnd();
            var input = new StringReader(text);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            return deserializer.Deserialize<Map>(input);
        }
    }

    // 地図上のある経路のナビゲーション
    public struct MapNavigation
    {
        // 経路 (地点IDの配列で表現)
        public int[] Route { get; set; }
        public bool  Reached { get; set; }
        public int   NextLandmarkId { get => Route[nextRouteIndex]; }
        public Landmark NextLandmark
        {
            get {
                int nextLandmarkId = this.NextLandmarkId;
                return Array.Find(map.Landmarks, landmark => landmark.Id == nextLandmarkId);
            }
        }

        private Map map;
        private int nextRouteIndex;

        public MapNavigation(Map map, int[] route)
        {
            this.map = map;
            Route = route;
            Reached = false;
            nextRouteIndex = 0;
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