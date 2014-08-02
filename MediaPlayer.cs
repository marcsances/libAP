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
using libZPlay; // impl
namespace libap
{
    public class MediaPlayer
    {
        private static MediaPlayer instance = null;
        private AudioFile af = null; ///< The audio file of this instance.

        private ZPlay impl_ap = null; ///< (IMPLEMENTATION EXCLUSIVE) The audio player instance.

        private bool media_ready = false;
        private bool paused = false;
        private bool stopped = false;
        /**
         * A class providing full media playback functionality.
         */

        /**
         * Constructor of this class.
         * Note: Make sure file does exist or an exception will be thrown.
         * \param af the AudioFile instance of the file to be played.
         */
        public MediaPlayer(AudioFile af)
        {
            this.af = af;
            this.impl_ap = new ZPlay(); // impl
            if (!this.impl_ap.OpenFile(this.af.FileName, TStreamFormat.sfAutodetect)) { throw new Exception("Unknown error during file initialization"); }
            else { this.media_ready = true; this.stopped = true; this.paused = false;  }// impl 
        }

        /**
         * Plays the audio file.
         * \return true if success, or false if file didn't exist, media was unavailable or there was an error.
         */
        public bool play()
        {
            if (this.media_ready && !this.paused && this.stopped)
            {
                if (this.impl_ap.StartPlayback())
                {
                    if (this.OnPlaybackStarted!=null) this.OnPlaybackStarted(this,new EventArgs());
                    this.stopped = false;
                    return true;
                }
                else return false;
            }
            else if (this.media_ready && !this.paused && !this.stopped)
            {
                return true; // media already playing
            }
            else if (this.media_ready) {
                if (this.impl_ap.ResumePlayback()) {
                    if (this.OnPlaybackResumed!=null) this.OnPlaybackResumed(this, new EventArgs());
                    this.paused = false;
                    return true;
                } else {
                return false;
                } 
            }
            else
            {
                return false; // media not ready
            }
        }

        /**
         * Pauses playback.
         * \return true if paused successfully or false if media was unavailable or there was an error.
         */
        public bool pause()
        {
            if (this.media_ready && !this.paused)
            {
                if (this.impl_ap.PausePlayback())
                {
                    if (this.OnPlaybackPaused!=null) this.OnPlaybackPaused(this, new EventArgs());
                    this.paused = true;
                    return true;
                }
                else return false;
            }
            else return this.media_ready; // true if already paused, false if media unavailable
        }

        /**
         * Stops playback.
         * \return true if stopped succesfully or false if media was unavailable or there was an error.
         */
        public bool stop()
        {
            if (this.media_ready && !this.stopped)
            {
                if (this.impl_ap.StopPlayback())
                {
                    this.paused = false;
                    this.stopped = true;
                    return true;
                }
                else return false;
            }
            else return this.media_ready; // true if already stopped, false if media unavailable
        }

        /**
         * If playing, pauses playback. If stopped or paused, begins or resumes playback.
         * \return whatever returns the called function
         */
        public bool playpause()
        {
            if (this.media_ready && (this.paused || this.stopped))
            {
                return this.play();
            }
            else return this.pause();
        }

        /**
         * Checks if it's now playing.
         * \return if it's now playing.
         */
        public bool playing()
        {
            return this.media_ready && !this.paused && !this.stopped;
        }


        /**
         * Seek the playback to a position.
         * \param pos The position to seek, from 0 to L.
         * \return true if seeked successfully, or false if position was outside bounds, there was an error, or media was not ready.
         */
        public bool seek(long pos)
        {
            if (this.media_ready && !this.stopped)
            {
                TStreamTime tt = new TStreamTime();
                tt.ms = (uint)pos;
                return this.impl_ap.Seek(TTimeFormat.tfMillisecond, ref tt, TSeekMethod.smFromBeginning);
            }
            else return false;
        }

        /**
         * Gets the length of the audio file, in milliseconds.
         * \return The length, in milliseconds, or -1 if media is not available.
         */
        public long getLength()
        {
            if (this.media_ready)
            {
                TStreamInfo inf = new TStreamInfo();
                this.impl_ap.GetStreamInfo(ref inf);
                return inf.Length.ms;
            }
            else return -1;
        }


        public bool checkIfEndedTrigger()
        {
            TStreamStatus tst = new TStreamStatus();
            this.impl_ap.GetStatus(ref tst);
            if (!tst.fPlay && !this.paused && !this.stopped)
            {
                if (this.OnPlaybackFinished!=null) this.OnPlaybackFinished(this, new EventArgs());
                return true;
            }
            else return false;
        }

        /**
         * Gets current position in audio playback.
         * \return The position, in milliseconds, or -1 if media is not available.
         */

