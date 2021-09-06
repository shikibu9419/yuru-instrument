using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using toio.MathUtils;
using Utils;
using UnityEngine;

namespace TactileMap {
    // 地図 = 全地点とそれらを結ぶ道の集合
    public class Map
    {
        public Landmark[] Landmarks { get; private set; }
        public LandmarkPath[] Paths { get; private set; }

        private Dictionary<int, Landmark> LandmarkById = new Dictionary<int, Landmark>();
        private List<int> LandmarkIds  = new List<int>();

        private static string YAML_PATH = "Assets/Data/";
        public static Map InitFromYaml() {
            StreamReader reader = new StreamReader(YAML_PATH + "map.yaml", System.Text.Encoding.UTF8);
            string text = reader.ReadToEnd();
            var input = new StringReader(text);
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            var map = deserializer.Deserialize<Map>(input);
            map.initialSetup();

            return map;
        }

        private void initialSetup()
        {
            foreach (var landmark in Landmarks)
            {
                LandmarkById[landmark.Id] = landmark;
                LandmarkIds.Add(landmark.Id);
            }

            // calculate path distance
            Paths = Paths.Select(path => {
                var fromPos = LandmarkById[path.From].Position;
                var toPos = LandmarkById[path.To].Position;
                path.Distance = Math.Sqrt((double) Math.Pow(fromPos.x - toPos.x, 2) + Math.Pow(fromPos.y - toPos.y, 2));

                return path;
            }).ToArray();
        }

        // Calculate route and retuern navigation using Dijkstra's Algorithm.
        public MapNavigation GetNavigation(int startId, int goalId)
        {
            // 地点のIDとそこまでの長さを短い順に保持
            var queue = new PriorityQueue<double, (int Id, double Length)>(t => t.Length, isDescending: false);
            // 各地点における, 最短路の距離(Distance)とkey IDをもつ地点の1つ前の地点(Id)
            var prevLandmark = new Dictionary<int, (int Id, double Distance)>();
            foreach (var id in LandmarkIds)
                prevLandmark[id] = (0, Double.PositiveInfinity);

            queue.Enqueue((startId, 0.0));
            prevLandmark[startId] = (0, 0.0);

            // startからgoalまでの最短路計算
            while(queue.Count > 0)
            {
                // nowId: 現在考えている地点, nowDistance: そこに至るまでの最短の長さ
                var (nowId, nowDistance) = queue.Dequeue();

                var availablePaths = Array.FindAll(Paths, path => path.From == nowId);
                foreach (var path in availablePaths)
                {
                    // 次の地点候補の距離を計算 (計算値が記録されている値より大きいなら飛ばす)
                    var toDistance = nowDistance + path.Distance;
                    if (prevLandmark[path.To].Distance <= toDistance)
                        continue;

                    // nextにおける最短路の長さを更新
                    prevLandmark[path.To] = (nowId, toDistance);
                    queue.Enqueue((path.To, toDistance));
                }
            }

            // 最短路routeを末尾から抽出していく
            var route = new List<int>();
            var landmarkId = goalId;
            while (landmarkId != 0)
            {
                route.Insert(0, landmarkId);
                landmarkId = prevLandmark[landmarkId].Id;
            }

            // routeの要素が1つ (到着点への経路がなかったとき) => 開始地点のみ返す
            if (route.Count == 1)
                route = new List<int>() { startId };

            return new MapNavigation(this, route.ToArray());
        }
    }
}
