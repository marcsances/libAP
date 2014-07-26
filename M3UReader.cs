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
using System.IO;
namespace libap
{
    /**
     * Provides a simple framework for reading and writing M3U playlists.
     */
    public class M3UReader
    {
        public static String[] read(string filename)
        {
            return File.ReadAllText(filename).Replace("\r", "").Replace("\n\n","\n").Split('\n');
        }

        public static void write(string filename, String[] list)
        {
            string lst = "";
            foreach (string k in list)
            {
                lst = lst + k + "\r\n";
            }
            lst = lst.Substring(0, lst.Length - 4); // remove last \r\n
            File.WriteAllText(filename, lst);
        }

        public static void append(string filename, string item)
        {
            File.AppendAllText(filename, "\r\n" + item);
        }
    }
}