        public long getPosition()
        {
            if (this.media_ready)
            {
                if (!this.paused && !this.stopped) checkIfEndedTrigger();
                TStreamTime inf = new TStreamTime();
                if (this.impl_ap!=null) this.impl_ap.GetPosition(ref inf);
                return inf.ms;
            }
            else return -1;
        }

        /**
         * Sets the volume for the audio player instance.
         * \param vol the volume, from 0 to 100.
         * \return true if success, false if failed.
         */
        public bool setVolume(int vol)
        {
            if (this.media_ready)
            {
                return this.impl_ap.SetPlayerVolume(vol, vol);
            }
            else return false;
        }
        /**
         * Gets the volume for the audio player instance.
         * \return the volume from 0 to 100 or -1 if failed.
         */
        public int getVolume()
        {
            if (this.media_ready)
            {
                int v = 0;
                int k = 0;
                this.impl_ap.GetPlayerVolume(ref v, ref k);
                return (v + k) / 2;
            }
            else return -1;
        }

        /**
         * Gets a player. Prevents multiple players from getting stacked, allowing easier coding.
         * If there is no current instance, inits one.
         * \return The instance.
         */
        public static MediaPlayer getInstance(AudioFile af)
        {
            if (instance != null)
            {
                instance.Dispose();
            }
            instance = new MediaPlayer(af);
            return instance;
        }

        /**
         * Similar to previous, but will not init instance if null.
         * \return The instance.
         */
        public static MediaPlayer getInstance()
        {
            return instance;
        }

        /**
         * Dispose this class.
         */
        public void Dispose()
        {
            
            try
            {
                
                if (this.impl_ap != null)
                {
                    this.impl_ap.Close();
                    this.impl_ap = null;
                }
            }
            finally
            {
                this.impl_ap = null;
            }

        }

        public event EventHandler OnPlaybackStarted;
        public event EventHandler OnPlaybackFinished;
        public event EventHandler OnPlaybackPaused;
        public event EventHandler OnPlaybackResumed;

        /**
         * \brief (SPECIAL METHOD) Calls a method from the implementation, if available.
         * Calls a method from the implementation API, if available.
         * Refer to IMPL_API.txt for a list of available functions in the implementation.
         * \param opcode The operation code of the API feature.
         * \param paramArray An array of objects with the parameters that are given to the API feature. Refer to implementation documentation above for details.
         * \return A null object array if the function is not implmented in this implementation, or an object array (including an empty one) if the function was implemented and performed.
         */
        public Object[] impl_command(string opcode, Object[] paramArray)
        {
            switch (opcode)
            {
                case "ZPLAYVER":
                    return ZPLAYVER(paramArray);
                case "SETTEMPO":
                    return SETTEMPO(paramArray);
                case "SETPITCH":
                    return SETPITCH(paramArray);
                case "GETPITCH":
                    return GETPITCH(paramArray);
                case "GETTEMPO":
                    return GETTEMPO(paramArray);
                case "GETZPLAY":
                    Object[] arr={this.impl_ap};
                    return arr;
                default:
                    return null;
            }
        }

        // From now on, all implementation functions.

        private Object[] ZPLAYVER(Object[] paramArray)
        {
            Object[] retzpv = { new ZPlay().GetVersion() };
            return retzpv;
        }

        private Object[] SETTEMPO(Object[] paramArray)
        {
            if (paramArray.Length == 1)
            {
                bool res = false;
                try
                {
                    res = this.media_ready && this.impl_ap.SetTempo((int)paramArray[0]);
                }
                catch
                {

                }
                Object[] retst = { res };
                return retst;
            }
            else
            {
                throw new ArgumentException("SETTEMPO expects one parameter.");
            }
        }

        private Object[] SETPITCH(Object[] paramArray)
        {

            if (paramArray.Length == 1)
            {
                bool res = false;
                try
                {
                    res = this.media_ready && this.impl_ap.SetPitch((int)paramArray[0]);
                }
                catch
                {

                }
                Object[] retsp = { res };
                return retsp;
            }
            else
            {
                throw new ArgumentException("SETPITCH expects one parameter.");
            }
        }

        private Object[] GETPITCH(Object[] paramArray)
        {
            int resgp;
            if (this.media_ready) { resgp = this.impl_ap.GetPitch(); } else { resgp = -1; }
            Object[] retgp = { resgp };
            return retgp;
        }

        private Object[] GETTEMPO(Object[] paramArray)
        {
            int resgp;
            if (this.media_ready) { resgp = this.impl_ap.GetTempo(); } else { resgp = -1; }
            Object[] retgp = { resgp };
            return retgp;
        }
    }
}
