using System;
using SplashKitSDK;

namespace Task7_3D
{
    public class Program
    {
        public static void Main()
        {

            Window gameWindow = new Window ("gameWindow", 800, 600); 
            Bitmap backgroundBitmap = new Bitmap("Background", "images/background1.png");
            SplashKitSDK.Timer gameTimer = new SplashKitSDK.Timer("scoreTimer");
            gameTimer.Start();

            DefendEarth defendEarth = new DefendEarth(gameWindow, gameTimer);

           
            while(!gameWindow.CloseRequested) {
               
                SplashKit.ProcessEvents();
                gameWindow.Clear(Color.White);
                backgroundBitmap.Draw(0, 0); 
                defendEarth.Draw();
                defendEarth.Update(); 
                defendEarth.HandleInput();
                gameWindow.Refresh(30); 
            }
            gameWindow.Close(); 

        }
    }
}
