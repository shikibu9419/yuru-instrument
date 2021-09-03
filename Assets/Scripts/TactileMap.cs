using System;
using System.IO;
using System.Linq;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace TactileMap
{
  public struct MapNavigation
  {
      public int[] Route;
      public bool Reached;
      public int NextLandmarkId { get { return Route[nextLandmarkIndex]; } }

      private int nextLandmarkIndex;

      public MapNavigation(int[] route)
      {
          Route = route;
          Reached = false;
          nextLandmarkIndex = route[0];
      }

      public void Next()
      {
          if (Route.Length == nextLandmarkIndex - 1)
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

  public struct LandmarkPath
  {
      public int From;
      public int To;

      public LandmarkPath(int from, int to)
      {
          From = from;
          To = to;
      }
  }

  public class Map
  {
      public Landmark[] Landmarks { get; set; }
      public LandmarkPath[] Paths { get; set; }

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

      public Landmark getLandmark(MapNavigation navi)
      {
          return Array.Find(Landmarks, landmark => landmark.Id == navi.NextLandmarkId);
      }
  }
}