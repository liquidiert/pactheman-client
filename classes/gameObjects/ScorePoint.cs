using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace pactheman_client {
    class ScorePoint {

        public static Texture2D Sprite = GameEnv.Instance.Content.Load<Texture2D>("sprites/objects/point");
        public Vector2 Position { get; set; }
        public RectangleF BoundingBox { get; set; }

        public ScorePoint(Vector2 position) {
            this.Position = position;
            this.BoundingBox = new RectangleF(Position, new Size2(64f, 64f));
        }

        public void Draw(SpriteBatch batch) {
            batch.Draw(Sprite, Position, color: Color.White);
        }
    }
}