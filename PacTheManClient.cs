using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;

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

        public PacTheManClient()
        {
            _graphics = new GraphicsDeviceManager(this);
            GameState.CurrentState = UIState.Menu;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
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

            // camera
            var viewportadapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1984, 2240);
            camera = new OrthographicCamera(viewportadapter);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (GameState.CurrentState == UIState.Game)
            {
                mapRenderer.Update(gameTime);
                camera.LookAt(new Vector2(0, 0));
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
            mapRenderer.Draw(camera.GetViewMatrix());
            _spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
