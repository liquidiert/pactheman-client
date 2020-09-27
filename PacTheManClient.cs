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
            _graphics = new GraphicsDeviceManager(this);
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

            environment = Environment.Instance.Init(map);
            var start = DateTime.UtcNow;
            var path = AStar.Instance.GetPath(environment.MapAsTiles, new Vector2(1, 1), new Vector2(17, 1));
            Console.WriteLine(DateTime.UtcNow.Subtract(start).TotalSeconds);

            // actors
            player = new HumanPlayer(Content, environment, map);
            pinky = new Pinky(Content, environment, "pinky");
            blinky = new Blinky(Content, environment, "blinky");
            inky = new Inky(Content, environment, "inky");
            clyde = new Clyde(Content, environment, "clyde");

            actors.AddMany(player, pinky, blinky, inky, clyde);

            environment.PacMan = player;

            // camera
            var viewportadapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1216, 1408);
            camera = new OrthographicCamera(viewportadapter);

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
                    mapRenderer.Draw(camera.GetViewMatrix());
                    foreach (var actor in actors) {
                        actor.Draw(_spriteBatch);
                    }
                    break;
            }
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
