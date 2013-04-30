using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bombersquad_ai
{
    /*
     * Configuration data for our game
     * Store all static values here
     */
    class GameConfig
    {
        // Visualization Data
        public static int graphicsWidth = 640;              // 20 tiles * 32 pixels
        public static int graphicsHeight = 640;             // 20 tiles * 32 pixels
        public static bool isFullScreen = false;            // Window v/s Fullscreen
        public static string contentDirectory = "Content";  // DO NOT change this!
        
        // Game Tile Data
        public static int gameWidth = 20;                   // Number of tiles horizontally
        public static int gameHeight = 20;                  // Number of tiles vertically
        public static int tileWidth = 32;                   // 640 pixels / 20 tiles
        public static int tileHeight = 32;                  // 640 pixels / 20 tiles

        // Player Starting Position
        public static int playerStartX = 1;
        public static int playerStartY = 3;

        // GameState Data
        public static int maxNumAIBombers = 4;              //number of AI players
        public static int explosionRadius = 3;              // Radius for bomb explosions
        
        // AI Starting Positions
        // TODO: Randomize in bottom right corner
        public static int Bomber1StartX = gameWidth - 1;
        public static int Bomber1StartY = gameWidth - 1;
        public static int Bomber2StartX = gameWidth - 3;
        public static int Bomber2StartY = gameWidth - 1;
        public static int Bomber3StartX = gameWidth - 5;
        public static int Bomber3StartY = gameWidth - 1;
        public static int Bomber4StartX = gameWidth - 7;
        public static int Bomber4StartY = gameWidth - 1;

    }
}
