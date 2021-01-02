using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Utf8Json;
using System.Threading.Tasks;
using Utf8Json.Resolvers;

namespace Application
{
    public static class Json
    {
        public static readonly IJsonFormatterResolver Standard = StandardResolver.ExcludeNullCamelCase;
        public static readonly IJsonFormatterResolver SnakeCase = StandardResolver.SnakeCase;

        public static readonly IJsonFormatterResolver AllowPrivate = StandardResolver.AllowPrivateExcludeNullCamelCase;

        public static string Serialize(object obj)
        {
            return JsonSerializer.ToJsonString(obj, Standard);
        }

        public static string SerializeSnakeCase(object obj)
        {
            return JsonSerializer.ToJsonString(obj, SnakeCase);
        }

        public static byte[] SerializeToBytes(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static Task SerializeAsync(Stream str, object obj)
        {
            return JsonSerializer.SerializeAsync(str, obj, Standard);
        }

        public static Task SerializePrivateAsync(Stream str, object obj)
        {
            return JsonSerializer.SerializeAsync(str, obj, AllowPrivate);
        }

        /// <summary>
        /// Use this as the point of last resort. There are Stream and byte[] options. There's no need
        /// to convert everything to string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string str)
        {
            return JsonSerializer.Deserialize<T>(str, Standard);
        }

        public static T Deserialize<T>(Stream st)
        {
            return JsonSerializer.Deserialize<T>(st, Standard);
        }

        public static T Deserialize<T>(byte[] bytes)
        {
            return JsonSerializer.Deserialize<T>(bytes, Standard);
        }

        public static T DeserializePrivate<T>(byte[] bytes)
        {
            return JsonSerializer.Deserialize<T>(bytes, AllowPrivate);
        }

        public static Task<T> DeserializeAsync<T>(Stream st)
        {
            return JsonSerializer.DeserializeAsync<T>(st, Standard);
        }

        public static M DeserializeFilter<M>(string json)
        {
            var unescaped = Compression.Decompress(Uri.UnescapeDataString(json));

            using (var input = new StringReader(unescaped))
            {
                return Deserialize<M>(input.ReadToEnd());
            }
        }
    }

    internal static class Compression
    {
        public static string Compress(string s)
        {
            var bytes = Encoding.Unicode.GetBytes(s);
            using (var msi = new MemoryStream(bytes))
            {
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress))
                    {
                        msi.CopyTo(gs);
                    }

                    return Convert.ToBase64String(mso.ToArray());
                }
            }
        }

        public static string Decompress(string s)
        {
            var bytes = Convert.FromBase64String(s);
            using (var msi = new MemoryStream(bytes))
            {
                using (var mso = new MemoryStream())
                {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress))
                    {
                        gs.CopyTo(mso);
                    }

                    return Encoding.Unicode.GetString(mso.ToArray());
                }
            }
        }
    }

}

