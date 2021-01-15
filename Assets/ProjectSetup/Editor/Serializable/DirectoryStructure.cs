using System;
using System.Collections.Generic;

namespace JellyFish.Setup
{
    [Serializable]
    public class DirectoryStructure
    {
        #region VARIABLES

        /// <summary>
        /// List of Directories to Create.
        /// </summary>
        public List<string> Directories = new List<string>();

        #endregion
    }
}