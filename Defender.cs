using SplashKitSDK;
using System;
using System.Collections.Generic;

public class Defender
{
    
    private int _shootDelay;
    private uint _lastShootTime;
    private Window _gameWindow;
    public Bitmap ShipBitmap { get; private set; }
    public Sprite ShipSprite;
    private Bullet? _bullet;
    private SplashKitSDK.Timer _gameTimer;
    public double X { get; set; }
    public double Y { get; set; }
    public bool Active { get; set; }
    private List<Enemy> _enemies;
    private float _angle;
    private Ship _shipDetails;
    private List<Enemy> _enemiesInRange;

    public Defender(Ship shipDetails, Window gameWindow, SplashKitSDK.Timer gameTimer, List<Enemy> enemies)
    {
        _gameTimer = gameTimer;
        _shootDelay = shipDetails.ShootDelay;
        Active = false;
        ShipBitmap = shipDetails.ShipBitmap;
        _gameWindow = gameWindow;
        _enemies = enemies;
        _shipDetails = shipDetails;
        _enemiesInRange = new List<Enemy>();

        ShipSprite = SplashKit.CreateSprite("defender", shipDetails.ShipBitmap);
        SplashKit.SpriteSetRotation(ShipSprite, (float) shipDetails.Angle);

        X = shipDetails.ShipX;
        Y = shipDetails.ShipY;

        SplashKit.LoadSoundEffect("shoot", "Sounds/laserSmall_001.ogg");
    }

    public void Draw()
    {
        SplashKit.MoveSpriteTo(ShipSprite, X, Y);
        SplashKit.DrawSprite(ShipSprite);

        if (Active && IsWithinRange() && _bullet != null)
        {
            _bullet.DrawBullet(_angle);
        }
    }

    public void Update()
    {
        if (Active && IsWithinRange())
        {
            RotateShip();

            if ((SplashKit.TimerTicks(_gameTimer) - _lastShootTime) >= _shootDelay)
            { 
                _bullet = new Bullet(ShipSprite, _enemiesInRange, _angle, _gameWindow, _shipDetails.BulletBitmap);
                SplashKit.PlaySoundEffect("shoot");
                _lastShootTime = SplashKit.TimerTicks(_gameTimer);
            }

            _bullet?.UpdateBullet();
            HandleBulletCollisions();
        }
    }

    private void HandleBulletCollisions()
    {
        foreach (Enemy enemy in _enemies)
        {
            if (_bullet != null && _bullet.CollidedWith(enemy))
            {
                _bullet = null;
                enemy.Lives--;
                if (enemy.Lives < 0) {
                    enemy.Lives = 0; 
                }
            }
        }
    }

    private bool IsWithinRange()
    {
        Point2D centerOfShip = new Point2D
        {
            X = ShipSprite.X + ShipSprite.Width / 2,
            Y = ShipSprite.Y + ShipSprite.Height / 2
        };

        Circle range = SplashKit.CircleAt(centerOfShip, _shipDetails.RangeRadius);
        _enemiesInRange = new List<Enemy>();

        foreach (Enemy enemy in _enemies)
        {
            if (SplashKit.PointInCircle(enemy.EnemyPosition, range))
            {
                _enemiesInRange.Add(enemy);
            }
        }

        return _enemiesInRange.Count > 0;
    }

    private void RotateShip()
    {
        if (_enemiesInRange.Count > 0)
        {
            double dx = _enemiesInRange[0].X - ShipSprite.X;
            double dy = _enemiesInRange[0].Y - ShipSprite.Y;
            double tempAngle = (Math.Atan2(dy, dx) * (180 / Math.PI)) + 90;
            _angle = Convert.ToSingle(tempAngle);
            SplashKit.SpriteSetRotation(ShipSprite, _angle);
        }
    }
}
