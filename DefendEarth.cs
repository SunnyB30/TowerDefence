using System.Collections.Generic;
using SplashKitSDK;

public class DefendEarth
{
    private Window _gameWindow;
    private List<Enemy> _enemies = new List<Enemy>();
    private List<Ship> _ships;
    private List<Defender> _playerShips = new List<Defender>();
    private Circle _startButton;
    private Bitmap _startButtonBitmap;
    private Point2D _buttonPoint;
    private bool _gameStarted;
    private const int SpawnCount = 30;
    private const int SpawnDensity = 50;
    private Rectangle _boundaryOne;
    private Rectangle _boundaryTwo;
    private SplashKitSDK.Timer _gameTimer;
    private Circle _range;
    private int _playerMaxHealth = 100;
    private int _playerCurrentHealth = 100;

    public DefendEarth(Window gameWindow, SplashKitSDK.Timer gameTimer)
    {
        _gameWindow = gameWindow;
        _gameTimer = gameTimer;
        _gameStarted = false;
        _buttonPoint = new Point2D { X = 725, Y = 555 };

        _startButton = SplashKit.CircleAt(_buttonPoint, 30);
        _startButtonBitmap = new Bitmap("startbutton", "images/forward.png");

        _boundaryOne = SplashKit.RectangleFrom(0, 167, _gameWindow.Width, 178);
        _boundaryTwo = SplashKit.RectangleFrom(0, 550, _gameWindow.Width, 50);

        for (int i = 0; i < SpawnCount; i++)
        {
            int random = SplashKit.Rnd(0, 10);
            if (random > 5)
            {
                UFO ufo = new UFO(_gameWindow, i, SpawnDensity);
                _enemies.Add(ufo);
            }
            else
            {
                SmallMeteor smallMeteor = new SmallMeteor(_gameWindow, i, SpawnDensity);
                _enemies.Add(smallMeteor);
            }
        }

        _ships = new List<Ship>
        {
            new Ship(
                new Bitmap("bullet1", "images/bullet1.png"),
                new Bitmap("ship1", "images/spaceShip1.png"),
                144, 533, 144, 533, 500, 100, SplashKit.OptionRotateBmp(0), 0
            ),
            new Ship(
                new Bitmap("bullet2", "images/bullet2.png"),
                new Bitmap("ship2", "images/spaceShip2.png"),
                594, 526, 594, 526, 400, 200, SplashKit.OptionRotateBmp(0), 0
            )
        };

        SplashKit.LoadSoundEffect("pickship", "Sounds/forceField_001.ogg");
    }

    public void Draw()
    {
        SplashKit.FillCircle(SplashKit.RGBColor(212, 77, 175), _startButton);
        SplashKit.DrawBitmap(_startButtonBitmap, _buttonPoint.X - _startButtonBitmap.Width / 2 + 4, _buttonPoint.Y - _startButtonBitmap.Height / 2);

        if (_gameStarted)
        {
            foreach (Enemy enemy in _enemies)
            {
                enemy.Draw();
            }
        }

        foreach (var ship in _ships)
        {
            if (ship != null)
            {
                SplashKit.DrawBitmap(ship.ShipBitmap, ship.ShipX, ship.ShipY, ship.Rotate);
            }
        }

        foreach (Defender playerShip in _playerShips)
        {
            playerShip.Draw();
        }
    }

    public void Update()
    {
        if (_gameStarted)
        {
            foreach (Enemy enemy in _enemies)
            {
                enemy.Update();
            }

            foreach (Defender playerShip in _playerShips)
            {
                playerShip.Active = true;
                playerShip.Update();
            }
        }

        ManageLives();
        DrawHealthBar();
    }

