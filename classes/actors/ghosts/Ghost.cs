using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Serialization;
using System;

namespace pactheman_client {

    enum GhostState {
        Pursuing,
        Evading
    }

    class Ghost : Actor {

        private MovingStates CurrentMovingState {
            get { return movingState; } 
            set { movingState = value; }
        }
 
        public string Name;

        public Ghost(ContentManager content, Environment env, string name) {
            var ghostSprite = content.Load<SpriteSheet>("sprites/ghosts/spriteFactory.sf", new JsonContentLoader());
            this.Sprite = new AnimatedSprite(ghostSprite);
            this.Sprite.Play("moving");
            this.MovementSpeed = 400f;
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