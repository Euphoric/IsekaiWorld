using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace IsekaiWorld
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Vector2 _cameraCenter = new Vector2(64);
        private float _tilePixels = 16;

        private Point _mapSize = new Point(128, 128);
        Texture2D _mapTexture;

        private Texture2D _characterTexture;
        private Point _characterPosition = new Point(16, 16);

        private SpriteFont _fontArial14;
        private Matrix2 _mapViewMatrix;
        private Matrix2 _characterWorldMatrix;
        private RectangleF _mapViewBoundingBox;
        private RectangleF _characterViewBoundingBox;
        private string _hoverText;
        private Point? _mouseOverMapCoordinates;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _fontArial14 = Content.Load<SpriteFont>("arial_14");

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a 1px square rectangle texture that will be scaled to the
            // desired size and tinted the desired color at draw time
            _mapTexture = new Texture2D(GraphicsDevice, _mapSize.X, _mapSize.Y);
            Color[] colorTileMapping = new[]
            {
                Color.LightGreen,
                Color.SandyBrown,
                Color.Aqua,
                Color.DarkGray
            };
            var map = MapGeneration.GenerateMap(_mapSize.X, _mapSize.Y);
            List<Color> groundColor = new List<Color>();
            for (int y = 0; y < _mapSize.Y; y++)
            {
                for (int x = 0; x < _mapSize.X; x++)
                {
                    if (x >= map.GetLength(0) || y >= map.GetLength(1))
                    {
                        groundColor.Add(Color.Gray);
                    }
                    else
                    {
                        var color = colorTileMapping[map[x, y]];
                        groundColor.Add(color);
                    }
                }
            }

            _mapTexture.SetData(groundColor.ToArray());

            _characterTexture = new Texture2D(GraphicsDevice, 1, 1);
            _characterTexture.SetData(new[] { Color.White });

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            _spriteBatch.Dispose();
            // If you are creating your texture (instead of loading it with
            // Content.Load) then you must Dispose of it
            _mapTexture.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            UpdateMapPosition(gameTime);

            base.Update(gameTime);
        }

        private void UpdateMapPosition(GameTime gameTime)
        {
            UpdateCameraFromInput(gameTime);

            // animate character
            _characterPosition = new Point(13) + new Point((int)(gameTime.TotalGameTime.TotalSeconds * 1));
            
            UpdateMatrices();
            UpdateBoundingBoxes();

            string hoverText = "NONE";
            _mouseOverMapCoordinates = null;
            var mouseState = Mouse.GetState();
            if (_mapViewBoundingBox.Contains(mouseState.Position))
            {
                var mapCoordinates = Matrix2.Invert(_mapViewMatrix).Transform(mouseState.Position.ToVector2());
                _mouseOverMapCoordinates = new Point((int)mapCoordinates.X, (int)mapCoordinates.Y);
                
                if (_characterViewBoundingBox.Contains(mouseState.Position))
                {
                    hoverText = "Character";
                }
                else
                {
                    hoverText = $"Map [{mapCoordinates.X},{mapCoordinates.Y}]";
                }
            }

            _hoverText = hoverText;
        }

        private void UpdateCameraFromInput(GameTime gameTime)
        {
            var cameraMoveSpeed = gameTime.ElapsedGameTime.Milliseconds * 0.05f;
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.A))
                _cameraCenter += -Vector2.UnitX * cameraMoveSpeed;
            if (keyboardState.IsKeyDown(Keys.D))
                _cameraCenter += Vector2.UnitX * cameraMoveSpeed;
            if (keyboardState.IsKeyDown(Keys.W))
                _cameraCenter += -Vector2.UnitY * cameraMoveSpeed;
            if (keyboardState.IsKeyDown(Keys.S))
                _cameraCenter += Vector2.UnitY * cameraMoveSpeed;

            _cameraCenter = Vector2.Max(Vector2.Zero, Vector2.Min(_cameraCenter, _mapSize.ToVector2()));

            if (keyboardState.IsKeyDown(Keys.E))
                _tilePixels += gameTime.ElapsedGameTime.Milliseconds * 0.05f;

            if (keyboardState.IsKeyDown(Keys.Q))
                _tilePixels += -gameTime.ElapsedGameTime.Milliseconds * 0.05f;

            _tilePixels = MathF.Max(4, MathF.Min(_tilePixels, 64f));
        }

        private void UpdateMatrices()
        {
            var mapPosition = (Vector2.Zero - _cameraCenter) * _tilePixels +
                              GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2;
            _mapViewMatrix = Matrix2.CreateScale(_tilePixels) * Matrix2.CreateRotationZ(0) *
                         Matrix2.CreateTranslation(mapPosition);
            
            var characterTranslation = _characterPosition.ToVector2();
            var characterLocalMatrix =  Matrix2.CreateRotationZ(0) *
                                        Matrix2.CreateTranslation(characterTranslation);
            _characterWorldMatrix = characterLocalMatrix * _mapViewMatrix;
        }

        private void UpdateBoundingBoxes()
        {
            _mapViewBoundingBox = TransformRectangle(_mapTexture.Bounds, _mapViewMatrix);
            _characterViewBoundingBox = TransformRectangle(_characterTexture.Bounds, _characterWorldMatrix);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _spriteBatch.Draw(
                _mapTexture,
                _mapViewMatrix.Translation,
                _mapTexture.Bounds,
                Color.White,
                _mapViewMatrix.Rotation,
                Vector2.Zero,
                _mapViewMatrix.Scale,
                SpriteEffects.None,
                0);

            _spriteBatch.Draw(
                _characterTexture,
                _characterWorldMatrix.Translation,
                _characterTexture.Bounds,
                Color.Aqua,
                _characterWorldMatrix.Rotation,
                Vector2.Zero,
                _characterWorldMatrix.Scale,
                SpriteEffects.None,
                1);
            
            _spriteBatch.DrawRectangle(_mapViewBoundingBox, Color.Red);
            _spriteBatch.DrawString(_fontArial14, $"Hovered over: {_hoverText}", new Vector2(14, 2), Color.White);
            _spriteBatch.DrawRectangle(_characterViewBoundingBox, Color.Red);

            if (_mouseOverMapCoordinates.HasValue)
            {
                var mapCellBoundingBox = new Rectangle(_mouseOverMapCoordinates.Value, new Point(1, 1));
                var mouseOverViewRectangle = TransformRectangle(mapCellBoundingBox, _mapViewMatrix);
                _spriteBatch.DrawRectangle(mouseOverViewRectangle, Color.Blue);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private static RectangleF TransformRectangle(RectangleF mapTextureBounds, Matrix2 mapMatrix)
        {
            var corners = mapTextureBounds.GetCorners().Select(mapMatrix.Transform).ToList();
            var x0 = corners.Min(c => c.X);
            var x1 = corners.Max(c => c.X);
            var y0 = corners.Min(c => c.Y);
            var y1 = corners.Max(c => c.Y);
            var boundingRectangle = RectangleF.CreateFrom(new Point2(x0, y0), new Point2(x1, y1));
            return boundingRectangle;
        }
    }
}