    public void ManageLives()
    {
        List<Enemy> enemiesToRemove = new List<Enemy>();

        foreach (Enemy enemy in _enemies)
        {
            if (enemy.IsOffScreen())
            {
                _playerCurrentHealth -= enemy.Lives;
            }

            if (enemy.Lives == 0 || enemy.IsOffScreen())
            {
                enemiesToRemove.Add(enemy);
            }
        }

        foreach (Enemy enemy in enemiesToRemove)
        {
            _enemies.Remove(enemy);
        }

        if (_enemies.Count == 0)
        {
            _gameStarted = false;
            DisplayWin();
        }

        if (_playerCurrentHealth <= 0)
        {
            _playerCurrentHealth = 0;
            _gameStarted = false;
            DisplayLose();
        }
    }

    public void DisplayLose()
    {
        Bitmap lose = new Bitmap("lose", "images/lose.png"); 
        
        SplashKit.DrawBitmap(lose, _gameWindow.Width/2 - lose.Width/2, _gameWindow.Height/2 - lose.Height/2);
    }

    public void DisplayWin()
    {
        Bitmap win = new Bitmap("win", "images/win.png"); 
        
        SplashKit.DrawBitmap(win, _gameWindow.Width/2 - win.Width/2, _gameWindow.Height/2 - win.Height/2);
    }

    public void HandleInput()
    {
        Point2D mouse = SplashKit.MousePosition();

        if (SplashKit.MouseClicked(MouseButton.LeftButton) && SplashKit.PointInCircle(mouse, _startButton))
        {
            _gameStarted = true;
        }

        foreach (var ship in _ships)
        {
            if (SplashKit.MouseDown(MouseButton.LeftButton) && SplashKit.BitmapPointCollision(ship.ShipBitmap, ship.ShipX, ship.ShipY, mouse.X, mouse.Y) && !IsCollidingWithOtherShips(ship))
            {
                ship.ShipX = mouse.X - ship.ShipBitmap.Width / 2;
                ship.ShipY = mouse.Y - ship.ShipBitmap.Height / 2;
                ship.ShipSelected = true;
                ShowRange(ship);

                Vector2D scroll = SplashKit.MouseWheelScroll();
                ship.Angle += 5 * scroll.Y;
                ship.Rotate = SplashKit.OptionRotateBmp(ship.Angle);

            }

            if (ship.ShipSelected && SplashKit.MouseDown(MouseButton.LeftButton))
            {
                ship.ShipX = mouse.X - ship.ShipBitmap.Width / 2;
                ship.ShipY = mouse.Y - ship.ShipBitmap.Height / 2;
                ShowRange(ship);
            }

            if (ship.ShipSelected && SplashKit.MouseUp(MouseButton.LeftButton))
            {
                ship.Rotate = SplashKit.OptionRotateBmp(0);

                if (IsOutOfBounds(ship) || IsCollidingWithOtherShips(ship))
                {
                    ship.ShipX = ship.OriginX;
                    ship.ShipY = ship.OriginY;
                    ship.Rotate = SplashKit.OptionRotateBmp(0);
                    ship.Angle = 0;
                    ship.ShipSelected = false;
                }
                else
                {
                    SplashKit.PlaySoundEffect("pickship");
                    _playerShips.Add(new Defender(ship, _gameWindow, _gameTimer, _enemies));

                    ship.ShipX = ship.OriginX;
                    ship.ShipY = ship.OriginY;
                    ship.Rotate = SplashKit.OptionRotateBmp(0);
                    ship.Angle = 0;
                    ship.ShipSelected = false;
                }
            }
        }
    }

    public void ShowRange(Ship ship)
    {
        Point2D centerOfRange = new Point2D
        {
            X = ship.ShipX + ship.ShipBitmap.Width / 2,
            Y = ship.ShipY + ship.ShipBitmap.Height / 2
        };

        if (IsCollidingWithOtherShips(ship) || IsOutOfBounds(ship))
        {
            Color transparentColor = SplashKit.RGBAColor(240, 128, 128, 40);
            _range = SplashKit.CircleAt(centerOfRange, ship.RangeRadius);

            SplashKit.FillCircle(transparentColor, _range);
        }
        else
        {
            Color transparentColorNotInRange = SplashKit.RGBAColor(0, 0, 0, 40);
            _range = SplashKit.CircleAt(centerOfRange, ship.RangeRadius);

            SplashKit.FillCircle(transparentColorNotInRange, _range);
        }



    }

