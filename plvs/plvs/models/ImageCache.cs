using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Atlassian.plvs.api;
using Atlassian.plvs.util;

namespace Atlassian.plvs.models {
    internal class ImageCache {

        public class ImageInfo {
            public ImageInfo(Image img, Uri fileUrl) {
                Img = img;
                FileUrl = fileUrl != null ? fileUrl.ToString() : null;
            }

            public Image Img { get; private set; }
            public string FileUrl { get; private set; }
        }

        private static readonly ImageCache INSTANCE = new ImageCache();

        public static ImageCache Instance {
            get { return INSTANCE; }
        }

        private readonly SortedDictionary<string, ImageInfo> cache = new SortedDictionary<string, ImageInfo>();

        private readonly string iconCacheDir;
        
        public ImageCache() {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            dir = Path.Combine(dir, "Atlassian Connector for Visual Studio\\Icons");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            iconCacheDir = dir;
        }

        public ImageInfo getImage(Server server, string url) {
            if (url == null) {
                return new ImageInfo(Resources.nothing, null);
            }
            lock (this) {
                if (cache.ContainsKey(url)) {
                    return cache[url];
                }
                try {
                    HttpWebResponse response = getResponse(server, url);
                    var responseStream = response.GetResponseStream();
                    
                    byte[] imgbytes = PlvsUtils.getBytesFromStream(responseStream);

                    Image img = Image.FromStream(new MemoryStream(imgbytes));

                    var fileName = iconCacheDir + "\\" + getFileName(url);
                    using (FileStream f = File.Create(fileName)) {
                        f.Write(imgbytes, 0, imgbytes.Length);
                        f.Close();
                    }
                    ImageInfo imageInfo = new ImageInfo(img, new Uri(fileName));
                    cache[url] = imageInfo;
                    return imageInfo;
                } catch (Exception e) {
                    Debug.WriteLine("ImageCache.getImage() - exception: " + e.Message);
                    cache[url] = new ImageInfo(Resources.nothing, null);
                    return new ImageInfo(Resources.nothing, null);
                }
            }
        }

        private static HttpWebResponse getResponse(Server server, string url) {
            string authUrl = server == null ? url : url + "?" + CredentialUtils.getOsAuthString(server);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(authUrl);
            request.KeepAlive = true;

            if (server != null) {
                request.Credentials = CredentialUtils.getCredentialsForUserAndPassword(url, server.UserName, server.Password);
            }

            request.Accept = @"image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, */*";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return response;
        }

        public void clear() {
            lock (this) {
                cache.Clear();
            }
        }

        private static string getFileName(string url) {
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(url));
            return Convert.ToBase64String(hash).Replace("/", "-");
        }
    }
}