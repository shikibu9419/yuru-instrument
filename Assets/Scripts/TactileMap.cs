using System;
using System.IO;
using System.Linq;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace TactileMap
{
  public struct Route
  {
      public int[] Path;
      public bool Reached;
      public int NextLandmarkId { get { return Path[nextLandmarkIndex]; } }

      private int nextLandmarkIndex;

      public Route(int[] path)
      {
          Path = path;
          Reached = false;
          nextLandmarkIndex = path[0];
      }

      public void Next()
      {
          if (Path.Length == nextLandmarkIndex - 1)
              Reached = true;
          else
              nextLandmarkIndex++;
      }
  }

  public struct Landmark
  {
      public int Id { get; set; }
      public string Name { get; set; }
      public int X { get; set; }
      public int Y { get; set; }

      public Landmark(int id, int x, int y, string name)
      {
          Id = id;
          Name = name;
          X = x;
          Y = y;
      }

      public UnityEngine.Vector2 GetPosition() => new UnityEngine.Vector2(X, Y);
  }

  public class Map
  {
      public Landmark[] Landmarks { get; set; }
      public (int, int)[] AvailablePaths { get; set; }

      private static string YAML_PATH = "Assets/Data/";
      public static Map fromYaml() {
          StreamReader reader = new StreamReader(YAML_PATH + "map.yaml", System.Text.Encoding.UTF8);
          string text = reader.ReadToEnd();
          var input = new StringReader(text);
          var deserializer = new DeserializerBuilder()
              .WithNamingConvention(new CamelCaseNamingConvention())
              .Build();

          return deserializer.Deserialize<Map>(input);
      }

      public Landmark getLandmark(Route route)
      {
          return Array.Find(Landmarks, landmark => landmark.Id == route.NextLandmarkId);
      }
  }
}