    private bool IsCollidingWithOtherShips(Ship ship)
    {
        foreach (var playerShip in _playerShips)
        {
            if (SplashKit.SpriteBitmapCollision(playerShip.ShipSprite, ship.ShipBitmap, ship.ShipX, ship.ShipY))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsOutOfBounds(Ship ship)
    {
        return SplashKit.BitmapRectangleCollision(ship.ShipBitmap, ship.ShipX, ship.ShipY, _boundaryOne) ||
               SplashKit.BitmapRectangleCollision(ship.ShipBitmap, ship.ShipX, ship.ShipY, _boundaryTwo);
    }

    private void DrawHealthBar()
    {
        if (_playerCurrentHealth > 0)
        {
            int barWidth = 200;
            int barHeight = 20;
            int x = 50;
            int y = 50;
            int radius = barHeight / 2;

            int filledBarWidth = (int)((_playerCurrentHealth / (float)_playerMaxHealth) * barWidth);

            Color startColor = Color.Red;
            Color endColor = Color.Green;
            float healthFactor = _playerCurrentHealth / (float)_playerMaxHealth;
            Color currentColor = InterpolateColor(startColor, endColor, healthFactor);

            SplashKit.FillRectangle(Color.Gray, x + radius, y, barWidth - barHeight, barHeight);
            SplashKit.FillCircle(Color.Gray, x + radius, y + radius, radius);
            SplashKit.FillCircle(Color.Gray, x + barWidth - radius, y + radius, radius);

            SplashKit.FillRectangle(currentColor, x + radius, y, filledBarWidth - barHeight, barHeight);
            SplashKit.FillCircle(currentColor, x + radius, y + radius, radius);
            SplashKit.FillCircle(currentColor, x + filledBarWidth - radius, y + radius, radius);

            string healthText = $"Health: {_playerCurrentHealth}/{_playerMaxHealth}";
            int fontSize = 24;
            SplashKit.DrawText(healthText, Color.White, "Arial", fontSize, x + 7 , y - 10);
        }
    }

    private static Color InterpolateColor(Color start, Color end, float factor)
    {
        factor = Math.Max(0, Math.Min(1, factor));

        float r = start.R + (end.R - start.R) * factor;
        float g = start.G + (end.G - start.G) * factor;
        float b = start.B + (end.B - start.B) * factor;
        float a = start.A + (end.A - start.A) * factor;

        return SplashKit.RGBAColor(r, g, b, a);
    }
}

public class Ship
{
    public Bitmap BulletBitmap { get; set; }
    public Bitmap ShipBitmap { get; set; }
    public double ShipX { get; set; }
    public double ShipY { get; set; }
    public double OriginX { get; set; }
    public double OriginY { get; set; }
    public bool ShipSelected { get; set; }
    public int ShootDelay { get; set; }
    public int RangeRadius { get; set; }
    public DrawingOptions Rotate { get; set; }
    public double Angle { get; set; }

    public Ship(Bitmap bulletBitmap, Bitmap shipBitmap, double shipX, double shipY, double originX, double originY, int shootDelay, int rangeRadius, DrawingOptions rotate, double angle)
    {
        BulletBitmap = bulletBitmap ?? throw new ArgumentNullException(nameof(bulletBitmap));
        ShipBitmap = shipBitmap ?? throw new ArgumentNullException(nameof(shipBitmap));
        ShipX = shipX;
        ShipY = shipY;
        OriginX = originX;
        OriginY = originY;
        ShipSelected = false;
        ShootDelay = shootDelay;
        RangeRadius = rangeRadius;
        Rotate = rotate;
        Angle = angle;
    }
}
