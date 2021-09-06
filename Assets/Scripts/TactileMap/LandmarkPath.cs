namespace TactileMap {
    // 地点同士を結ぶ道
    public struct LandmarkPath
    {
        public int From { get; set; }
        public int To   { get; set; }
        public double Distance { get; set; }

        public LandmarkPath(int from, int to)
        {
            From = from;
            To = to;
            // 特に指定がないなら距離は単一とする
            Distance = 1.0;
        }
    }
}
