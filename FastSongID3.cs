using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libZPlay; // impl
namespace libap
{
    /**
     * Faster implementation of SongID3
     */
    public class FastSongID3 : SongID3
    {
       
        /**
         * Automatically generate an instance of SongID3 by loading ID3 of file "filename".
         * \param filename The file name of the song to load ID3 from.
         */
        public FastSongID3(string filename)
        {
            this.FILENAME = filename;
            refreshID3();
        }
        public override void refreshID3()
        {
            // (impl
            try
            {
                ZPlay zp = new ZPlay();
                TID3InfoEx tinf = new TID3InfoEx();
                TID3Info tinf2 = new TID3Info();
                if ((zp.LoadFileID3(this.FILENAME, TStreamFormat.sfAutodetect, TID3Version.id3Version1, ref tinf2) && tinf2.Title != ""))
                {
                    impl_loadID3Info(tinf2);
                }
                else
                {
                    loadBasicInfo();
                }
            }
            catch
            {
                loadBasicInfo();
            }
            // impl)
        }
    }
}
