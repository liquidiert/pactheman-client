using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Serialization;
using System;

namespace pactheman_client {

    class Clyde : Ghost {

        public Clyde(ContentManager content, string name) : base(content, "sprites/ghosts/spriteFactory.sf") {
            this.Sprite.Play("moving");
            this.Position = Environment.Instance.GhostStartPoints.Pop(new Random().Next(Environment.Instance.GhostStartPoints.Count)).Position;
            this.Name = name;
        }

        public override void Move(GameTime gameTime, GraphicsDeviceManager graphics) {
        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(Sprite, Position);
        }
    }
}