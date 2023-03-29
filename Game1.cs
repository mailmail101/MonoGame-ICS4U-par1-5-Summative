using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;
namespace Project4;
public class Game1 : Game
{
    enum Screen
    {
        Intro,
        TribbleRanch,
        TibbleDestroyer,
        GameOver
    }
    private GraphicsDeviceManager _graphics;
    private Double _destroyerTimeStart;
    private int _shotsThatHit = 0;
    private Texture2D _TribbleBackGround;
    private int _shotsFired = 0;
    private Texture2D _endBackGround;
    public Texture2D _laser;
    public int _currentDeadTribbles;
    private Texture2D _TempestBomber;
    private List<Point> _explosions = new List<Point>();
    private int _totalTribbles;
    private List<Tribble> _tribbles = new List<Tribble>();
    private List<Laser> _lasers = new List<Laser>();
    private Texture2D _explsion;
    private SpriteBatch _spriteBatch;
    private SpriteFont _freeSansBold;
    private SpriteFont _freeSansBold36;
    private TempestBomber _bomber = new TempestBomber();
    private Double _endGameTime;
    private MouseState _lastMouseState;
    public  double _sizeFactor = 0.1;
    private Color _backGroundColor = Color.Blue;
    private int[] _window = {1280, 720};
    Screen _screen;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _lastMouseState = Mouse.GetState();
         _currentDeadTribbles = 0;
        _graphics.PreferredBackBufferWidth = _window[0];
       _graphics.PreferredBackBufferHeight = _window[1];
       _graphics.ApplyChanges();
       this.Window.Title = "Tribbles";
        _screen =Screen.Intro;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _freeSansBold36 = Content.Load<SpriteFont>("FreeSansBold36");
        _freeSansBold = Content.Load<SpriteFont>("FreeSansBold");
        _TribbleBackGround = Content.Load<Texture2D>("TribbleBackGround");
        _TempestBomber = Content.Load<Texture2D>("TempestBomber");
        _laser = Content.Load<Texture2D>("Laser");
        _explsion = Content.Load<Texture2D>("explosion");
        _endBackGround = Content.Load<Texture2D>("warpEnterprise");
        for (int i = 0; i < 20; i ++)
        {
             _tribbles.Add(new Tribble(Content.Load<Texture2D>("tribbleBrown"), 403, 399, _window, _sizeFactor));
            _tribbles.Add(new Tribble(Content.Load<Texture2D>("tribbleCream"), 332, 294, _window,_sizeFactor));
            _tribbles.Add(new Tribble(Content.Load<Texture2D>("tribbleGrey"), 419, 419, _window, _sizeFactor));
            _tribbles.Add(new Tribble(Content.Load<Texture2D>("tribbleOrange"), 339, 290, _window, _sizeFactor));
      
        }
        _totalTribbles = _tribbles.Count;
         // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        var mouseState = Mouse.GetState();
        var keyboardState = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        if (_screen == Screen.Intro)
        {
            if (mouseState.RightButton == ButtonState.Pressed || mouseState.LeftButton == ButtonState.Pressed)
            {
                _screen = Screen.TribbleRanch;
            }
        }
        else if (_screen == Screen.TribbleRanch)
        {
            if (mouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Released 
             || mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Released)
            {
                _screen = Screen.TibbleDestroyer;
                _destroyerTimeStart = gameTime.TotalGameTime.TotalSeconds;
            }
        }
        if (_screen == Screen.TribbleRanch || _screen == Screen.TibbleDestroyer)
        {
            foreach (Tribble tribble in _tribbles)
            {
                _backGroundColor = tribble.MoveTribble(_window, _backGroundColor);
            }
        }
        if (_screen == Screen.TibbleDestroyer)
        { 
            List<Laser> lasersToBeRemoved = new List<Laser>();
            foreach(Laser laser in _lasers)
            {
                laser.CalulateMove();
                if (laser.IsAtTarget())
                {
                    bool hit = false;
                    Point explosion = laser.Cords;
                    _explosions.Add(explosion);
                    lasersToBeRemoved.Add(laser);
                    List<Tribble> triblesToBeRemved = new List<Tribble>();
                    foreach (Tribble tribble in _tribbles)
                    {
                        if (tribble.HasBeenHit(explosion))
                        {
                            hit = true;
                            triblesToBeRemved.Add(tribble);
                        }
                    }
                    triblesToBeRemved = triblesToBeRemved.Distinct().ToList();
                    foreach ( Tribble tribble1 in triblesToBeRemved)
                    {
                        _tribbles.Remove(tribble1);
                        _currentDeadTribbles ++;
                    }
                    if (hit) {_shotsThatHit ++;}
                }
            }
            foreach(Laser laser1 in lasersToBeRemoved)
            {
                _lasers.Remove(laser1);
            }
            _bomber.CalulateMove(_window);
            if (mouseState.RightButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Released 
             || mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.RightButton == ButtonState.Released && _lastMouseState.LeftButton == ButtonState.Released)
            {
                _shotsFired ++;
                _lasers.Add(new Laser(_bomber.Cords, mouseState.Position));
            }
        }
        if (_screen == Screen.GameOver && keyboardState.IsKeyDown(Keys.Space)){Environment.Exit(0);}
        // TODO: Add your update logic here
        _lastMouseState = mouseState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(_backGroundColor);
        _spriteBatch.Begin();
        if (_screen == Screen.Intro)
        {
            _spriteBatch.Draw(_TribbleBackGround, new Vector2(0, 0), Color.White);
            _spriteBatch.DrawString(_freeSansBold36, $"Welcome to Tribble ranch click to continue", new Vector2(_window[0] / 10, 0), Color.RoyalBlue);
        }
        if (_screen == Screen.TibbleDestroyer || _screen == Screen.TribbleRanch)
        {
            foreach (Tribble tribble in _tribbles)
            {   
                tribble.DrawTribble(_spriteBatch);
            } 
        }
        if (_screen == Screen.TribbleRanch)
        {
            _spriteBatch.DrawString(_freeSansBold, $"There are to many tribbles use your tempest bomber to get rid of them click to shoot and start the fight", new Vector2(_window[0] / 10, _window[1] - 60), Color.OrangeRed);
        }
        if (_screen == Screen.TibbleDestroyer)
        {
            _spriteBatch.DrawString(_freeSansBold, $"{_currentDeadTribbles} / {_totalTribbles}     {Math.Floor((gameTime.TotalGameTime.TotalSeconds - _destroyerTimeStart) / 60)} : {Math.Round(gameTime.TotalGameTime.TotalSeconds - _destroyerTimeStart - Math.Floor((gameTime.TotalGameTime.TotalSeconds - _destroyerTimeStart) / 60) * 60, 1)}", new Vector2(_window[0] / 2 - 40, 0), Color.Black);
            _bomber.DrawBomber(_spriteBatch, _TempestBomber);
            foreach(Laser laser in _lasers)
            {
                laser.DrawLaser(_spriteBatch, _laser);
            }
            foreach(Point explosion in _explosions)
            {
                _spriteBatch.Draw(_explsion, new Rectangle( explosion.X - 36,  explosion.Y - 36, 76, 76), Color.White);
            }
            if (_totalTribbles == _currentDeadTribbles)
            {
                _endGameTime = gameTime.TotalGameTime.TotalSeconds - _destroyerTimeStart;
                _screen = Screen.GameOver;
            }
        }
        if (_screen == Screen.GameOver)
        {
            _spriteBatch.Draw(_endBackGround, new Rectangle(0, 0, _window[0], _window[1]), Color.White);
            _spriteBatch.DrawString(_freeSansBold36, $"You have saved earth from the tribbles thank you", new Vector2(_window[0] / 15, 100), Color.OrangeRed);
            _spriteBatch.DrawString(_freeSansBold36, $"You killed all {_totalTribbles} in {Math.Floor((_endGameTime - _destroyerTimeStart) / 60)}:{Math.Round(_endGameTime - _destroyerTimeStart - Math.Floor((_endGameTime - _destroyerTimeStart) / 60) * 60, 1)}", new Vector2(_window[0] / 15, 150), Color.OrangeRed);
            _spriteBatch.DrawString(_freeSansBold36, $"You shot {_shotsFired} Bombs with a accuracy of {(_shotsThatHit * 100)/(_shotsFired)}%", new Vector2(_window[0] / 15, 200), Color.OrangeRed);
            _spriteBatch.DrawString(_freeSansBold36, $"press space to end", new Vector2(_window[0] / 15, 250), Color.OrangeRed);
        }
        _spriteBatch.End();
        _explosions.Clear();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
public class Tribble    
    {
        private Texture2D _tribbleName;
        private int _width;
        private int _height;
        private int[] _cords;
        private int[] _speeds;
        private Random _random;

