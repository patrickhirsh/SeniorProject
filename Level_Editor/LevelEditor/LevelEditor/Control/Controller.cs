using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace LevelEditor
{
    public class Controller
    {
        //The current level being edited
        Level currentLevel;

        //The current tileset being edited (what's currently displayed in editor)
        tileSet currentTileSet;
        
        public Controller()
        {
            currentLevel = new Level();
            currentTileSet = currentLevel.getDefaultTileSet();
        }

        //export to JSON file function
        public void save()
        {
            string writeString = JsonConvert.SerializeObject(currentLevel);
            System.Diagnostics.Debug.WriteLine(writeString);
            if (currentLevel.getSaved())
            {
                System.IO.File.WriteAllText(currentLevel.FileName, writeString);
                currentLevel.setSaved();
                currentLevel.setCSaved();
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JSON File| *.json";
                saveFileDialog.Title = "Export Level File";
                saveFileDialog.ShowDialog();

                if(saveFileDialog.FileName != "")
                {
                    System.IO.File.WriteAllText(saveFileDialog.FileName, writeString);
                    currentLevel.FileName = saveFileDialog.FileName;
                }

                currentLevel.setSaved();
                currentLevel.setCSaved();
            }
            //CalcAndSavePathfinding();

        }

        /// <summary>
        /// Algorithm only used in saving, not loading
        /// Finds all possible paths out of each tile, 
        /// and saves them in a structure of nodes for use 
        /// by the Dijkstra algorithm in Unity
        /// </summary>
        private void CalcAndSavePathfinding()
        {
            /*So in here we need to basically iterate through all of the cells in our griddataview 
             * With each of these cells, we need to get the tileID, and then based on what that tiles behavior is
             * we need to create a node object for the current tile (if it is a navigation node) and then set it's children
             * based on where it can navigate to. 
             */
             //TODO: Actually figure out how to store children 
            foreach(tileSet x in currentLevel.getMasterlist().Values)
            {
                for (int i = 0; i < Model.tileH; i++)
                {
                    for(int j = 0; j < Model.tileW; j++)
                    {
                        calcTile(x.getTile(i, j));
                    }
                }
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculate the child nodes of a given tile
        /// based on that tile's ID. 
        /// </summary>
        /// <param name="tile"></param>
        private void calcTile(tile tile)
        {
            switch(tile.getTileID()){
                /*
                 * In here is where we'd program all of the behavior for tiles once we determine what tiles we have.
                 * Easily addable/modifiable, just increment tileid and throw in a new case and you're pretty much done.
                 */ 
                case 1:
                    break;

                default:
                    break;
            }
                
        }

        /// <summary>
        /// Loads a levels data into the editors current level data for editing
        /// </summary>
        //TODO: Make it so that this saves previous level datat before loading up new data so that data isn't lost.
        public void load()
        {
            // Displays an OpenFileDialog so the user can select a Cursor.  
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Level files|*.json";
            openFileDialog1.Title = "Select a Cursor File";

            // Show the Dialog.  
            // If the user clicked OK in the dialog and  
            // a .CUR file was selected, open it.  
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.  
                this.currentLevel = JsonConvert.DeserializeObject<Level>(openFileDialog1.OpenFile().ToString());
            }
        }

        /// <summary>
        /// This needs to set the id of the corresponding tile in the "level" to equal the tile ID it was set to in the view
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        internal void SetTileID(int rowIndex, int columnIndex, int ID)
        {
            currentTileSet.setTile(rowIndex, columnIndex, ID);
        }


        //This needs to actually populate the level editor with the data of the new level data
        //TODO: Have this reset and then populate the level editor with new data. 
        internal void setCurrentEditorLevel()
        {
            throw new NotImplementedException();
        }

    }


}
