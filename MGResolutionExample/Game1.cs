using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MGResolutionExample;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    //  Is screen resizing flag
    private bool _isResizing;

    //  Screen size
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int ViewWidth { get; private set; }
    public int ViewHeight { get; private set; }

    //  Screen scale matrix
    public Matrix ScreenScaleMatrix { get; private set; }

    //  Screen Viewport
    public Viewport Viewport { get; private set; }

    //  View padding, amount to apply for letter/pillar boxing
    private int _viewPadding;
    public int ViewPadding
    {
        get => _viewPadding;
        set
        {
            //  Only perform view update if the value is changed
            if (_viewPadding != value)
            {
                _viewPadding = value;
                UpdateView();
            }
        }
    }

    //  Just a rectangle to represent a flat surface, or floor in our world
    private Rectangle _screenRect;

    //  just a rectangle to represent the "player"
    private Rectangle _playerRect;

    //  A 1x1 pixel that will be used to draw the screen and player texture.
    private Texture2D _pixel;

    //  The camera
    private Basic2DCamera _camera;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;

        //  Width and Height are our internal rendering resolution, we'll set this low for example purposes
        Width = 640;
        Height = 360;

        //  Hook into these events so we can recalculate the screen scale matrix when needed
        _graphics.DeviceCreated += OnGraphicsDeviceCreated;
        _graphics.DeviceReset += OnGraphicsDeviceReset;
        Window.ClientSizeChanged += OnWindowSizeChanged;


        //  Applying my graphics settings, we'll start with a 1280x720 window (this is 2x our width and height resolution)
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();



    }

    private void OnGraphicsDeviceCreated(object sender, EventArgs e)
    {
        //  When graphics device is created, call UpdateView to recalculate the screen scale matrix
        UpdateView();
    }

    private void OnGraphicsDeviceReset(object sender, EventArgs e)
    {
        //  When graphics device is reset, call UpdateView to recalculate the screen scale matrix
        UpdateView();
    }

    private void OnWindowSizeChanged(object sender, EventArgs e)
    {
        //  Window size changing is a little different, we only want to call UpdateView when it's finished resizing
        //  for instance, if the user clicks and drags the window border, during each size change we don't want
        //  to call UpdateView, so we use the _isResizing flag
        if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !_isResizing)
        {
            _isResizing = true;

            //  Set the backbuffer width and height to the window bounds
            _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

            //  Now update the view
            UpdateView();

            _isResizing = false;
        }
    }


    /// <summary>
    ///     Updates the values for the graphics view such as the screen matrix
    ///     and viewport to provide independent resolution rendering.
    /// </summary>
    /// <!--
    ///     The method for indpendent resolution rendering comes from the 
    ///     Monocle Engine developed by Maddy Thorson and used in the games 
    ///     Towerfall and Celeste. The Monocle Engine was originally found at 
    ///     https://bitbucket.org/MattThorson/monocle-engine however the source
    ///    code does not seem to be available any more at this link.
    ///     
    ///     Monocole is licensed under the MIT License.
    /// -->
    private void UpdateView()
    {
        float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
        float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        // get View Size
        if (screenWidth / Width > screenHeight / Height)
        {
            ViewWidth = (int)(screenHeight / Height * Width);
            ViewHeight = (int)screenHeight;
        }
        else
        {
            ViewWidth = (int)screenWidth;
            ViewHeight = (int)(screenWidth / Width * Height);
        }

        // apply View Padding
        var aspect = ViewHeight / (float)ViewWidth;
        ViewWidth -= ViewPadding * 2;
        ViewHeight -= (int)(aspect * ViewPadding * 2);

        // update screen matrix
        ScreenScaleMatrix = Matrix.CreateScale(ViewWidth / (float)Width);

        // update viewport
        Viewport = new Viewport
        {
            X = (int)(screenWidth / 2 - ViewWidth / 2),
            Y = (int)(screenHeight / 2 - ViewHeight / 2),
            Width = ViewWidth,
            Height = ViewHeight,
            MinDepth = 0,
            MaxDepth = 1
        };
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData<Color>(new Color[] { Color.White });

        _screenRect = new Rectangle(0, 0, Width, Height);

        //  Setting the player to a 32x32 sprite, but setting the position to be in the center of the screen rect
        //  which is why width and height are halved and then 16 (half the player size) subtracted
        _playerRect = new Rectangle((Width / 2) - 16, (Height / 2) - 16, 32, 32);

        //  Create camera
        _camera = new(Width, Height);

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        KeyboardState keyState = Keyboard.GetState();

        //  Move player up/down/left/right
        if (keyState.IsKeyDown(Keys.Left))
        {
            _playerRect.X -= 1;
        }
        else if (keyState.IsKeyDown(Keys.Right))
        {
            _playerRect.X += 1;
        }
        else if (keyState.IsKeyDown(Keys.Up))
        {
            _playerRect.Y -= 1;
        }
        else if (keyState.IsKeyDown(Keys.Down))
        {
            _playerRect.Y += 1;
        }

        //  Ensure camera is centered on player
        _camera.Position = _playerRect.Location.ToVector2() + (new Vector2(_playerRect.Size.X, _playerRect.Size.Y) * 0.5f);
        _camera.CenterOrigin();



        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {

        GraphicsDevice.Viewport = Viewport;
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(transformMatrix: _camera.TransformationMatrix * ScreenScaleMatrix);
        _spriteBatch.Draw(_pixel, _screenRect, null, Color.Orange);
        _spriteBatch.Draw(_pixel, _playerRect, null, Color.Blue);
        _spriteBatch.End();


        base.Draw(gameTime);
    }
}
