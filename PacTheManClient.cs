using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;

namespace pactheman_client
{
    public class PacTheManClient : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private OrthographicCamera camera;

        // environment
        private TiledMap map;
        private TiledMapRenderer mapRenderer;
        private Environment environment;

        // characters
        private HumanPlayer player;
        private Ghost pinky;
        private Ghost blinky;
        private Ghost inky;
        private Ghost clyde;

        private List<Actor> actors = new List<Actor>();

        public PacTheManClient()
        {
            _graphics = new GraphicsDeviceManager(this) { IsFullScreen = false };
            GameState.CurrentState = UIState.Game;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            
            ContentTypeReaderManager.AddTypeCreator("Default", () => new JsonContentTypeReader<TexturePackerFile>());
        }

        protected override void Initialize()
        {
            base.Initialize();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // tile map
            map = Content.Load<TiledMap>("pactheman_map");
            mapRenderer = new TiledMapRenderer(GraphicsDevice, map);

            environment = Environment.Instance.Init(Content, map);

            // actors
            player = new HumanPlayer(Content, map);
            environment.PacMan = player; // ensure player is set before ghosts

            //pinky = new Pinky(Content, "pinky");
            blinky = new Blinky(Content, "blinky");
            /* inky = new Inky(Content, "inky");
            clyde = new Clyde(Content, "clyde"); */

            actors.AddMany(player, blinky);

            foreach (var actor in actors) {
                environment.Walls.CreateActor(actor);
            }

            // camera
            var viewportadapter = new BoxingViewportAdapter(Window, GraphicsDevice, 2216, 1408);
            camera = new OrthographicCamera(viewportadapter);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(); // TODO: rather open menu

            if (GameState.CurrentState == UIState.Game)
            {
                // update map
                mapRenderer.Update(gameTime);
                camera.LookAt(new Vector2(608, 704));

                // update actors
                foreach (var actor in actors) {
                    actor.Move(gameTime, _graphics);
                    actor.Sprite.Update(gameTime);
                }

            }

            environment.Walls.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // map draw
            _spriteBatch.Begin(
                transformMatrix: camera.GetViewMatrix(),
                samplerState: new SamplerState { Filter = TextureFilter.Point }
            );
            switch (GameState.CurrentState) {
                case UIState.Menu:
                    break;
                case UIState.Settings:
                    break;
                case UIState.Game:
                    // draw map
                    mapRenderer.Draw(camera.GetViewMatrix());

                    // draw score points
                    foreach (var point in environment.ScorePointPositions) {
                        point.Draw(_spriteBatch);
                    }

                    // player one stats
                    // draw name
                    _spriteBatch.DrawString(
                        Content.Load<SpriteFont>("ScoreFont"),
                        player.Name,
                        new Vector2(-350, 50),
                        Color.White
                    );
                    // draw lives
                    _spriteBatch.DrawString(
                        Content.Load<SpriteFont>("ScoreFont"),
                        $"Lives: {player.Lives}",
                        new Vector2(-350, 100),
                        Color.White
                    );
                    // draw score
                    _spriteBatch.DrawString(
                        Content.Load<SpriteFont>("ScoreFont"),
                        $"Score: {player.Score}",
                        new Vector2(-350, 150),
                        Color.White
                    );

                    // draw actors
                    foreach (var actor in actors) {
                        actor.Draw(_spriteBatch);
                    }
                    /* Texture2D _texture;

                    _texture = new Texture2D(GraphicsDevice, 1, 1);
                    _texture.SetData(new Color[] { Color.DarkSlateGray });
                    _spriteBatch.Draw(_texture, 
                        new Rectangle((int) blinky.BoundingBox.X, (int) blinky.BoundingBox.Y, (int) blinky.BoundingBox.Width, (int) blinky.BoundingBox.Height),
                        Color.White); */
                    break;
            }
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
