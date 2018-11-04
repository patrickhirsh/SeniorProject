using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Newtonsoft.Json;

namespace LevelEditor
{
    public class tileSet
    {
        //[x,y] coordinate based dictionary to hold all the tiles in a tile set
        [JsonProperty]
        Dictionary<Tuple<int, int>, tile> tiles = new Dictionary<Tuple<int, int>, tile>();
        //coordiantes of this tile set
        [JsonProperty]
        Triple location;
        //get the id of a given tile
        public int getTileID(int x, int y)
        {
            Tuple<int, int> newTup = new Tuple<int, int>(x, y);
            return tiles[newTup].getTileID();
        }
        //get the tile object at a given coordinate
        public tile getTile(int x, int y)
        {
            Tuple<int, int> newTup = new Tuple<int, int>(x, y);
            return tiles[newTup];
        }

        internal void setTile(int rowIndex, int columnIndex, int iD)
        {
            Tuple<int, int> tuple = new Tuple<int, int>(rowIndex, columnIndex);
            tiles[tuple] = new tile(iD);
        }
    }

    public class tile
    {
        //tile ID
        [JsonProperty]
        int tileID;

        //constructor
        public tile(int x)
        {
            tileID = x;
        }

        //getter for tile ID
        public int getTileID()
        {
            return tileID;
        }

    }

    /// <summary>
    /// Transfer tile is a tile object that can path to an adjacent tileset, and thus will need to hold some extra info
    /// </summary>
    public class transferTile
    {
        //tile ID
        [JsonProperty]
        int tileID;

        //getter for tile ID
        public int getTileID()
        {
            return tileID;
        }
        
        //coordinates of tileset that this tile with path to
        Triple adjacentTileset;
    }
}