        public Tribble(Texture2D tribbleName, int width, int height, int[] window, double sizeFactor) 
        {

            _random = new Random();
            _tribbleName = tribbleName;
            _width = Convert.ToInt32(width * sizeFactor);
            _height = Convert.ToInt32(height * sizeFactor);
            _cords = PlaceAtRandomLocation(window);
            _speeds = RandomSpeed();
        }
        public int[] RandomSpeed()
        {
            int[] _speeds = {0, 0};
            while (_speeds[0] == 0 & _speeds[1] == 0)
            {
                _speeds[0] = _random.Next(-6, 7);
                _speeds[1] = _random.Next(-6, 7);
            }
            return _speeds;
        }
        public int[] PlaceAtRandomLocation(int[] window)
        {
            int[] _cords = {0, 0};
            _cords[0] = _random.Next(0, window[0] - _width);
            _cords[1] = _random.Next(0, window[1] - _height);
            return _cords;
        }
        public int[] BounceOffWall(int[] window)
        {  
            if (_cords[0] + _width >= window[0] && _speeds[0] > 0 ||  _cords[0] <= 0 && _speeds[0] < 0)
            {
            _speeds[0] *= -1;            
            }
            if (_cords[1]  + _height >= window[1] && _speeds[1] > 0 || _cords[1] <= 0 && _speeds[1] < 0)
            {
                _speeds[1] *= -1;
            }
            return _speeds;
        }
        public Color MoveTribble( int[] window, Color backGround)
        {
            if (_cords[0] + _width >= window[0] | _cords[0] <= 0 | _cords[1] + _height >= window[1] | _cords[1] <= 0)
            {
                if (_random.Next(0, 20) == 1)
                {
                    backGround = new Color (_random.Next(0, 256), _random.Next(0, 256), _random.Next(0, 256));
                }
            }
             _speeds = BounceOffWall(window);
            

            _cords[0] += _speeds[0];
            _cords[1] += _speeds[1];
            return backGround;
        }
        public void DrawTribble(SpriteBatch draw)
        {
            draw.Draw(_tribbleName, new Rectangle(_cords[0], _cords[1], _width, _height), Color.White);
        }
        public bool HasBeenHit(Point explosion)
        {
            if (Math.Abs(explosion.X - _cords[0]) <= 76)
        {
            if (Math.Abs(explosion.Y - _cords[1]) <= 76)
            {
                return true;
            }
        }
            return false;
        }
    }
public class Laser
{
    private Point _speed;
    private Point _target;
    private Point _cords;
    public Point Cords
    {
        get{return _cords;}
    }
    public Laser(Point cords, Point target)
    {
        _target = target;
        _cords = new Point (cords.X + 150, cords.Y + 60);
    }
    public void CalulateMove()
    {
        CalulateSpeed();
        _cords.X += _speed.X;
        _cords.Y += _speed.Y;
    }
    private void CalulateSpeed()
    {
        double distanceToTargetX = Math.Abs(_target.X - _cords.X);
        double distanceToTargetY = Math.Abs(_target.Y - _cords.Y);
        double xSpeed = distanceToTargetX / (distanceToTargetX + distanceToTargetY);
        _speed.X = Convert.ToInt32(xSpeed * 20);
        if (_cords.X > _target.X)
        {
            _speed.X *= -1;
        }
        _speed.Y = 20 - Convert.ToInt32(xSpeed * 20);
        if (_cords.Y > _target.Y)
        {
            _speed.Y *= -1;
        }
    }
    public bool IsAtTarget()
    {
        if (Math.Abs(_target.X - _cords.X) <= 15)
        {
            if (Math.Abs(_target.Y - _cords.Y) <= 15)
            {
                return true;
            }
        }
    return false;
    }
    public void DrawLaser(SpriteBatch draw, Texture2D laser)
        {
            draw.Draw(laser, new Rectangle(_cords.X, _cords.Y, 20, 20), Color.White);
        }
}
public class TempestBomber
{
    private int _speed;
    private Point _cords;
     public Point Cords
    {
        get {return _cords;}
        //set {_cords = value;}
    }
    public TempestBomber()
    {
        _speed = 10;
        _cords.X = 10;
        _cords.Y = 10;
    }
    public void CalulateMove(int[] window)
    {
        _cords.X += _speed;
        CalulateSpeed(window);
    }

    private void CalulateSpeed(int[] window)
    {
        if (_cords.X + 177 >= window[0] || _cords.X <= 10)
        {
           this._speed *= -1;
        }
    }
    public void DrawBomber(SpriteBatch draw, Texture2D bomber)
        {
            draw.Draw(bomber, new Rectangle(_cords.X, _cords.Y, 167, 100), Color.White);
        }
}
}

