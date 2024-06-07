using System.Collections.Generic;
using SplashKitSDK;

public class Bullet
{
    public Sprite BulletSprite { get; private set; }
    private Sprite _defenderSprite;
    private Vector2D _velocity;
    private double _xBullet;
    private double _yBullet;
    private float _angle;
    private List<Enemy> _enemiesInRange;
    private Window _gameWindow;

    public Bullet(Sprite defenderSprite, List<Enemy> enemiesInRange, float angle, Window gameWindow, Bitmap bulletBitmap)
    {
        _defenderSprite = defenderSprite;
        _enemiesInRange = enemiesInRange;
        _angle = angle;
        _gameWindow = gameWindow;
        
        BulletSprite = SplashKit.CreateSprite("bullet", bulletBitmap);

        _xBullet = _defenderSprite.X + _defenderSprite.Width / 2 - BulletSprite.Width / 2;
        _yBullet = _defenderSprite.Y + _defenderSprite.Height / 2 - BulletSprite.Height / 2;

        SplashKit.SpriteSetRotation(BulletSprite, _angle);
        SplashKit.MoveSpriteTo(BulletSprite, _xBullet, _yBullet);
    }

    public void DrawBullet(float angle)
    {
        SplashKit.SpriteSetRotation(BulletSprite, angle);
        SplashKit.MoveSpriteTo(BulletSprite, _xBullet, _yBullet);
        SplashKit.DrawSprite(BulletSprite);
    }

    public void UpdateBullet()
    {
        const int Speed = 20;
        Point2D fromPoint = new Point2D { X = _xBullet, Y = _yBullet };

        Point2D toPoint = _enemiesInRange[0].EnemyPosition;

        Vector2D direction = SplashKit.UnitVector(SplashKit.VectorPointToPoint(fromPoint, toPoint));
        _velocity = SplashKit.VectorMultiply(direction, Speed);

        _xBullet += _velocity.X;
        _yBullet += _velocity.Y;
        SplashKit.UpdateSprite(BulletSprite);
    }


    public bool CollidedWith(Enemy enemy)
    {
        if (enemy.EnemySprite != null) {
            return SplashKit.SpriteCollision(BulletSprite, enemy.EnemySprite);
        }
        return false; 
        
    }
}
