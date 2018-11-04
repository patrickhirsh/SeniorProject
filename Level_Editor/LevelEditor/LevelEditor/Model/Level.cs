using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace LevelEditor
{
    
    public class Level
    {
        //Bool of whether the level file has ever been created
        bool savedBefore;
        //bool of whether the level has been edited since the last save
        bool savedCurrent;

        //TODO: figure out why this isn't serializing
        //Dictionary that contains the tilesets, keyed by coordiantes
        [JsonProperty]
        Dictionary<Triple, tileSet> masterList;

        public Level()
        {
            masterList = new Dictionary<Triple, tileSet>();
        }

        public tileSet getDefaultTileSet()
        {
            //if the northwestmost tileset exists in this level, then return that
            if(masterList.ContainsKey(new Triple(0,0,0)))
            {
                return masterList[new Triple(0,0,0)];
            }
            //elsewise, just create a new tileset and return that. 
            else
            {
                Triple Z = new Triple(0, 0, 0);
                masterList.Add(Z, new tileSet());
                return masterList[Z];
            }
        }

        //File Name
        public string FileName { get; internal set; }

        /// <summary>
        /// For determining whether the level has ever been saved
        /// </summary>
        /// <returns>whether this level has been saved before or if the save dialog needs to be opened </returns>
        public bool getSaved()
        {
            return savedBefore;
        }

        //setting whether the level has been saved or not
        internal void setSaved()
        {
            savedBefore = true;
        }

        /// <summary>
        /// For determining if the level has been saved since being edited
        /// </summary>
        /// <returns>whether this level has been edited since it's been saved (returns false if edits have happened since last save)</returns>
        public bool getCSaved()
        {
            return savedCurrent;
        }

        //setting the level as saved with no new edits
        internal void setCSaved()
        {
            savedCurrent = true;
        }

        //Returns the tileset at a given coordinate in a level
        public tileSet GetTileSet(int a, int b, int c)
        {
            Triple triple = new Triple(a, b, c);
            if (masterList.ContainsKey(triple))
                return masterList[triple];
            else
                return new tileSet();
        }

        //getting a copy of the masterlist 
        public Dictionary<Triple, tileSet> getMasterlist()
        {
            return masterList;
        }

        //list to hold transfer tiles objects in any given level
        [JsonProperty]
        List<transferTile> ttiles;

        //struct to hold assorted level details
        [JsonProperty]
        levelDetails details;

        internal void setTile(int rowIndex, int columnIndex, int iD)
        {
            throw new NotImplementedException();
        }
    }

    public struct levelDetails
    {
        //number of angry cars spawned in a level
        int angryCarNum;
        //number of passive cars spawned in a level
        int passiveCarNum;
        //number id for aesthetic style in a level. 
        int styleNum;
    }
}
