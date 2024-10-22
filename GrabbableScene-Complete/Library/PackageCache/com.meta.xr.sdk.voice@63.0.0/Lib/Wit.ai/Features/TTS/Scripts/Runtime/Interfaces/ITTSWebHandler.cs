/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * This source code is licensed under the license found in the
 * LICENSE file in the root directory of this source tree.
 */

using Meta.WitAi.TTS.Data;
using Meta.WitAi.TTS.Events;

namespace Meta.WitAi.TTS.Interfaces
{
    public interface ITTSWebHandler
    {
        /// <summary>
        /// Any web request performs this event
        /// </summary>
        TTSWebRequestEvents WebRequestEvents { get; }

        /// <summary>
        /// Streaming events
        /// </summary>
        TTSStreamEvents WebStreamEvents { get; }

        /// <summary>
        /// Method for determining if there are problems that will arise
        /// with performing a web request prior to doing so
        /// </summary>
        /// <param name="clipData">The clip data to be used for the request</param>
        /// <returns>Invalid error(s).  It will be empty if there are none</returns>
        string GetWebErrors(TTSClipData clipData);

        /// <summary>
        /// Method for performing a web load request
        /// </summary>
        /// <param name="clipData">Clip request data</param>
        void RequestStreamFromWeb(TTSClipData clipData);

        /// <summary>
        /// Cancel web stream
        /// </summary>
        /// <param name="clipID">Clip unique identifier</param>
        bool CancelWebStream(TTSClipData clipData);

        /// <summary>
        /// Download events
        /// </summary>
        TTSDownloadEvents WebDownloadEvents { get; }

        /// <summary>
        /// Method for performing a web load request
        /// </summary>
        /// <param name="clipData">Clip request data</param>
        /// <param name="downloadPath">Path to save clip</param>
        void RequestDownloadFromWeb(TTSClipData clipData, string downloadPath);

        /// <summary>
        /// Cancel web download
        /// </summary>
        /// <param name="clipID">Clip unique identifier</param>
        /// <param name="downloadPath">Path to save clip</param>
        bool CancelWebDownload(TTSClipData clipData, string downloadPath);
    }
}
