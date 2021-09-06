using toio.MathUtils;

namespace TactileMap {
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
}
