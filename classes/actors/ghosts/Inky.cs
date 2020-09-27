using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Serialization;
using System;

namespace pactheman_client {

    class Inky : Ghost {

        public Inky(ContentManager content, Environment env, string name) {
            // HACK: MonoGame.Extended somehow can't read xnb files; thus always be sure the file is present in build dir!
            var ghostSprite = content.Load<SpriteSheet>("sprites/ghosts/spriteFactory.sf", new JsonContentLoader());
            this.Sprite = new AnimatedSprite(ghostSprite, "moving");
            this._environment = env;
            this.Position = env.GhostStartPoints.PopAt(new Random().Next(env.GhostStartPoints.Count)).Position;
            this.Name = name;
        }

        public override void Move(GameTime gameTime, GraphicsDeviceManager graphics) {
        }

        public override void Draw(SpriteBatch spriteBatch) {
            spriteBatch.Draw(this.Sprite, this._transform);
        }
    }
}