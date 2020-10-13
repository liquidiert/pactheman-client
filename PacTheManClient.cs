using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Linq;
using System.Collections.Generic;

namespace pactheman_client {
    public class PacTheManClient : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private OrthographicCamera _camera;

        // GUI
        private GuiSystem _guiSystem;
        private MainMenu _mainMenu = new MainMenu();

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

        public PacTheManClient() {
            _graphics = new GraphicsDeviceManager(this) { IsFullScreen = false };
            GameState.Instance.CurrentUIState = UIState.MainMenu;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            // Window.ClientSizeChanged += WindowOnClientSizeChanged;

            ContentTypeReaderManager.AddTypeCreator("Default", () => new JsonContentTypeReader<TexturePackerFile>());
        }

        private void WindowOnClientSizeChanged(object sender, EventArgs eventArgs) {
            _guiSystem.ClientSizeChanged();
        }

        protected override void Initialize() {
            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // _camera
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 2216, 1408);
            _camera = new OrthographicCamera(viewportAdapter);
            _camera.LookAt(new Vector2(608, 704));

            // menus
            var font = Content.Load<BitmapFont>("go");
            BitmapFont.UseKernings = false;
            Skin.CreateDefault(font);
            var guiRenderer = new GuiSpriteBatchRenderer(GraphicsDevice, viewportAdapter.GetScaleMatrix);
            _guiSystem = new GuiSystem(viewportAdapter, guiRenderer) { ActiveScreen = _mainMenu };

            // tile map
            map = Content.Load<TiledMap>("pactheman_map");
            mapRenderer = new TiledMapRenderer(GraphicsDevice, map);

            environment = Environment.Instance.Init(Content, map);

            // actors
            player = new HumanPlayer(Content, map);
            environment.PacMan = player; // ensure player is set before ghosts

            pinky = new Pinky(Content, "pinky");
            blinky = new Blinky(Content, "blinky");
            inky = new Inky(Content, "inky");
            clyde = new Clyde(Content, "clyde");

            actors.AddMany(player, blinky, pinky, inky, clyde);
            environment.Actors = actors;

            // add collisions
            environment.AddCollisions();

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(); // TODO: rather open menu

            switch (GameState.Instance.CurrentUIState) {
                case UIState.MainMenu:
                    _mainMenu.Update(gameTime);
                    break;
                case UIState.Game:
                    // update map
                    mapRenderer.Update(gameTime);

                    // update actors
                    foreach (var actor in actors) {
                        actor.Move(gameTime);
                        actor.Sprite.Update(gameTime);
                    }

                    // update collision pairs
                    foreach (var pair in GameState.Instance.CollisionPairs) {
                        pair.Update(gameTime);
                    }
                    break;
                case UIState.GameReset:
                    GameState.Instance.RESET_COUNTER -= gameTime.GetElapsedSeconds();
                    if (GameState.Instance.RESET_COUNTER <= 0) {
                        GameState.Instance.CurrentUIState = UIState.Game;
                        GameState.Instance.RESET_COUNTER = 4f;
                    }
                    break;

            }

            _guiSystem.Update(gameTime);
            environment.Walls.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            // map draw
            _spriteBatch.Begin(
                transformMatrix: _camera.GetViewMatrix(),
                samplerState: new SamplerState { Filter = TextureFilter.Point }
            );
            switch (GameState.Instance.CurrentUIState) {
                case UIState.MainMenu:
                    _guiSystem.Draw(gameTime);
                    break;
                case UIState.Settings:
                    break;
                case UIState.Game:
                    DrawEnvironment();
                    break;
                case UIState.GameReset:
                    DrawEnvironment();
                    _spriteBatch.DrawString(
                        Content.Load<SpriteFont>("ScoreFont"),
                        $"{(int)GameState.Instance.RESET_COUNTER}",
                        new Vector2(570, 650),
                        Color.Yellow,
                        0f,
                        Vector2.Zero,
                        3f,
                        SpriteEffects.None,
                        0f
                    );
                    break;
            }
            _spriteBatch.End();


            base.Draw(gameTime);
        }

        private void DrawEnvironment() {
            // draw map
            mapRenderer.Draw(_camera.GetViewMatrix());

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
        }
    }
}
