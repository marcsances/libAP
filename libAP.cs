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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
namespace libap
{
    /**
     * Provides library related functions.
     */
    public class libAP
    {
        public const int LIBAP_API_VER = 1; ///< Returns the API version of libap API implemented in this library.
        public const double LIBAP_API_IMPL = 1.0; ///< Returns the version of this implementation of libap API.
        public const int NT_COMPATIBLE = 2; ///< Returns 0 if not compatible with NT platforms, 1 if compatibility is possible, but not guaranteed, 2 if fully compatible.
        public const int W98SE_COMPATIBLE = 0; ///< Returns 0 if not compatible with Windows 98SE/ME, 1 if compatibility is possible, but not guaranteed, 2 if fully compatible.
        public const int WINE_COMPATIBLE = 1; ///< Returns 0 if not compatible with Wine platforms, 1 if compatibility is possible, but not guaranteed, 2 if fully compatible.
        public const int MONO_NT_COMPATIBLE = 1; ///< Returns 0 if not compatible with Mono on NT platforms, 1 if compatibility is possible, but not guaranteed, 2 if fully compatible.
        public const int MONO_POSIX_COMPATIBLE = 0; ///< Returns 0 if not compatible wit Mono on POSIX platforms, 1 if compatibility is possible, but not guaranteed, 2 if fully compatible.
        public const int WINCE_COMPATIBLE = 0; ///< Returns 0 if not compatible with Windows CE/Mobile platforms, 1 if compatibility is possible, but not guaranteed, 2 if fully compatible.
        public const int WIN8_COMPATIBLE = 2; ///< Returns 0 if not compatible with Windows 8 Store platforms, 1 if compatibility is possible, but not guaranteed, 2 if fully compatible.
        public const int WINPHO_COMPATIBLE = 0; ///< Returns 0 if not compatible with Windows Phone platforms, 1 if compatibility is possible, but not guaranteed, 2 if fully compatible.
        public const int XBOX_XNA_COMPATIBLE = 0; ///< Returns 0 if not compatible with Xbox XNA platforms, 1 if compatibility is possible, but not guaranteed, 2 if fully compatible.
        public const bool CAN_SAVE_ID3 = false; ///< True if this implementation can save ID3 information to the file.

        /**
         * Get the base name of that filename, without extension.
         * \return the base name of the file.
         */
        public static string basename(string filename)
        {
            string[] filenamearr = filename.Split(getDirectorySeparator(filename));
            return filenamearr[filenamearr.Length - 1].Split('.')[0];
        }


        /**
         * Portability method: Check which is the directory separator for the given path name.
         * \param filename the full path of the file.
         */
        private static char getDirectorySeparator(string filename)
        {
            char[] directorycharacters = { '\\', '/' };
            try
            {
                return filename.Substring(filename.IndexOfAny(directorycharacters), 1).ToCharArray()[0];
            }
            catch
            {
                return '\\'; // Feeling lucky.
            }
        }

        /**
         * In Windows, paths are c:\whatever\else.ext or c:/whatever/else.txt alike.
         * In POSIX systems, paths are /whatever/else.ext alike.
         * We don't want "c:" as our parent directory, so we'd need a bigger parameter for length comparison in Windows.
         * This method gives that value.
         * \param filename the full path of the file.
         * \return the minimum length of the splitted file that does have parent directory.
         */
        private static int getMinimumPathContainers(string filename)
        {
            if (getDirectorySeparator(filename) == '\\')
            {
                return 3; // c:\folder\file.ext (Windows)
            }
            else if (filename.IndexOf(':') != -1)
            {
                return 3; // c:/folder/file.ext (usual on Windows file: directions)
            }
            else
            {
                return 2; // /folder/file.ext (POSIX system)
            }
        }

        /**
         * Check which folder contains the file.
         * \param filename the full path of the file.
         * \return the parent directory name or empty string if file had no parent.
         */
        public static string parentfolder(string filename)
        {
            string[] filenamearr = filename.Split(getDirectorySeparator(filename));
            if (filenamearr.Length >= getMinimumPathContainers(filename))
            {
                return filenamearr[filenamearr.Length - 2];
            }
            else
            {
                return "";
            }
        }

        /**
         * Retrieves license information from libAP for About box.
         * \return A string containing a license header.
         */
        public static string getLicenseInfo()
        {
            return "This program uses libAP.\r\nLibAP is © 2014, MSS Software & Services and licensed under the GNU Lesser General Public License.\r\nThis program uses libzplay. Libzplay is licensed under the GNU General Public License.\r\nShould you have not received a copy of any, visit http://www.gnu.org/licenses/.";
        }

        /**
         * Exports an AudioFile member to a file via serialization.
         * \return True if success.
         */
        public static bool exportAudioFile(AudioFile af, String filename)
        {
            File.WriteAllText(filename, af.toFile());
            return true;
        }

        /**
         * Imports an AudioFile member from a file via deserialization.
         * \return AudioFile instance or null if failed.
         */
        public static AudioFile importAudioFile(String filename)
        {
            return AudioFile.fromFile(File.ReadAllText(filename));
        }

        /**
         * utility method for other functions
         * \param str A string.
         * \return The bytes of that string.
         */
        public static byte[] GetStrBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }


        /**
         * utility method for other functions
         * \param str A byte array
         * \return The string of that array.
         */
        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

       
    }
}