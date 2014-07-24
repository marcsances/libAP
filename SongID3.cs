﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libZPlay; // impl
namespace libap
{
    /**
     * A class to provide an universal framework for ID3 information manipulation. This class is independent from the ID3 metasystem used.
     */
    public class SongID3
    {
        protected string FILENAME; ///< The file name of this instance.
        private string TITLE; ///< The title of the song.
        private string ALBUM; ///< The album of the song.
        private string ARTIST; ///< The artist of the song.
        private string COMMENT; ///< The comment of the song.
        private string GENRE; ///< The genre of the song.
        private int YEAR; ///< The year of the song.
        private Bitmap ALBUM_IMAGE; ///< The album image of the song.
        private int TRACKNO; ///< The track number of the song.

        public SongID3()
        {

        }
        /**
         * Automatically generate an instance of SongID3 by loading ID3 of file "filename".
         * \param filename The file name of the song to load ID3 from.
         */
        public SongID3(string filename)
        {
            this.FILENAME = filename;
            refreshID3();
        }

        /**
         * Create a manual instance of SongID3 for saving ID3 information (if implemented).
         * \param filename The file name of the song
         * \param title The song title
         * \param album The song album
         * \param artist The song artist
         * \param comment The song comment
         * \param genre The song genre
         * \param year The song year
         * \param albumImage The album image of the song
         * \param trackno The track number
         */
        public SongID3(string filename, string title, string album, string artist, string comment, string genre, int year, Bitmap albumImage, int trackno)
        {
            this.FILENAME = filename;
            this.TITLE = title;
            this.ALBUM = album;
            this.COMMENT = comment;
            this.GENRE = genre;
            this.YEAR = year;
            this.ALBUM_IMAGE = albumImage;
            this.TRACKNO = trackno;
        }

        /**
         * (IMPLEMENTATION ONLY)
         * Load extended ID3 file information.
         * \param tinf the ID3InfoEx instance.
         */
        protected void impl_loadID3InfoEx(TID3InfoEx tinf)
        {
            this.TITLE = tinf.Title;
            this.ARTIST = tinf.Artist;
            this.ALBUM = tinf.Album;
            this.COMMENT = tinf.Comment;
            this.GENRE = tinf.Genre;
            try
            {
                this.TRACKNO = Convert.ToInt32(tinf.Track);
            }
            catch
            {
                this.TRACKNO = 0;
            }
            try
            {
                this.YEAR = Convert.ToInt32(tinf.Year);
            }
            catch
            {
                this.YEAR = 0;
            }
            if (tinf.Picture.PicturePresent)
            {
                this.ALBUM_IMAGE = tinf.Picture.Bitmap;
            }
        }

        /**
         * (IMPLEMENTATION ONLY)
         * Load standard ID3 file information.
         * \param tinf the ID3Info instance.
         */
        protected void impl_loadID3Info(TID3Info tinf)
        {
            this.TITLE = tinf.Title;
            this.ARTIST = tinf.Artist;
            this.ALBUM = tinf.Album;
            this.COMMENT = tinf.Comment;
            this.GENRE = tinf.Genre;
            try
            {
                this.TRACKNO = Convert.ToInt32(tinf.Track);
            }
            catch
            {
                this.TRACKNO = 0;
            }
            try
            {
                this.YEAR = Convert.ToInt32(tinf.Year);
            }
            catch
            {
                this.YEAR = 0;
            }
        }

        /**
         * Load basic file information.
         */
        protected void loadBasicInfo()
        {
            this.TITLE = libAP.basename(this.FILENAME);
            this.ARTIST = "";
            this.ALBUM = libAP.parentfolder(this.FILENAME);
            this.ALBUM_IMAGE = null;
            this.COMMENT = "";
            this.YEAR = 0;
            this.TRACKNO = 0;
            this.GENRE = "";
        }
        public void Dispose()
        {
            this.TITLE = "";
            this.ARTIST = "";
            this.ALBUM = "";
            this.ALBUM_IMAGE = null;
            this.COMMENT = "";
            this.YEAR = 0;
            this.TRACKNO = 0;
            this.GENRE = "";
        }
        /**
         * Refresh or load ID3 information from file.
         */
        public virtual void refreshID3()
        {
            // (impl
            try
            {
                ZPlay zp = new ZPlay();
                TID3InfoEx tinf=new TID3InfoEx();
                TID3Info tinf2 = new TID3Info();
                if (zp.LoadFileID3Ex(this.FILENAME, TStreamFormat.sfAutodetect, ref tinf,true) && tinf.Title != "")
                {
                    impl_loadID3InfoEx(tinf);
                }
                else if ((zp.LoadFileID3(this.FILENAME,TStreamFormat.sfAutodetect,TID3Version.id3Version2,ref tinf2) && tinf2.Title!="") || (zp.LoadFileID3(this.FILENAME,TStreamFormat.sfAutodetect,TID3Version.id3Version1,ref tinf2) && tinf2.Title!="")) 
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

        /**
         * Get the title of the song.
         */
        public string Title
        {
            get
            {
                return this.TITLE;
            }
        }

        /**
         * Get the album of the song.
         */
        public string Album {
            get {
                return this.ALBUM;
            }
        }

        /**
         * Get the artist name of the song.
         */
        public string Artist {
            get {
                return this.ARTIST;
            }
        }

        /**
         * Get the comment of the song.
         */
        public string Comment
        {
            get
            {
                return this.COMMENT;
            }
        }

        /**
         * Get the genre of the song.
         */
        public string Genre
        {
            get
            {
                return this.GENRE;
            }
        }

        /**
         * Get the year of the song.
         */
        public int Year
        {
            get
            {
                return this.YEAR;
            }
        }

        /**
         * Get the album image of the song, if available.
         */
        public Bitmap AlbumImage
        {
            get
            {
                return this.ALBUM_IMAGE;
            }
        }

        /**
         * Get the track number of the song.
         */
        public int TrackNumber
        {
            get
            {
                return this.TRACKNO;
            }
        }

        /**
         * Save ID3 information to file. Just create a new instance with destination file and all ID3 info you want to save, then call this method from that instance.
         * Please note: To prevent a NotImplementedException in your code, double-check that this function is available in current implementation.
         * Example:
         *      if (libap.CAN_SAVE_ID3) { new SongID3(filename,title,album,artist,comment,genre,year,albumImage,trackno).save(); }
         * \throws NotImplementedException
         */
        public void save()
        {
            throw new NotImplementedException("This feature is not available in current libAP implementation. Please contact application developer.");
        }

    }
}
