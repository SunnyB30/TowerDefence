using SplashKitSDK;

public abstract class Enemy
{
    public Sprite? EnemySprite { get; protected set; }
    public double X { get; protected set; }
    public double Y { get; protected set; }
    protected Window GameWindow { get; }
    public Point2D EnemyPosition { get; protected set; }

    protected int _lives;

    public virtual int Lives
    {
        get { return _lives; }
        set { _lives = value; }
    }

    protected Enemy(Window gameWindow)
    {
        GameWindow = gameWindow;
    }

    public virtual void Draw()
    {
        if (EnemySprite != null)
        {
            SplashKit.MoveSpriteTo(EnemySprite, X, Y);
            SplashKit.DrawSprite(EnemySprite);
        }
    }

    public virtual void Update()
    {
        const int Speed = 1;
        X += Speed;

        if (EnemySprite != null)
        {
            SplashKit.MoveSpriteTo(EnemySprite, X, Y);
            EnemyPosition = SplashKit.CenterPoint(EnemySprite);
        }
    }

    public bool IsOffScreen()
    {
        return X > GameWindow.Width;
    }
}
