﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using Plugin.MediaManager.Abstractions;
using UIKit;

namespace Plugin.MediaManager
{
    public class MediaExtractorImplementation : IMediaExtractor
    {
        public async Task<IMediaFile> ExtractMediaInfo(IMediaFile mediaFile)
        {
            try
            {
                var assetsToLoad = new List<string>
                {
                    AVMetadata.CommonKeyArtist,
                    AVMetadata.CommonKeyTitle,
                    AVMetadata.CommonKeyArtwork
                };
                var nsUrl = new NSUrl(mediaFile.Url);
            
                // Default title to filename
                mediaFile.Metadata.Title = nsUrl.LastPathComponent;
            
                var asset = AVAsset.FromUrl(nsUrl);
                await asset.LoadValuesTaskAsync(assetsToLoad.ToArray());

                foreach (var avMetadataItem in asset.CommonMetadata)
                {
                    if (avMetadataItem.CommonKey == AVMetadata.CommonKeyArtist)
                    {
                        mediaFile.Metadata.Artist = ((NSString) avMetadataItem.Value).ToString();
                    }
                    else if (avMetadataItem.CommonKey == AVMetadata.CommonKeyTitle)
                    {
                        mediaFile.Metadata.Title = ((NSString) avMetadataItem.Value).ToString();
                    }
                    else if (avMetadataItem.CommonKey == AVMetadata.CommonKeyArtwork)
                    {
                        var image = UIImage.LoadFromData(avMetadataItem.DataValue);
                        mediaFile.Metadata.Cover = image;
                    }
                }
                return mediaFile;
            }
            catch (Exception)
            {
                return mediaFile;
            }
        }
    }
}