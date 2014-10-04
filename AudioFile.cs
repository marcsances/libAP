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
    public class AudioFile : IComparable
    {
        private string FILENAME; ///< The file name this instance points to.
        private SongID3 SONG_ID3; ///< The ID3 information of the song.

        /**
         * Blank constructor.
         * For internal usage.
         */
        private AudioFile()
        {

        }
        /**
         * Constructor of this class.
         * Can throw an exception if file was not found.
         * \param filename the file name representing this audio file.
         */
        public AudioFile(string filename)
        {
            this.FILENAME = filename;
            this.SONG_ID3 = new SongID3(filename);
            
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

        /**
         * These functions are under test.
         * Do not use.
         */

        private void UnitTest()
        {
            Console.WriteLine("Performing unit tests over AudioFile.cs...");
            string a = "FILE 1";
            string b = "FILE 2";
            string c = "FILE 20";
            string d = "FILE 30";
            string e = "03 - file";
            string f = "FILA 2";
            string g = "15 - file";
            Console.WriteLine("TEST 1. Expected result: -1 >>> " + CompareStrings(a,b));
            Console.WriteLine("TEST 1. Expected result: -1 >>> " + CompareStrings(a, c));
            Console.WriteLine("TEST 1. Expected result: -1 >>> " + CompareStrings(b, c));
            Console.WriteLine("TEST 1. Expected result: -1 >>> " + CompareStrings(c, d));
            Console.WriteLine("TEST 1. Expected result: -1 >>> " + CompareStrings(e, g));
            Console.WriteLine("TEST 1. Expected result: 1 >>> " + CompareStrings(a, f));
            Console.WriteLine("TEST 1. Expected result: 0 >>> " + CompareStrings(a, a));
            Console.WriteLine("TEST 1. Expected result: 1 >>> " + CompareStrings(g, e));
        }

        private int FindFullNumber(char[] a, int pos, int no)
        {
            if (pos==a.Length || !Char.IsDigit(a[pos])) return no;
            else
            {
                return FindFullNumber(a, pos + 1, no * 10 + a[pos]);
            }
        }

        private int FindFullNumber(char[] a, int pos)
        {
            if (a.Length==0 || Char.IsDigit(a[pos])) return FindFullNumber(a, pos + 1, a[pos]);
            else return 0;
        }

        private int CompareDigits(char[] a, char[] b, int pos)
        {
            if (Char.IsDigit(a[pos]) && !Char.IsDigit(b[pos]))
            {
                return -1;
            }
            else if (!Char.IsDigit(a[pos]) && Char.IsDigit(a[pos]))
            {
                return 1;
            }
            else if (!Char.IsDigit(a[pos]) && !Char.IsDigit(b[pos]))
            {
                return CompareStrings(a, b, pos);
            }
            else {
                int ai = FindFullNumber(a, pos);
                int bi = FindFullNumber(b, pos);
                if (ai < bi) return -1;
                else if (ai == bi) return CompareStrings(a, b, pos + ai.ToString().Length);
                else return 1;
            }
        }

        private int CompareStrings(char[] a, char[] b, int pos)
        {
            if (a.Length==pos) {
                if (b.Length==pos) {return 0;}
                else {return -1;}
            }
            else if (b.Length == pos)
            {
                return 1;
            }
            if (!Char.IsDigit(a[pos]) && !Char.IsDigit(b[pos]))
            {
                if (a[pos]<b[pos]) {
                    return -1;
                } else if (a[pos]==b[pos]) {
                    return CompareStrings(a,b,pos+1);
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                return CompareDigits(a, b, pos);
            }

        }

        private int CompareStrings(string a, string b)
        {
            return CompareStrings(a.ToCharArray(), b.ToCharArray(), 0);
        }

        /** End of tests **/
        public static bool once = false;
        public int CompareTo(object af)
        {
            if (!once)
            {
                //UnitTest();
                once = true;
            }
            if (af is AudioFile)
            {
                //return CompareStrings(this.SONG_ID3.Title,((AudioFile)af).SONG_ID3.Title);
                return this.SONG_ID3.Title.CompareTo(((AudioFile)af).SONG_ID3.Title);
            }
            else return 1;
        }

        /**
         * Converts this instance to a file.
         * \return A string with the content to write to file.
         */
        public string toFile()
        {
            return this.ID3Information.toFile();
        }

        /**
         * Creates a new instance from a file.
         * \param The string of the file.
         * \return the new instance.
         */
        public static AudioFile fromFile(string data)
        {
            AudioFile af = new AudioFile();
            af.SONG_ID3 = SongID3.fromFile(data);
            af.FILENAME = af.SONG_ID3.FileName;
            return af;
        }
    }

}
