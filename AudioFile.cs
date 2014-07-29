/*
 * This file is part of libAP.

    LibAP is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    LibAP is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with libAP.  If not, see <http://www.gnu.org/licenses/>.
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libap
{
    /**
     * A comparable class representing an audio file.
     * Includes the ID3 information of it.
     */
    [Serializable()]
    public class AudioFile : IComparable
    {
        private string FILENAME; ///< The file name this instance points to.
        private FastSongID3 SONG_ID3; ///< The ID3 information of the song.
        /**
         * Constructor of this class.
         * Can throw an exception if file was not found.
         * \param filename the file name representing this audio file.
         */
        public AudioFile(string filename)
        {
            this.FILENAME = filename;
            this.SONG_ID3 = new FastSongID3(filename);
            
        }
        public void Dispose()
        {
            this.FILENAME = "";
            this.SONG_ID3.Dispose();
        }
        public SongID3 ID3Information
        {
            get {
                return this.SONG_ID3;
            }
        }

        public string FileName
        {
            get
            {
                return this.FILENAME;
            }
        }

        public int CompareTo(object af)
        {
            if (af is AudioFile)
            {
                return this.SONG_ID3.Title.CompareTo(((AudioFile)af).SONG_ID3.Title);
            }
            else return 1;
        }
    }

}
